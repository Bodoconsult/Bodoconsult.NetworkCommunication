// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.EventSources;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// IP implementation for <see cref="ICommunicationHandler"/> via socket
/// </summary>
public class IpCommunicationHandler : ICommunicationHandler
{
    private readonly IAppEventSource _appEventSource;
    private IDataMessagingConfig DataMessagingConfig { get; }
    private IWaitStateManager _waitStateManager;

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
    public void OnMessageNotSent(ReadOnlyMemory<byte> message, string reason)
    {
        DataMessagingConfig.MonitorLogger.LogWarning($"message not sent: '{DataMessageHelper.GetStringFromArrayCsharpStyle(message)}' \n Reason: '{reason}'");
    }

    private void OnNotExpectedMessageReceivedEvent(IInboundDataMessage e)
    {
        _waitStateManager.LastMessageTimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public MessageSendingResult SendMessage(IOutboundMessage message)
    {
        // Todo: check if blocking
        var s = $"Send message {message.ToShortInfoString()}";
        Debug.Print(s);
        DataMessagingConfig.MonitorLogger.LogDebug(s);

        var x = DuplexIo.SendMessage(message).GetAwaiter().GetResult();

        // Now log app performance measures (must be located after sending!)
        _appEventSource.ReportMetric(DataMessagingEventSourceProvider.DclSentDataMessageBytes, message.RawMessageData.Length);
        _appEventSource.ReportIncrement(DataMessagingEventSourceProvider.DclSentDataMessageCount);

        return x;
    }

    /// <summary>
    /// This method should check if sending a handshake is required and send it if yes
    /// </summary>
    /// <param name="message">Data message received</param>
    public void OnReceivedMessage(IInboundDataMessage message)
    {
        try
        {
            _waitStateManager.LastMessageTimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

            // Send only a handshake if the message requires it
            if (message.AnswerWithAcknowledgement)
            {
                var response = DataMessagingConfig.DataMessageProcessingPackage.DataMessageHandshakeFactory.GetAckResponse(message);

                // Fire and forget
                SendMessage(response);
            }
            
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate?.Invoke(message));

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
        if (IsConnected)
        {
            return;
        }

        // Connect the socket
        AsyncHelper.RunSync(() => SocketProxy.Connect());

        AsyncHelper.RunSync(() => DuplexIo.StartCommunication());
    }

    /// <summary>
    /// Disconnect from tower
    /// </summary>
    public void Disconnect()
    {
        DuplexIo.StopCommunication();
        SocketProxy.Shutdown();
        SocketProxy.Close();
        DataMessagingConfig.MonitorLogger.LogDebug($"{DataMessagingConfig.LoggerId}disconnect - Socket has been closed.");
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
    /// Is the tower connected
    /// </summary>
    public bool IsConnected => SocketProxy is { Connected: true };

    public ISocketProxy SocketProxy => DuplexIo.DataMessagingConfig.SocketProxy;


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



    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        try
        {
            DuplexIo?.DisposeAsync();
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