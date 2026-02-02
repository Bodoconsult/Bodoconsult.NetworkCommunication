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

    private CancellationTokenSource _cancellationSource;

    private readonly BufferPool<DummyMemory> _bufferPool = new();

    // Do not remove
    private Thread _fillPipelineTask;

    private ReadOnlySequence<byte> _buffer = new([]);

    private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;


    private readonly ProducerConsumerQueue<DummyMemory> _currentPipeline = new();

    //public static int SendTimeout = 5;

    public static int FillPipelineTimeout = 5;


    private readonly DuplexIoIsWorkInProgressDelegate _duplexIoIsWorkInProgressDelegate;
    private readonly DuplexIoSetNotInProgressDelegate _duplexIoSetNotInProgressDelegate;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="deviceCommSettings">Current device comm settings</param>
    /// <param name="duplexIoIsWorkInProgressDelegate">Delegate for checking if the socket is wokring currently</param>
    /// <param name="duplexIoSetNotInProgressDelegate">Delegate to set socket state to no work in progress</param>
    public IpDuplexIoReceiver(IDataMessagingConfig deviceCommSettings,
        DuplexIoIsWorkInProgressDelegate duplexIoIsWorkInProgressDelegate,
        DuplexIoSetNotInProgressDelegate duplexIoSetNotInProgressDelegate) : base(deviceCommSettings)
    {
        _duplexIoIsWorkInProgressDelegate = duplexIoIsWorkInProgressDelegate;
        _duplexIoSetNotInProgressDelegate = duplexIoSetNotInProgressDelegate;

        _currentPipeline.ConsumerTaskDelegate  = TryToSendReceivedData;
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
        DataMessagingConfig.MonitorLogger?.LogDebug(s);

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
                DataMessagingConfig.MonitorLogger?.LogDebug(msg);
            }
            else
            {
                var validationResult = DataMessagingConfig.DataMessageProcessingPackage.DataMessageValidator.IsMessageValid(codecResult.DataMessage);
                if (!validationResult.IsMessageValid)
                {
                    msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)} NOT valid: {validationResult.ValidationResult}";
                    Debug.Print(msg);
                    DataMessagingConfig.MonitorLogger?.LogDebug(msg);
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
    /// Start the internal receiver
    /// </summary>
    public override async Task StartReceiver()
    {
        if (_cancellationSource != null)
        {
            try
            {
                _cancellationSource.Cancel();
                _cancellationSource?.Dispose();
            }
            catch (Exception e)
            {
                DataMessagingConfig.MonitorLogger.LogError("CancellationToken cancelling failed", e);
            }
        }

        _cancellationSource = new();

        _currentPipeline.StartConsumer();

        await Task.Run(() =>
        {
            _fillPipelineTask = new Thread(StartFillMessagePipeline)
            {
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true
            };
            _fillPipelineTask.Start();
        });

    }

    public void StartFillMessagePipeline()
    {
        Debug.Print("StartFillMessagePipeline in progress");

        try
        {

            //while (!_cancellationSource.Token.IsCancellationRequested)
            //{

            //    if (!DataMessagingConfig.SocketProxy.Connected)
            //    {
            //        AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(new SocketException()));
            //        break;
            //    }

            var task = Task.Run(FillMessagePipeline);
            task.Wait();
            task.Dispose();

            //AsyncHelper.Delay(FillPipelineTimeout);

            //}

        }
        catch (Exception exception)
        {
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
        }
    }

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    public override async Task StopReceiver()
    {
        try
        {
            _cancellationSource?.Cancel();
        }
        catch
        {
            // Do nothing
        }

        _fillPipelineTask?.Join();

        _currentPipeline.StopConsumer();

        await Task.Run(FillMessagePipeline);

        try
        {
            _cancellationSource?.Dispose();
        }
        catch (Exception e)
        {
            DataMessagingConfig.MonitorLogger.LogError("CancellationToken cancelling failed", e);
        }
    }



    public override async Task FillMessagePipeline()
    {
        //try
        //{
                
        while (true)
        {

            // Debug.Print("FillMessagePipeline in progress");
            try
            {
                if (_cancellationSource.Token.IsCancellationRequested)
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

            if (_duplexIoIsWorkInProgressDelegate())
            {
                //Debug.Print("Other operation in progress");
                AsyncHelper.Delay(FillPipelineTimeout);
                continue;
            }

            var availableData = socketProxy.BytesAvailable;
            if (availableData == 0)
            {
                _duplexIoSetNotInProgressDelegate();
                //Debug.Print("No data");
                AsyncHelper.Delay(FillPipelineTimeout);
                continue;
            }

            var data = new byte[availableData].AsMemory();

            var messageLength = await socketProxy.Receive(data);

            // Give the socket free
            _duplexIoSetNotInProgressDelegate();

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

        _cancellationSource.Cancel();

        try
        {
            _duplexIoSetNotInProgressDelegate();
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
        // Do nothing
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
            _fillPipelineTask = null;
        });

        _cancellationSource?.Dispose();
        _cancellationSource = null;
    }


}