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
/// Comm adapter subsystem for message receiving for IP based networks (UDP/TCP) without timer
/// </summary>
public class IpDuplexIoReceiver : BaseDuplexIoReceiver
{
    private readonly BufferPool<DummyMemory> _bufferPool = new();
    private ReadOnlySequence<byte> _buffer = new([]);
    private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;
    private readonly ProducerConsumerQueue<DummyMemory> _currentPipeline = new();

    /// <summary>
    /// Current validator impl for data messages
    /// </summary>
    private readonly IDataMessageValidator _dataMessageValidator;


    //public static int SendTimeout = 5;

    public static int FillPipelineTimeout { get; set; } = 5;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="deviceCommSettings">Current device comm settings</param>
    /// <param name="duplexIoIsWorkInProgressDelegate">Delegate for checking if the socket is wokring currently</param>
    /// <param name="duplexIoSetNotInProgressDelegate">Delegate to set socket state to no work in progress</param>
    public IpDuplexIoReceiver(IDataMessagingConfig deviceCommSettings,
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

    public void TryToSendReceivedData(DummyMemory data)
    {
        var chunk = new ChunkedSequence<byte>(_buffer);
        chunk.Append(data.Memory);

        _bufferPool.Enqueue(data);

        _buffer = chunk;

        var msg = $"Data in buffer: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref _buffer)}";
        //Debug.Print(msg);
        Logger.LogDebug(msg);

        while (DataMessageSplitter.TryReadCommand(ref _buffer, out var command))
        {

            var length = (int)command.Length;
            if (length == 0)
            {
                continue;
            }

            // Take a copy of the command to avoid errors if the pipeline socket is closed before processing the command
            var array = ArrayPool.Rent(length);

            command.CopyTo(array);

            var mem = ((Memory<byte>)array)[..length];

            var codecResult = DataMessageCodingProcessor.DecodeDataMessage(mem);

            if (codecResult.ErrorCode != 0 || codecResult.DataMessage == null)
            {
                msg = $"Parsing command failed with error code {codecResult.ErrorCode}: {codecResult.ErrorMessage}: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
                //Debug.Print(msg);
                Logger?.LogDebug(msg);
                ArrayPool.Return(array);
                return;
            }

            var validationResult = _dataMessageValidator.IsMessageValid(codecResult.DataMessage);
            if (!validationResult.IsMessageValid)
            {
                msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)} NOT valid: {validationResult.ValidationResult}";
                //Debug.Print(msg);
                Logger?.LogDebug(msg);
            }
            else
            {
                msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
                //Debug.Print(msg);
                DataMessagingConfig.MonitorLogger?.LogDebug(msg);

                DataMessageProcessor.ProcessMessage(codecResult.DataMessage);
            }

            ArrayPool.Return(array);
        }
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

        // Wait until the socket is connected
        if (!await WaitForSocketIsConnected())
        {
            return;
        }

        Trace.TraceInformation("FillMessagePipeline started");

        //try
        //{

        while (true)
        {
            var socketProxy = DataMessagingConfig.SocketProxy;

            //Debug.Print("FillMessagePipeline in progress");
            try
            {
                if (CancellationSource?.Token.IsCancellationRequested ?? true)
                {
                    if (socketProxy?.CancellationTokenSource != null)
                    {
                        await socketProxy.CancellationTokenSource.CancelAsync();
                    }
                    //Debug.Print("FillMessagePipeline cancelled");
                    return;
                }
            }
            catch (Exception e)
            {
                Trace.TraceError($"FillMessagePipeline exception: {e}");
                return;
            }

            if (socketProxy is not { Connected: true } || socketProxy.IsDisposed)
            {
                // Debug.Print("Not connected");
                DuplexIoNoDataDelegate.Invoke();
                await RaiseException(new SocketException());
                return;
            }

            if (DuplexIoIsWorkInProgressDelegate.Invoke())
            {
                Trace.TraceWarning("Other operation in progress");
                AsyncHelper.Delay(FillPipelineTimeout);
                continue;
            }

            var availableData = socketProxy.BytesAvailable;
            if (availableData == 0)
            {
                DuplexIoNoDataDelegate.Invoke();
                //Debug.Print("No data");
                AsyncHelper.Delay(FillPipelineTimeout);
                continue;
            }

            var data = new byte[availableData].AsMemory();

            var messageLength = await socketProxy.Receive(data);

            // Give the socket free
            DuplexIoNoDataDelegate.Invoke();

            if (messageLength <= 0)
            {
                AsyncHelper.Delay(FillPipelineTimeout);
                continue;
            }

            //Debug.Print("Got data");

            var dummy = _bufferPool.Dequeue();
            dummy.Memory = data;

            _currentPipeline.Enqueue(dummy);
        }
        //}
        //catch (Exception e)
        //{
        //    Console.WriteLine(e);
        //    throw;
        //}
    }


    private Task RaiseException(Exception ex)
    {
        //await StopReceiver();

        CancellationSource?.Cancel();
        Trace.TraceError(ex.ToString());

        try
        {
            DuplexIoNoDataDelegate?.Invoke();
        }
        catch (Exception e)
        {
            // Do nothing
            DataMessagingConfig.MonitorLogger.LogError("DuplexIoSetNotInProgressDelegate raised an exception", e);
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

        CancellationSource?.Dispose();
        CancellationSource = null;
    }
}