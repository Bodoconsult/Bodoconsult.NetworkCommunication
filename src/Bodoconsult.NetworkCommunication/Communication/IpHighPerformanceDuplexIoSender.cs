// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for message sending version 2 for IP based networks (UDP/TCP) 
/// </summary>
public class IpHighPerformanceDuplexIoSender : BaseDuplexIoSender
{

    private readonly Pipe _pipe;

    private Task? _sendLoop;

    private CancellationTokenSource? _cancellationSource;

    private readonly int _pollingTimeOut;

    private readonly ISocketProxy _socketProxy;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="pipe">Current pipe to use</param>
    /// <param name="config">Current device comm settings</param>
    /// <param name="pollingTimeOut">Polling timeout in milliseconds</param>
    public IpHighPerformanceDuplexIoSender(Pipe pipe, IDataMessagingConfig config, int pollingTimeOut) : base(config)
    {
        _pipe = pipe;
        _pollingTimeOut = pollingTimeOut;

        _socketProxy = DataMessagingConfig.SocketProxy ?? throw new ArgumentNullException(nameof(DataMessagingConfig.SocketProxy));
    }

    /// <summary>
    /// Start the message sender
    /// </summary>
    public override async Task StartSender()
    {
        _cancellationSource = new CancellationTokenSource();

        await Task.Run(() =>
        {
            _sendLoop = Task.Run(SendMessageLoop);
        });
    }

    /// <summary>
    /// Stop the message sender
    /// </summary>
    public override async Task StopSender()
    {
        await Task.Run(() =>
        {
            _cancellationSource?.Cancel();

            if (_sendLoop == null)
            {
                return;
            }
            Wait.Until(() => _sendLoop == null || _sendLoop.IsCompleted, 1000);
            _sendLoop = null;
        });
    }

    /// <summary>
    /// Send a message to the device
    /// </summary>
    /// <param name="message">Current message to send</param>
    public override async Task<int> SendMessage(IOutboundMessage message)
    {
        OutboundCodecResult? result = null;

        try
        {
            //Create a byte array from message according to current protocol
            result = DataMessageCodingProcessor.EncodeDataMessage(message);

            if (result.ErrorCode != 0)
            {
                AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, result.ErrorMessage));
                AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(new Exception(result.ErrorMessage)));
                return 0;
            }

            //Trace.TraceInformation($"SendMessage: {messageBytes.Length}");

            await _pipe.Writer.WriteAsync(message.RawMessageData);

            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(message.RawMessageData));

            return message.RawMessageData.Length;
        }
        catch (Exception exception)
        {
            if (result != null)
            {
                AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(message.RawMessageData, exception.Message));
            }

            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
            return 0;
        }
    }

    /// <summary>
    /// Implements the loop for message sending to the device
    /// </summary>
    /// <returns></returns>
    private async Task SendMessageLoop()
    {

        try
        {
            while (!_cancellationSource?.Token.IsCancellationRequested ?? false)
            {

                if (!_socketProxy.Connected)
                {
                    AsyncHelper.Delay(5);
                    continue;
                }

                var read = await _pipe.Reader.ReadAsync();

                if (read.IsCanceled)
                {
                    break;
                }

                var buffer = read.Buffer;

                //Trace.TraceInformation($"Buffer: {buffer.Length}");

                if (buffer.IsEmpty && read.IsCompleted)
                {
                    break;
                }

                if (buffer.IsEmpty)
                {
                    continue;
                }

                await SendMessageInternal(buffer);
                
                _pipe.Reader.AdvanceTo(buffer.Start, buffer.End);

                ////Trace.TraceInformation($"Parsed command: {SmddeviceMessageHelper.GetStringFromArrayCsharpStyle(ref buffer)}");

                //while (DataMessageSplitter.TryReadCommand( ref buffer, out var command))
                //{

                //    //Trace.TraceInformation($"Command: length 1: {command.Length}");
                //    command = SmddeviceMessageHelper.CheckCommandForNullAtTheEnd(command);

                //    if (command.Length > 0)
                //    {

                //        //_monitorLogger.LogInformation($"Parsed command: {SmddeviceMessageHelper.GetStringFromArrayCsharpStyle(ref command)}");
                //        try
                //        {
                //            await SendMessageInternal(command);
                //        }
                //        catch (SocketException socketException)
                //        {
                //            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(socketException));
                //        }
                //        catch (Exception sendException)
                //        {
                //            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(sendException));
                //        }
                //    }
                //    else
                //    {
                //        DataMessagingConfig.MonitorLogger.LogError($"Parsed command: empty");
                //    }
                //}

                //_pipe.Reader.AdvanceTo(buffer.Start, buffer.End);

            }
        }
        catch (Exception exception)
        {
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
        }

        await _pipe.Writer.CompleteAsync();
        await _pipe.Reader.CompleteAsync();
    }

    /// <summary>
    /// Sends the message
    /// </summary>
    /// <param name="command">Current message to send</param>
    /// <returns></returns>
    protected async Task SendMessageInternal(ReadOnlySequence<byte> command)
    {
        string msg;
        var success = SequenceMarshal.TryGetReadOnlyMemory(command, out var readOnlyMemory);

        var sent = 0;

        if (success)
        {
            try
            {
                sent = await _socketProxy.Send(readOnlyMemory);
            }
            catch (SocketException socketException)
            {
                msg = $"FillMessagePipeline failed: {socketException}";
                DataMessagingConfig.MonitorLogger.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}: {msg}");

                AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("TcpIpDuplexIoSender"));
                AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(command.ToArray(), socketException.Message));
                AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(socketException));
                return;
            }
            catch (Exception e)
            {
                msg = $"FillMessagePipeline failed: {e}";
                DataMessagingConfig.MonitorLogger.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}: {msg}");
                AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(command.ToArray(), e.Message));
                AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(e));
                return;
            }
        }

        if (sent <= 0)
        {
            AsyncHelper.FireAndForget(() => DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(command.ToArray(), "Reason unknown"));

            msg = "message could not be sent via socket: unknown reason";
            DataMessagingConfig.MonitorLogger.LogError(msg);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}: {msg}");
            return;
        }

        DataMessagingConfig.MonitorLogger.LogInformation($"sent {command.Length} bytes to device");
    }

    /// <summary>
    /// Current implementation of disposing
    /// </summary>
    /// <param name="disposing">True if diposing should run</param>
    protected override async Task Dispose(bool disposing)
    {
        if (!disposing)
        {
        }

        await Task.Run(() =>
        {
            try
            {
                _sendLoop?.Dispose();
            }
            catch
            {
                // Do nothing
            }
        });
    }
}