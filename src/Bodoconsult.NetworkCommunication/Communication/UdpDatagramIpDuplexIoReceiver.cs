// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for packetwise message receiving for IP based networks (UDP/TCP) without timer. One packet will result in one message
/// </summary>
public class UdpDatagramIpDuplexIoReceiver : BaseDuplexIoReceiver
{
    private long _messageCounter;

    /// <summary>
    /// Current validator impl for data messages
    /// </summary>
    private readonly IDataMessageValidator _dataMessageValidator;

    /// <summary>
    /// Fill pipeline timeout in ms from check to check
    /// </summary>
    public static int FillPipelineTimeout { get; set; } = 5;

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
        if (data.IsEmpty)
        {
            return;
        }

        string msg;

        //#if DEBUG
        //        var msg = $"{LoggerId}received: {data.Length} byte";
        //        MonitorLogger.LogInformation(msg);
        //#endif

        if (ActivateReceiveLogging)
        {
            msg = $"Data in buffer: {DataMessageHelper.GetStringFromArrayCsharpStyle(data)}";
            MonitorLogger.LogDebug(msg);
        }
        else
        {
            if (_messageCounter == long.MaxValue)
            {
                _messageCounter = -1;
            }
            _messageCounter++;

            if (Math.Abs(_messageCounter % 100.0) < 0.1)
            {
                msg = $"Received message {_messageCounter}";
                //Trace.TraceInformation(msg);
                MonitorLogger.LogDebug(msg);
            }
        }

        var codecResult = DataMessageCodingProcessor.DecodeDataMessage(data);

        if (codecResult.ErrorCode != 0 || codecResult.DataMessage == null)
        {
            msg = $"Parsing command failed with error code {codecResult.ErrorCode}: {codecResult.ErrorMessage}: {ArrayHelper.GetStringFromArrayCsharpStyle(data, false)}";
            MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");
            return;
        }

        var validationResult = _dataMessageValidator.IsMessageValid(codecResult.DataMessage);
        if (!validationResult.IsMessageValid)
        {
            msg = $"Parsed command {ArrayHelper.GetStringFromArrayCsharpStyle(data, false)} NOT valid: {validationResult.ValidationResult}";
            MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");
        }
        else
        {
            //if (ActivateReceiveLogging)
            //{
                msg = $"Parsed command {codecResult.DataMessage.RawMessageDataClearText}";
                //Trace.TraceInformation(msg);
                DataMessagingConfig.MonitorLogger.LogDebug(msg);
            //}
            DataMessageProcessor.ProcessMessage(codecResult.DataMessage);
        }

    }

    /// <summary>
    /// Activate the received messages logging. Should be turned off in production
    /// </summary>
    public bool ActivateReceiveLogging { get; set; }

    ///// <summary>
    ///// Try to send received data as message to internal receivers
    ///// </summary>
    ///// <param name="data">Data received</param>
    //public void TryToSendReceivedData(DummyMemory data)
    //{
    //    var chunk = new ChunkedSequence<byte>(_buffer);
    //    chunk.Append(data.Memory);

    //    data.Reset();
    //    _bufferPool.Enqueue(data);

    //    _buffer = chunk;

    //    string msg;

    //    if (ActivateReceiveLogging)
    //    {
    //        msg = $"Data in buffer: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref _buffer)}";
    //        MonitorLogger.LogDebug(msg);
    //    }
    //    else
    //    {
    //        _messageCounter++;

    //        if (Math.Abs(_messageCounter % 100.0) < 0.1)
    //        {
    //            msg = $"Received message {_messageCounter}";
    //            //Trace.TraceInformation(msg);
    //            MonitorLogger.LogDebug(msg);
    //        }

    //        if (_messageCounter == long.MaxValue)
    //        {
    //            _messageCounter = 0;
    //        }
    //    }

    //    if (!DataMessageSplitter.TryReadCommand(ref _buffer, out var command))
    //    {
    //        return;
    //    }

    //    var length = (int)command.Length;
    //    if (length == 0)
    //    {
    //        return;
    //    }

    //    var array = command.ToArray();

    //    var mem = ((Memory<byte>)array)[..length];

    //    var codecResult = DataMessageCodingProcessor.DecodeDataMessage(mem);

    //    if (codecResult.ErrorCode != 0 || codecResult.DataMessage == null)
    //    {
    //        msg = $"Parsing command failed with error code {codecResult.ErrorCode}: {codecResult.ErrorMessage}: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
    //        MonitorLogger.LogError(msg);
    //        DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");
    //        return;
    //    }

    //    var validationResult = _dataMessageValidator.IsMessageValid(codecResult.DataMessage);
    //    if (!validationResult.IsMessageValid)
    //    {
    //        msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)} NOT valid: {validationResult.ValidationResult}";
    //        MonitorLogger.LogError(msg);
    //        DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");
    //    }
    //    else
    //    {
    //        //if (ActivateReceiveLogging)
    //        //{
    //        //    msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
    //        //    //Trace.TraceInformation(msg);
    //        //    DataMessagingConfig.MonitorLogger.LogDebug(msg);
    //        //}
    //        DataMessageProcessor.ProcessMessage(codecResult.DataMessage);
    //    }
    //}

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
        //if (FillPipelineTask == null && SendPipelineTask == null)
        //{
        //    return;
        //}

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

        //FillPipelineTask?.Join();

        //_currentPipeline.StopConsumer();

        //await Task.Run(FillMessagePipeline);

        //SendPipelineTask?.Join();

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

        //FillPipelineTask = null;
        //SendPipelineTask = null;
    }

    ///// <summary>
    ///// Receive messages from the device.
    ///// This method is not intended to be called directly from production code.
    ///// It is a unit test method.
    ///// </summary>
    ///// <returns>Received device message or null in case of any error</returns>
    //public override async Task FillMessagePipeline()
    //{
    //    ArgumentNullException.ThrowIfNull(DuplexIoIsWorkInProgressDelegate);
    //    ArgumentNullException.ThrowIfNull(DuplexIoNoDataDelegate);
    //    ArgumentNullException.ThrowIfNull(DataMessagingConfig.SocketProxy);

    //    //// Wait until the socket is connected
    //    //if (!await WaitForSocketIsConnected())
    //    //{
    //    //    Trace.TraceInformation($"{LoggerId}FillMessagePipeline stopped");
    //    //    return;
    //    //}

    //    DuplexIoNoDataDelegate.Invoke();

    //    MonitorLogger.LogInformation($"{LoggerId}FillMessagePipeline started");

    //    try
    //    {

    //        while (true)
    //        {
    //            var socketProxy = DataMessagingConfig.SocketProxy;

    //            //Trace.TraceInformation("FillMessagePipeline in progress");
    //            try
    //            {
    //                if (CancellationSource?.Token.IsCancellationRequested ?? true)
    //                {
    //                    if (socketProxy?.CancellationTokenSource != null)
    //                    {
    //                        await socketProxy.CancellationTokenSource.CancelAsync();
    //                    }
    //                    MonitorLogger.LogInformation($"{LoggerId}FillMessagePipeline cancelled");
    //                    return;
    //                }
    //            }
    //            catch (Exception e)
    //            {
    //                MonitorLogger.LogError($"{LoggerId}FillMessagePipeline exception: {e}");
    //                return;
    //            }

    //            //if (socketProxy is not { Connected: true } || socketProxy.IsDisposed)
    //            //{
    //            //    Trace.TraceInformation($"{LoggerId}Not connected");
    //            //    DuplexIoNoDataDelegate.Invoke();
    //            //    await RaiseException(new SocketException());
    //            //    return;
    //            //}

    //            if (DuplexIoIsWorkInProgressDelegate.Invoke())
    //            {
    //                MonitorLogger.LogDebug($"{LoggerId}Other operation in progress");
    //                AsyncHelper.Delay(FillPipelineTimeout);
    //                continue;
    //            }

    //            //var availableData = socketProxy.BytesAvailable;
    //            //if (availableData == 0)
    //            //{
    //            //    //Trace.TraceInformation($"No {availableData} bytes");
    //            //    DuplexIoNoDataDelegate.Invoke();
    //            //    //Trace.TraceInformation("No data");
    //            //    AsyncHelper.Delay(FillPipelineTimeout);
    //            //    continue;
    //            //}

    //            //Trace.TraceInformation($"Received {availableData} bytes");

    //            var data = ArrayPool.Rent(MaxDatagramSize);

    //            var messageLength = await socketProxy.Receive(data);

    //            // Give the socket free
    //            DuplexIoNoDataDelegate.Invoke();

    //            if (messageLength <= 0)
    //            {
    //                //Trace.TraceInformation($"{LoggerId}No data");
    //                AsyncHelper.Delay(FillPipelineTimeout);
    //                continue;
    //            }

    //            //Trace.TraceInformation($"{LoggerId}Got data");

    //            var dummy = _bufferPool.Dequeue();
    //            dummy.Memory = data.AsSpan()[..messageLength].ToArray().AsMemory();

    //            _currentPipeline.Enqueue(dummy);

    //            ArrayPool.Return(data);
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        MonitorLogger.LogError($"{LoggerId}FillMessagePipeline: {e}");
    //    }
    //}

    //private Task RaiseException(Exception ex)
    //{
    //    //await StopReceiver();

    //    CancellationSource?.Cancel();

    //    try
    //    {
    //        DuplexIoNoDataDelegate?.Invoke();
    //    }
    //    catch (Exception e)
    //    {
    //        // Do nothing
    //        DataMessagingConfig.MonitorLogger.LogError("DuplexIoSetNotInProgressDelegate raised an exception", e);
    //        Trace.TraceError($"{LoggerId}DuplexIoNoDataDelegate?.Invoke: {e}");
    //    }

    //    AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(ex));

    //    return Task.CompletedTask;
    //}

    ///// <summary>
    ///// Process the messages received from device internally
    ///// This method is not intended to be called directly from production code.
    ///// It is a unit test method.
    ///// </summary>
    //public override Task SendMessagePipeline()
    //{
    //    _currentPipeline.StartConsumer();
    //    return Task.CompletedTask;
    //}

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