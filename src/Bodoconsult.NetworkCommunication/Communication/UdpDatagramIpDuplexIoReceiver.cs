// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for packetwise message receiving for IP based networks (UDP/TCP) without timer. One packet will result in one message
/// </summary>
public class UdpDatagramIpDuplexIoReceiver : BaseDuplexIoReceiver
{
    private long _messageCounter;
    private int _count;

    /// <summary>
    /// Current validator impl for data messages
    /// </summary>
    private readonly IDataMessageValidator _dataMessageValidator;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="config">Current device comm settings</param>
    public UdpDatagramIpDuplexIoReceiver(IDataMessagingConfig config) : base(config)
    {
        ArgumentNullException.ThrowIfNull(config.SocketProxy);

        if (config.SocketProxy is not IUdpSocketProxy socketProxy)
        {
            throw new ArgumentException("deviceCommSettings.SocketProxy is not IUdpSocketProxy");
        }

        socketProxy.StartReceiverLoop(SocketReceivedData);

        if (socketProxy.ReceiverPipeline is not IDatagramPipeline pipeline)
        {
            throw new ArgumentException("_socketProxy.ReceiverPipeline is not IStreamPipeline");
        }

        socketProxy.StartReceiverLoop(SocketReceivedData);
        pipeline.StartReceiverLoop(SocketReceivedData);

        ArgumentNullException.ThrowIfNull(config.DataMessageProcessingPackage);

        _dataMessageValidator = config.DataMessageProcessingPackage.DataMessageValidator;
    }

    /// <summary>
    /// Handle data the socket has received
    /// </summary>
    /// <param name="data">Received data</param>
    public void SocketReceivedData(Memory<byte> data)
    {
        //if (data.IsEmpty)
        //{
        //    return;
        //}

        //var msg = $"OID: {BitConverter.ToUInt64(data[..8].Span)} ";
        //MonitorLogger.LogInformation(msg);

        //Debug.Print($"Buffer ({data.Length}B): {msgId}");
        //return;

        string msg;

        try
        {
            //#if DEBUG
            //        var msg = $"{LoggerId}received: {data.Length} byte";
            //        MonitorLogger.LogInformation(msg);
            //#endif

            //if (ActivateReceiveLogging)
            //{
            //    msg = $"Buffer ({data.Length}B): {ArrayHelper.GetStringFromArrayCsharpStyle(data, false)}";
            //    MonitorLogger.LogDebug(msg);
            //}
            //else
            //{
            //    if (_messageCounter == long.MaxValue)
            //    {
            //        _messageCounter = -1;
            //    }

            //    _messageCounter++;

            //    if (Math.Abs(_messageCounter % 100.0) < 0.1)
            //    {
            //        msg = $"Received message {_messageCounter}";
            //        //Trace.TraceInformation(msg);
            //        MonitorLogger.LogDebug(msg);
            //    }
            //}

            //Debug.Print($"Before decoding: {msgId}");

            // Decode
            var codecResult = DataMessageCodingProcessor.DecodeDataMessage(data);

            //Debug.Print($"After decoding: {msgId}");

            if (codecResult is { ErrorCode: 0, DataMessage: not null })
            {
                var validationResult = _dataMessageValidator.IsMessageValid(codecResult.DataMessage);
                if (validationResult.IsMessageValid)
                {
                    //if (_count < 15)
                    //{
                    //    //if (ActivateReceiveLogging)
                    //    //{
                    //    //msg = $"Parsed command {codecResult.DataMessage.RawMessageDataClearText}";

                    //    //Debug.Print(ArrayHelper.GetStringFromArrayCsharpStyle(codecResult.DataMessage.RawMessageData));

                    //    msg = $"Parsed {codecResult.DataMessage.ToShortInfoString()}: {codecResult.DataMessage.RawMessageDataClearText}";
                    //    //Trace.TraceInformation(msg);
                    //    DataMessagingConfig.MonitorLogger.LogDebug(msg);
                    //    //}

                    //    if (_count < 15)
                    //    {
                    //        _count++;
                    //    }
                    //}

                    //Debug.Print($"Before processing: {codecResult.DataMessage.ToShortInfoString()}");
                    DataMessageProcessor.AddMessageToQueue(codecResult.DataMessage);

                    //msg = $"{codecResult.DataMessage.ToShortInfoString()}: start processing";
                    //MonitorLogger.LogDebug(msg);
                }
            }
            else if (codecResult.ErrorCode == 6)
            {
                // Do nothing: message already handled
            }
            else
            {
                msg = $"Parsing command failed with error code {codecResult.ErrorCode}: {codecResult.ErrorMessage}: {ArrayHelper.GetStringFromArrayCsharpStyle(data, false)}";
                MonitorLogger.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");
            }
        }
        catch (Exception e)
        {
            msg = $"Parsing command failed: {ArrayHelper.GetStringFromArrayCsharpStyle(data, false)}\r\n{e}";
            MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");
        }
    }

    /// <summary>
    /// Activate the received messages logging. Should be turned off in production
    /// </summary>
    public bool ActivateReceiveLogging { get; set; }

    /// <summary>
    /// Start the internal receiver
    /// </summary>
    public override async Task StartReceiver()
    {
        if (CancellationSource != null)
        {
            try
            {
                await CancellationSource.CancelAsync();
                CancellationSource?.Dispose();
            }
            catch (Exception e)
            {
                var msg = $"CancellationToken cancelling failed: {e}";
                MonitorLogger.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");
            }
        }

        CancellationSource = new();
    }

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    public override async Task StopReceiver()
    {
        try
        {
            if (CancellationSource != null)
            {
                await CancellationSource.CancelAsync();
            }
        }
        catch
        {
            // Do nothing
        }

        try
        {
            CancellationSource?.Dispose();
        }
        catch (Exception e)
        {
            MonitorLogger.LogError("CancellationToken cancelling failed", e);
        }
        finally
        {
            CancellationSource = null;
        }
    }

    /// <summary>
    /// Current implementation of disposing
    /// </summary>
    /// <param name="disposing">True if diposing should run</param>
    protected override async Task Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        await StopReceiver();

        try
        {
            CancellationSource?.Dispose();
        }
        catch (Exception e)
        {
            MonitorLogger.LogError("CancellationToken cancelling failed", e);
        }
        finally
        {
            CancellationSource = null;
        }
    }
}