// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for message receiving base on high-performance pipeline implementation
/// </summary>
public class IpHighPerformanceDuplexIoReceiver : BaseDuplexIoReceiver
{
    private readonly Pipe _pipe;
    private bool _isDone;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="pipe">Current pipe</param>
    /// <param name="config">Current config</param>
    /// <param name="pollingTimeOut">Polling timeout in seconds</param>
    public IpHighPerformanceDuplexIoReceiver(Pipe pipe, IDataMessagingConfig config, int pollingTimeOut) : base(config)
    {
        _pipe = pipe;
        PollingTimeOut = pollingTimeOut;
    }

    private bool IsCompleted()
    {
        return FillPipelineTask is not { IsAlive: true };
    }

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    public override async Task StopReceiver()
    {
        _isDone = true;

        await Task.Run(() =>
        {
            try
            {
                Debug.Print("Wait for completion");

                if (FillPipelineTask == null)
                {
                    return;
                }

                Wait.Until(IsCompleted, 1000);

                FillPipelineTask = null;
                SendPipelineTask = null;

                Debug.Print("Completed");
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                Logger?.LogError(e, "Stopping receiver failed");
            }
        });
    }

    public override async Task FillMessagePipeline()
    {
        //Debug.Print("Start fill message pipeline");
        var writer = _pipe.Writer;
        var memSize = DataMessagingConfig.SocketProxy.MinimumBufferSize;

        while (!_isDone)
        {
            // Allocate at least 512 bytes from the PipeWriter.
            try
            {
                if (!DataMessagingConfig.SocketProxy.Connected)
                {
                    AsyncHelper.Delay(5);
                    continue;
                }

                Debug.Print($"{_isDone}");
                if (_isDone)
                {
                    break;
                }

                var memory = writer.GetMemory(memSize);


                var bytesRead = await DataMessagingConfig.SocketProxy.Receive(memory);
                //Debug.Print($"Socket bytes read: {bytesRead}");

                if (bytesRead > 0)
                {
                    // Tell the PipeWriter how much was read from the Socket.
                    writer.Advance(bytesRead);
                }

                // Make the data available to the PipeReader.
                var result = await writer.FlushAsync();
                if (result.IsCompleted 
                    || result.IsCanceled
                    || _isDone)
                {
                    break;
                }
            }
            catch (SocketException socketException)
            {
                Logger?.LogError("filling pipe failed", socketException);
                _isDone = true;
                break;
            }
            catch (Exception otherException)
            {
                Logger?.LogError("filling pipe failed", otherException);
            }
        }

        // By completing PipeWriter, tell the PipeReader that there's no more data coming.
        await writer.CompleteAsync();

        //Debug.Print("Completed fill message pipeline");
    }



    /// <summary>
    /// Process the messages received from device internally
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    public override async Task SendMessagePipeline()
    {
        var reader = _pipe.Reader;
        //Debug.Print("Start send message pipeline");
        while (!_isDone)
        {
            var result = await reader.ReadAsync();
            var buffer = result.Buffer;

            // Stop reading if there's no more data coming.
            if (buffer.IsEmpty && (result.IsCompleted || _isDone))
            {
                break;
            }

            if (buffer.IsEmpty)
            {
                continue;
            }

            //Debug.Print($"Raw command: {ArrayHelper.GetStringFromArrayCsharpStyle(ref buffer)}");
            Logger?.LogInformation($"Raw command: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref buffer)}");

            //Debug.Print($"Buffer: pre-length: {buffer.Length}");

            // In the event that no message is parsed successfully, mark consumed
            // as nothing and examined as the entire buffer.

            while (DataMessageSplitter.TryReadCommand(ref buffer, out var command))
            {

                var length = (int)command.Length;
                if (length == 0)
                {
                    continue;
                }

                var mem = new Memory<byte>(command.ToArray());

                //var array = ArrayPool.Rent(length);

                //command.CopyTo(array);

                //var mem = ((Memory<byte>)array)[..length];

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
                        msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)} NOT valid: {validationResult.ValidationResult}. Message was NOT processed.";
                        Debug.Print(msg);
                        Logger?.LogDebug(msg);
                    }
                    else
                    {
                        msg = $"Parsed command {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
                        Debug.Print(msg);
                        Logger?.LogDebug(msg);

                        DataMessageProcessor.ProcessMessage(codecResult.DataMessage);
                    }
                }

                //ArrayPool.Return(array);

            }

            // Tell the PipeReader how much of the buffer has been consumed.
            reader.AdvanceTo(buffer.Start, buffer.End);

            //Debug.Print($"Buffer: post-length: {buffer.Length}");


            // Stop reading if there's no more data coming.
            if (result.Buffer.IsEmpty && result.IsCompleted)
            {
                break;
            }
        }

        // Mark the PipeReader as complete.
        await reader.CompleteAsync();

        //Debug.Print("Completed send message pipeline");
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
        FillPipelineTask = null;
    }
}