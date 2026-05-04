// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using Bodoconsult.App.BufferPool;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for packetwise message receiving for IP based networks (UDP/TCP) without timer. One packet will result in one message
/// </summary>
public class UdpDatagramIpDuplexIoReceiver : BaseDuplexIoReceiver
{
    private readonly BufferPool<DummyMemory> _bufferPool = new();
    private ReadOnlySequence<byte> _buffer = new([]);
    private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;
    private readonly ProducerConsumerQueue<DummyMemory> _currentPipeline = new();
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
    /// <param name="deviceCommSettings">Current device comm settings</param>
    /// <param name="duplexIoIsWorkInProgressDelegate">Delegate for checking if the socket is wokring currently</param>
    /// <param name="duplexIoSetNotInProgressDelegate">Delegate to set socket state to no work in progress</param>
    public UdpDatagramIpDuplexIoReceiver(IDataMessagingConfig deviceCommSettings,
        DuplexIoIsWorkInProgressDelegate duplexIoIsWorkInProgressDelegate,
        DuplexIoNoDataDelegate duplexIoSetNotInProgressDelegate) : base(deviceCommSettings)
    {
        DuplexIoIsWorkInProgressDelegate = duplexIoIsWorkInProgressDelegate;
        DuplexIoNoDataDelegate = duplexIoSetNotInProgressDelegate;

        _currentPipeline.ConsumerTaskDelegate = TryToSendReceivedData;
        _bufferPool.LoadFactoryMethod(() => new DummyMemory());
        _bufferPool.Allocate(3);

        ArgumentNullException.ThrowIfNull(deviceCommSettings.DataMessageProcessingPackage);

        _dataMessageValidator = deviceCommSettings.DataMessageProcessingPackage.DataMessageValidator;
    }

    /// <summary>
    /// Activate the received messages logging. Should be turned off in production
    /// </summary>
    public bool ActivateReceiveLogging { get; set; }

    /// <summary>
    /// Maximum buffer size for UDP datagram. Set this value lower if your datagrams do not reach the maximum length of 65536 byte for UDP diagrams defined by protocol specs
    /// </summary>
    public int MaxDatagramSize { get; set; } = 65536;

    /// <summary>
    /// Try to send received data as message to internal receivers
    /// </summary>
    /// <param name="data">Data received</param>
    public void TryToSendReceivedData(DummyMemory data)
    {
        var chunk = new ChunkedSequence<byte>(_buffer);
        chunk.Append(data.Memory);
        _buffer = chunk;

        string msg;

        if (ActivateReceiveLogging)
        {
            msg = $"Data in buffer: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref _buffer)}";
            Trace.TraceInformation($"{LoggerId}{msg}");
            Logger.LogDebug(msg);
        }
        else
        {
            _messageCounter++;

            if (Math.Abs(_messageCounter % 100.0) < 0.1)
            {
                msg = $"Received message {_messageCounter}";
                //Trace.TraceInformation(msg);
                Logger.LogDebug(msg);
                Trace.TraceInformation($"{LoggerId}UdpDatagramIpDuplexIoReceiver: {msg}");
            }

            if (_messageCounter == long.MaxValue)
            {
                _messageCounter = 0;
            }
        }

        if (!DataMessageSplitter.TryReadCommand(ref _buffer, out var command))
        {
            return;
        }

        var length = (int)command.Length;
        if (length == 0)
        {
            return;
        }

        var array = ArrayPool.Rent(length);

        command.CopyTo(array);

        var mem = ((Memory<byte>)array)[..length];

        var codecResult = DataMessageCodingProcessor.DecodeDataMessage(mem);

        if (codecResult.ErrorCode != 0 || codecResult.DataMessage == null)
        {
            msg = $"Parsing command failed with error code {codecResult.ErrorCode}: {codecResult.ErrorMessage}: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
            //Trace.TraceInformation(msg);
            Logger.LogDebug(msg);
            Trace.TraceWarning($"{LoggerId}{msg}");
            ArrayPool.Return(array);
            return;
        }

        var validationResult = _dataMessageValidator.IsMessageValid(codecResult.DataMessage);
        if (!validationResult.IsMessageValid)
        {
            msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)} NOT valid: {validationResult.ValidationResult}";
            //Trace.TraceInformation(msg);
            Logger.LogError(msg);
            Trace.TraceError($"{LoggerId}{msg}");
        }
        else
        {
            //if (ActivateReceiveLogging)
            //{
            //    msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
            //    //Trace.TraceInformation(msg);
            //    DataMessagingConfig.MonitorLogger.LogDebug(msg);
            //}
            DataMessageProcessor.ProcessMessage(codecResult.DataMessage);
        }

        ArrayPool.Return(array);
    }

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    public override async Task StopReceiver()
    {
        if (FillPipelineTask == null && SendPipelineTask == null)
        {
            return;
        }

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

        FillPipelineTask?.Join();

        _currentPipeline.StopConsumer();

        await Task.Run(FillMessagePipeline);

        SendPipelineTask?.Join();

        try
        {
            CancellationSource?.Dispose();
        }
        catch (Exception e)
        {
            Logger.LogError("CancellationToken cancelling failed", e);
        }
        finally
        {
            CancellationSource = null;
        }

        FillPipelineTask = null;
        SendPipelineTask = null;
    }

    /// <summary>
    /// Receive messages from the device.
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    /// <returns>Received device message or null in case of any error</returns>
    public override async Task FillMessagePipeline()
    {
        ArgumentNullException.ThrowIfNull(DuplexIoIsWorkInProgressDelegate);
        ArgumentNullException.ThrowIfNull(DuplexIoNoDataDelegate);
        ArgumentNullException.ThrowIfNull(DataMessagingConfig.SocketProxy);

        //// Wait until the socket is connected
        //if (!await WaitForSocketIsConnected())
        //{
        //    Trace.TraceInformation($"{LoggerId}FillMessagePipeline stopped");
        //    return;
        //}

        DuplexIoNoDataDelegate.Invoke();

        Trace.TraceInformation($"{LoggerId}FillMessagePipeline started");

        try
        {

            while (true)
            {
                var socketProxy = DataMessagingConfig.SocketProxy;

                //Trace.TraceInformation("FillMessagePipeline in progress");
                try
                {
                    if (CancellationSource?.Token.IsCancellationRequested ?? true)
                    {
                        if (socketProxy?.CancellationTokenSource != null)
                        {
                            await socketProxy.CancellationTokenSource.CancelAsync();
                        }
                        Trace.TraceInformation($"{LoggerId}FillMessagePipeline cancelled");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError($"{LoggerId}FillMessagePipeline exception: {e}");
                    return;
                }

                //if (socketProxy is not { Connected: true } || socketProxy.IsDisposed)
                //{
                //    Trace.TraceInformation($"{LoggerId}Not connected");
                //    DuplexIoNoDataDelegate.Invoke();
                //    await RaiseException(new SocketException());
                //    return;
                //}

                if (DuplexIoIsWorkInProgressDelegate.Invoke())
                {
                    Trace.TraceInformation($"{LoggerId}Other operation in progress");
                    AsyncHelper.Delay(FillPipelineTimeout);
                    continue;
                }

                //var availableData = socketProxy.BytesAvailable;
                //if (availableData == 0)
                //{
                //    //Trace.TraceInformation($"No {availableData} bytes");
                //    DuplexIoNoDataDelegate.Invoke();
                //    //Trace.TraceInformation("No data");
                //    AsyncHelper.Delay(FillPipelineTimeout);
                //    continue;
                //}

                //Trace.TraceInformation($"Received {availableData} bytes");

                var data = ArrayPool.Rent(MaxDatagramSize);

                var messageLength = await socketProxy.Receive(data);

                // Give the socket free
                DuplexIoNoDataDelegate.Invoke();

                if (messageLength <= 0)
                {
                    //Trace.TraceInformation($"{LoggerId}No data");
                    AsyncHelper.Delay(FillPipelineTimeout);
                    continue;
                }

                Trace.TraceInformation($"{LoggerId}Got data");

                var dummy = _bufferPool.Dequeue();
                dummy.Memory = data.AsSpan()[..messageLength].ToArray().AsMemory();

                _currentPipeline.Enqueue(dummy);

                ArrayPool.Return(data);
            }
        }
        catch (Exception e)
        {
            Trace.TraceError($"{LoggerId}FillMessagePipeline: {e}");
        }
    }

    private Task RaiseException(Exception ex)
    {
        //await StopReceiver();

        CancellationSource?.Cancel();

        try
        {
            DuplexIoNoDataDelegate?.Invoke();
        }
        catch (Exception e)
        {
            // Do nothing
            DataMessagingConfig.MonitorLogger.LogError("DuplexIoSetNotInProgressDelegate raised an exception", e);
            Trace.TraceError($"{LoggerId}DuplexIoNoDataDelegate?.Invoke: {e}");
        }

        AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(ex));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Process the messages received from device internally
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    public override Task SendMessagePipeline()
    {
        _currentPipeline.StartConsumer();
        return Task.CompletedTask;
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

        await Task.Run(() =>
        {
            FillPipelineTask = null;
        });

        try
        {
            CancellationSource?.Dispose();
        }
        catch (Exception e)
        {
            Logger.LogError("CancellationToken cancelling failed", e);
        }
        finally
        {
            CancellationSource = null;
        }
    }
}