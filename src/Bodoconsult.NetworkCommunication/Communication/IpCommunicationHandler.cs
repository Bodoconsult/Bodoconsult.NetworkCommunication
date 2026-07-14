// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EventSources;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// IP implementation for <see cref="ICommunicationHandler"/> via socket
/// </summary>
public class IpCommunicationHandler : ICommunicationHandler
{
    private readonly IAppEventSource _appEventSource;
    private readonly string _loggerId;

    private IWaitStateManager? _waitStateManager;
    private bool _isInitialized;

    private readonly ProducerConsumerQueue<IInboundDataMessage> _inboundQueue = new();
    private readonly IAppLoggerProxy _monitorLogger;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="duplexIo">Duplex IO communication to use</param>
    /// <param name="dataMessagingConfig">SMD tower master data</param>
    /// <param name="appEventSourceFactory">Current app event source factory used for APM</param>
    public IpCommunicationHandler(IDuplexIo duplexIo, IDataMessagingConfig dataMessagingConfig, IAppEventSourceFactory appEventSourceFactory)
    {
        _appEventSource = appEventSourceFactory.CreateInstance();

        DuplexIo = duplexIo;
        DataMessagingConfig = dataMessagingConfig;
        _monitorLogger = DataMessagingConfig.MonitorLogger;
        _loggerId = $"{DataMessagingConfig.LoggerId}{(dataMessagingConfig.LoggerId.EndsWith(": ") ? "" : ": ")}";

        UpdateDevice();

        DataMessagingConfig.RaiseDataMessageNotSentDelegate = OnMessageNotSent;

        DataMessagingConfig.RaiseCommLayerDataMessageReceivedDelegate = OnReceivedMessage;
        DataMessagingConfig.RaiseUnexpectedDataMessageReceivedDelegate = OnNotExpectedMessageReceivedEvent;

        _inboundQueue.ConsumerTaskDelegate = InboundMessageReachedQueue;
        _inboundQueue.StartConsumer();
    }

    /// <summary>
    /// Activate the received and sent messages logging. Should be turned off in production
    /// </summary>
    public bool ActivateLogging { get; set; }

    /// <summary>
    /// Current <see cref="IDuplexIo"/> instance to use
    /// </summary>
    public IDuplexIo DuplexIo { get; }

    /// <summary>
    /// Current data messaging config
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Update the device wait state config
    /// </summary>
    public void UpdateDevice()
    {
        if (DataMessagingConfig.DataMessageProcessingPackage == null)
        {
            return;
        }
        _waitStateManager = DataMessagingConfig.DataMessageProcessingPackage.WaitStateManager;
    }

    /// <summary>
    /// This message should be fired if a message has not been sent
    /// </summary>
    /// <param name="message">Message not sent</param>
    /// <param name="reason">The reason why the message was not sent</param>
    public virtual void OnMessageNotSent(ReadOnlyMemory<byte> message, string? reason)
    {
        // Do nothing
    }

    private void OnNotExpectedMessageReceivedEvent(IInboundDataMessage e)
    {
        ArgumentNullException.ThrowIfNull(_waitStateManager);
        _waitStateManager.LastMessageTimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public async Task<MessageSendingResult> SendMessage(IOutboundDataMessage message)
    {
        try
        {
            // Todo: check if blocking
            if (ActivateLogging)
            {
                var s = $"Enqueue message {message.ToShortInfoString()}";
                _monitorLogger.LogDebug(s);
            }

            var erg = await DuplexIo.SendMessage(message);

            // Now log app performance measures (must be located after sending!)
            _appEventSource.ReportMetric(DataMessagingEventSourceProvider.DclSentDataMessageBytes, message.RawMessageData.Length);
            _appEventSource.ReportIncrement(DataMessagingEventSourceProvider.DclSentDataMessageCount);

            return erg;
        }
        catch (Exception e)
        {
            var msg = $"SendMessage failed: {e}";
            _monitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{_loggerId}{msg}");
            throw;
        }
    }

    /// <summary>
    /// This method should check if sending a handshake is required and send it if yes
    /// </summary>
    /// <param name="message">Data message received</param>
    public void OnReceivedMessage(IInboundDataMessage message)
    {
        _inboundQueue.Enqueue(message);
        //_monitorLogger.LogDebug($"{_loggerId}received {message.ToShortInfoString()}");
    }

    /// <summary>
    /// Connect to the tower
    /// </summary>
    public void Connect()
    {
        if (_isInitialized)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(SocketProxy);

        // Connect the socket in sync manner
        AsyncHelper.RunSync(() =>
        {
            try
            {
                return SocketProxy.Connect();
            }
            catch (Exception e)
            {
                var msg = $"connecting failed: {e}";
                _monitorLogger.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{_loggerId}{msg}");
                return Task.CompletedTask;
            }

        });

        // Start the communication in sync manner
        AsyncHelper.RunSync(() =>
        {
            try
            {
                return DuplexIo.StartCommunication();
            }
            catch (Exception e)
            {
                var msg = $"StartCommunication failed: {e}";
                _monitorLogger.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{_loggerId}{msg}");
                return Task.CompletedTask;
            }

        });

        if (!_inboundQueue.IsActivated)
        {
            _inboundQueue.ConsumerTaskDelegate = InboundMessageReachedQueue;
            _inboundQueue.StartConsumer();
        }

        _isInitialized = true;
    }

    private void InboundMessageReachedQueue(IInboundDataMessage message)
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig.DataMessageProcessingPackage);

        //if (message is TncpInboundDataMessage)
        //{
        //    Debug.Print($"TNCP: 0 {message.ToShortInfoString()}");
        //}

        try
        {
            if (_waitStateManager!=null)
            {
                _waitStateManager.LastMessageTimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            }

            // Send only a handshake if the message requires it
            if (message.AnswerWithAcknowledgement)
            {
                var response = DataMessagingConfig.DataMessageProcessingPackage.DataMessageHandshakeFactory.GetAckResponse(message);

                if (response is not DoNotSendOutboundHandshakeMessage)
                {
                    // Fire and forget
                    AsyncHelper.FireAndForget(async void () =>
                    {
                        try
                        {
                            await DuplexIo.SendMessage(response);
                        }
                        catch (Exception e)
                        {
                            var msg = $"DuplexIo.SendMessage: {message.ToShortInfoString()}: Sending failed: {e}";
                            _monitorLogger.LogError(msg);
                            DataMessagingConfig.AppLogger.LogError($"{_loggerId}{msg}");
                        }
                    }); 
                }
            }

            //if (message is TncpInboundDataMessage)
            //{
            //    Debug.Print($"TNCP: 0 {message.ToShortInfoString()}");
            //}

            if (DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate == null)
            {
                //if (message is TncpInboundDataMessage)
                //{
                //    Debug.Print($"TNCP: 1 {message.ToShortInfoString()}");
                //}
                _monitorLogger.LogDebug($"{_loggerId}DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate is null");
            }
            else
            {
                //if (message is TncpInboundDataMessage)
                //{
                //    Debug.Print($"TNCP: 2 {message.ToShortInfoString()}");
                //}
                DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate.Invoke(message);
                //if (message is TncpInboundDataMessage)
                //{
                //    Debug.Print($"TNCP: 3 {message.ToShortInfoString()}");
                //}
            }

            // Now log app performance measures (must be located after sending!)
            _appEventSource.ReportMetric(DataMessagingEventSourceProvider.DclReceivedDataMessageBytes, message.RawMessageData.Length);
            _appEventSource.ReportIncrement(DataMessagingEventSourceProvider.DclReceivedDataMessageCount);
        }
        catch (Exception e)
        {
            var msg = $"{message.ToShortInfoString()} received, but handling failed: {e}";
            _monitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{_loggerId}{msg}");
        }
    }

    /// <summary>
    /// Disconnect from tower
    /// </summary>
    public void Disconnect()
    {
        _inboundQueue.StopConsumer();

        DuplexIo.StopCommunication().Wait(5000);
        SocketProxy?.Close();
        _monitorLogger.LogDebug("disconnecting - Socket has been closed.");
        _isInitialized = false;
    }

    /// <summary>
    /// Is the device connected
    /// </summary>
    public bool IsConnected => SocketProxy is { Connected: true };

    /// <summary>
    /// Current socket proxy
    /// </summary>
    public ISocketProxy? SocketProxy => DuplexIo.DataMessagingConfig.SocketProxy;

    /// <summary>
    /// Dispose the instance
    /// </summary>
    /// <param name="disposing">Is disposing?</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        try
        {
            DuplexIo.DisposeAsync();
        }
        catch
        {
            // Do nothing
        }

        SocketProxy?.Dispose();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}