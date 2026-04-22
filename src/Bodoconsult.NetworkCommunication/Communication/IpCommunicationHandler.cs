// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Abstractions.SyncExecution;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.EventSources;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// IP implementation for <see cref="ICommunicationHandler"/> via socket
/// </summary>
public class IpCommunicationHandler : ICommunicationHandler
{
    private readonly IAppEventSource _appEventSource;
    
    private IWaitStateManager? _waitStateManager;
    private bool _isInitialized;

    private readonly SyncProcessManager<long, MessageSendingResult> _syncProcessManager = new();
    private readonly ProducerConsumerQueue<IOutboundMessage> _queue = new();
    
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

        UpdateDevice();

        DataMessagingConfig.RaiseDataMessageNotSentDelegate = OnMessageNotSent;

        DataMessagingConfig.RaiseCommLayerDataMessageReceivedDelegate = OnReceivedMessage;
        DataMessagingConfig.RaiseUnexpectedDataMessageReceivedDelegate = OnNotExpectedMessageReceivedEvent;
    }

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
    public void OnMessageNotSent(ReadOnlyMemory<byte> message, string? reason)
    {
        DataMessagingConfig.MonitorLogger.LogWarning($"message not sent: '{DataMessageHelper.GetStringFromArrayCsharpStyle(message)}' \n Reason: '{reason}'");
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
    public MessageSendingResult SendMessage(IOutboundDataMessage message)
    {
        try
        {
            // Todo: check if blocking
            var s = $"Enqueue message {message.ToShortInfoString()}";
            //Debug.Print(s);
            DataMessagingConfig.MonitorLogger.LogDebug(s);

            message.RaiseStopSyncExecutionDelegate = StopExecutionOfSyncOrder;

            _queue.Enqueue(message);

            var syncData = _syncProcessManager.AddSyncProcess(message.MessageId, 5000);

            // Now wait for order execution (doing it in a non-blocking mannor)
            var erg = AsyncHelper.RunSync(syncData.CreateWaitingTask);

            // Remove the order from waiting queue
            _syncProcessManager.RemoveSyncProcess(message.MessageId);

            //// Now log app performance measures (must be located after sending!)
            //_appEventSource.ReportMetric(DataMessagingEventSourceProvider.DclSentDataMessageBytes, message.RawMessageData.Length);
            //_appEventSource.ReportIncrement(DataMessagingEventSourceProvider.DclSentDataMessageCount);

            return erg ?? new MessageSendingResult(message, OrderExecutionResultState.Error);
            //return x;
        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
            throw;
        }
    }

    private void StopExecutionOfSyncOrder(MessageSendingResult result)
    {
        var syncData = _syncProcessManager.GetSyncProcessDataForProcess(result.Message?.MessageId ?? 0);

        if (syncData == null)
        {
            return;
        }

        syncData.TaskCompletionSource?.SetResult(result);
    }

    /// <summary>
    /// This method should check if sending a handshake is required and send it if yes
    /// </summary>
    /// <param name="message">Data message received</param>
    public void OnReceivedMessage(IInboundDataMessage message)
    {
        Trace.TraceInformation($"IpCommunicationHandler: received message {message.MessageId}: {message.RawMessageData.Length} bytes");

        ArgumentNullException.ThrowIfNull(DataMessagingConfig.DataMessageProcessingPackage);
        ArgumentNullException.ThrowIfNull(_waitStateManager);

        try
        {
            _waitStateManager.LastMessageTimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            // Send only a handshake if the message requires it
            if (message.AnswerWithAcknowledgement)
            {
                var response = DataMessagingConfig.DataMessageProcessingPackage.DataMessageHandshakeFactory.GetAckResponse(message);

                if (response is not DoNotSendOutboundHandshakeMessage)
                {
                    // Fire and forget
                    _queue.Enqueue(response);
                }
            }

            if (DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate==null)
            {
                Trace.TraceWarning("IpCommunicationHandler: DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate is null");
            }
            else
            {
                AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate.Invoke(message));
            }
            
            // Now log app performance measures (must be located after sending!)
            _appEventSource.ReportMetric(DataMessagingEventSourceProvider.DclReceivedDataMessageBytes, message.RawMessageData.Length);
            _appEventSource.ReportIncrement(DataMessagingEventSourceProvider.DclReceivedDataMessageCount);
        }
        catch (Exception e)
        {
            DataMessagingConfig.MonitorLogger.LogError($"Received tower message {message.MessageId} but handling failed", e);
        }
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
        AsyncHelper.RunSync(() => SocketProxy?.Connect());

        // Start the communication in sync manner
        AsyncHelper.RunSync(() => DuplexIo.StartCommunication());

        _queue.ConsumerTaskDelegate = ConsumerTaskDelegate;
        _queue.StartConsumer();

        _isInitialized = true;
    }

    private async void ConsumerTaskDelegate(IOutboundMessage message)
    {
        try
        {
            // Todo: check if blocking
            var s = $"Send message {message.ToShortInfoString()}";
            Debug.Print(s);
            DataMessagingConfig.MonitorLogger.LogDebug(s);

            var erg = await DuplexIo.SendMessage(message);

            //// Now log app performance measures (must be located after sending!)
            //_appEventSource.ReportMetric(DataMessagingEventSourceProvider.DclSentDataMessageBytes, message.RawMessageData.Length);
            //_appEventSource.ReportIncrement(DataMessagingEventSourceProvider.DclSentDataMessageCount);

            message.RaiseStopSyncExecutionDelegate?.Invoke(erg);

            //return x;
        }
        catch (Exception e)
        {
            message.RaiseStopSyncExecutionDelegate?.Invoke(
                new MessageSendingResult(message, OrderExecutionResultState.Error)
                {
                    Information = e.ToString()
                });
        }
    }

    /// <summary>
    /// Disconnect from tower
    /// </summary>
    public void Disconnect()
    {
        _queue.StopConsumer();

        DuplexIo.StopCommunication().Wait(2000);
        SocketProxy?.Close();
        DataMessagingConfig.MonitorLogger.LogDebug($"{DataMessagingConfig.LoggerId}disconnect - Socket has been closed.");
        _isInitialized = false;
    }

    ///// <summary>
    ///// Send a message to the tower.
    ///// </summary>
    ///// <param name="message">Current message to send</param>
    //public MessageSendingResult SendMessage(IOutboundMessage message)
    //{
    //    var s = $"Send message {message.ToShortInfoString()}";
    //    Debug.Print(s);
    //    DataMessagingConfig.MonitorLogger.LogDebug(s);

    //    var result = DuplexIo.SendMessage(message);
    //    result.Wait(2000);

    //    // Now log app performance measures (must be located after sending!)
    //    _appEventSource.ReportMetric(DataMessagingEventSourceProvider.DclSentDataMessageBytes, message.RawMessageData.Length);
    //    _appEventSource.ReportIncrement(DataMessagingEventSourceProvider.DclSentDataMessageCount);

    //    return result.Result;
    //}

    /// <summary>
    /// Is the device connected
    /// </summary>
    public bool IsConnected => SocketProxy is { Connected: true };

    /// <summary>
    /// Current socket proxy
    /// </summary>
    public ISocketProxy? SocketProxy => DuplexIo.DataMessagingConfig.SocketProxy;

    ///// <summary>
    ///// Read some bytes
    ///// </summary>
    ///// <param name="buffer">buffer to fill</param>
    ///// <param name="offset">offset</param>
    ///// <param name="expectedBytesLength">Expected length of the byte array</param>

    //public async Task Receive(byte[] buffer, int offset, int expectedBytesLength)
    //{
    //    await SocketProxy.Receive(buffer, offset, expectedBytesLength);
    //}

    ///// <summary>
    ///// Read a single byte
    ///// </summary>
    ///// <returns>Byte as int value</returns>
    //public int ReadByte()
    //{
    //    var onebyte = new byte[1];
    //    AsyncHelper.RunSync(() => SocketProxy.Receive(onebyte));
    //    return onebyte[0];
    //}


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