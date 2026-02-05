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


    //public static int SendTimeout = 5;

    public static int FillPipelineTimeout = 5;

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
    }

    public void TryToSendReceivedData(DummyMemory data)
    {
        var chunk = new ChunkedSequence<byte>(_buffer);
        chunk.Append(data.Memory);

        _bufferPool.Enqueue(data);

        _buffer = chunk;

        var s = $"Data in buffer: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref _buffer)}";
        Debug.Print(s);
        Logger?.LogDebug(s);

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

            string msg;

            var codecResult = DataMessageCodingProcessor.DecodeDataMessage(mem);

            if (codecResult.ErrorCode != 0)
            {
                msg = $"Parsing command failed with error code {codecResult.ErrorCode}: {codecResult.ErrorMessage}: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
                Debug.Print(msg);
                Logger?.LogDebug(msg);
            }
            else
            {
                var validationResult = DataMessagingConfig.DataMessageProcessingPackage.DataMessageValidator.IsMessageValid(codecResult.DataMessage);
                if (!validationResult.IsMessageValid)
                {
                    msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)} NOT valid: {validationResult.ValidationResult}";
                    Debug.Print(msg);
                    Logger?.LogDebug(msg);
                }
                else
                {
                    msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
                    Debug.Print(msg);
                    DataMessagingConfig.MonitorLogger?.LogDebug(msg);

                    DataMessageProcessor.ProcessMessage(codecResult.DataMessage);
                }
            }

            ArrayPool.Return(array);
        }
    }

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    public override async Task StopReceiver()
    {
        if (FillPipelineTask == null && SendPipelineTask==null)
        {
            return;
        }

        try
        {
            CancellationSource?.Cancel();
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
        //try
        //{

        while (true)
        {

            // Debug.Print("FillMessagePipeline in progress");
            try
            {
                if (CancellationSource.Token.IsCancellationRequested)
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            var socketProxy = DataMessagingConfig.SocketProxy;

            if (!socketProxy.Connected || socketProxy.IsDisposed)
            {
                // Debug.Print("Not connected");
                await RaiseException(new SocketException());
                return;
            }

            if (DuplexIoIsWorkInProgressDelegate())
            {
                //Debug.Print("Other operation in progress");
                AsyncHelper.Delay(FillPipelineTimeout);
                continue;
            }

            var availableData = socketProxy.BytesAvailable;
            if (availableData == 0)
            {
                DuplexIoNoDataDelegate();
                //Debug.Print("No data");
                AsyncHelper.Delay(FillPipelineTimeout);
                continue;
            }

            var data = new byte[availableData].AsMemory();

            var messageLength = await socketProxy.Receive(data);

            // Give the socket free
            DuplexIoNoDataDelegate();

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

        CancellationSource.Cancel();

        try
        {
            DuplexIoNoDataDelegate();
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
