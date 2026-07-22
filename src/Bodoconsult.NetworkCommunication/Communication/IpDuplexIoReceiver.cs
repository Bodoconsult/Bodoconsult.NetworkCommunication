// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Comm adapter subsystem for message receiving for TCP/IP based networks without timer
/// </summary>
public class IpDuplexIoReceiver : BaseDuplexIoReceiver
{
    /// <summary>
    /// Current validator impl for data messages
    /// </summary>
    private readonly IDataMessageValidator _dataMessageValidator;

    private readonly IStreamPipeline _pipeline;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="config">Current device comm settings</param>
    public IpDuplexIoReceiver(IDataMessagingConfig config) : base(config)
    {
        ArgumentNullException.ThrowIfNull(config.SocketProxy);

        if (config.SocketProxy is not ITcpSocketProxy socketProxy)
        {
            throw new ArgumentException("deviceCommSettings.SocketProxy is not ITcpSocketProxy");
        }

        socketProxy.StartReceiverLoop();

        if (socketProxy.ReceiverPipeline is not IStreamPipeline pipeline)
        {
            throw new ArgumentException("_socketProxy.ReceiverPipeline is not IStreamPipeline");
        }

        _pipeline = pipeline;
        _pipeline.SocketReceivedDataDelegate = SocketReceivedData;

        ArgumentNullException.ThrowIfNull(config.DataMessageProcessingPackage);

        _dataMessageValidator = config.DataMessageProcessingPackage.DataMessageValidator;
    }

    /// <summary>
    /// Handle data the socket has received
    /// </summary>
    public void SocketReceivedData()
    {
        var buffer = _pipeline.Buffer;

        if (buffer.IsEmpty)
        {
            return;
        }

        var msg = buffer.Length < DeviceCommunicationBasics.DataMessageMaxLoggedSize ? 
            $"{LoggerId}Buffer ({buffer.Length}B): {DataMessageHelper.GetStringFromArrayCsharpStyle(ref buffer)}" : 
            $"{LoggerId}Buffer ({buffer.Length}B)";

#if DEBUG
        if (buffer.Length < DeviceCommunicationBasics.DataMessageMaxLoggedSize)
        {
            msg = $"{LoggerId}buffer {buffer.Length}B: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref buffer)}";
        }
        else
        {
            msg = $"{LoggerId}buffer {buffer.Length}B";
        }

        DataMessagingConfig.MonitorLogger?.LogDebug(msg);
#endif

        MonitorLogger.LogInformation(msg);


        while (DataMessageSplitter.TryReadCommand(ref buffer, out var command))
        {
            var length = (int)command.Length;
            if (length == 0)
            {
                continue;
            }

            // Take a copy of the command to avoid errors if the pipeline socket is closed before processing the command
            var array = command.ToArray().AsMemory();
            var codecResult = DataMessageCodingProcessor.DecodeDataMessage(array);

            //Trace.TraceInformation($"IpDuplexIoReceiver: parsed command {Encoding.UTF8.GetString(command)} to message {codecResult.DataMessage?.MessageId}. Buffer {_buffer.Length}: { Encoding.UTF8.GetString( _buffer)}");
            //Trace.TraceInformation($"IpDuplexIoReceiver: parsed command {Encoding.UTF8.GetString(command)} to message {codecResult.DataMessage?.MessageId}");

            if (codecResult.ErrorCode != 0 || codecResult.DataMessage == null)
            {
                msg = $"Parsing command failed with error code {codecResult.ErrorCode}: {codecResult.ErrorMessage}: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
                MonitorLogger?.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");

                _pipeline.MoveForward(buffer.Length);
                return;
            }

            var validationResult = _dataMessageValidator.IsMessageValid(codecResult.DataMessage);
            if (!validationResult.IsMessageValid)
            {
                msg = $"Parsed command NOT valid: {validationResult.ValidationResult}: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
                MonitorLogger?.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");
            }
            else
            {
#if DEBUG
                if (command.Length < DeviceCommunicationBasics.DataMessageMaxLoggedSize)
                {
                    msg = $"{LoggerId}Parsed {codecResult.DataMessage.ToShortInfoString()}: buffer {buffer.Length}B: {DataMessageHelper.GetStringFromArrayCsharpStyle(ref command)}";
                }
                else
                {
                    msg = $"{LoggerId}Parsed {codecResult.DataMessage.ToShortInfoString()}: buffer {buffer.Length}B";
                }

                DataMessagingConfig.MonitorLogger?.LogDebug(msg);
#endif

                DataMessageProcessor.AddMessageToQueue(codecResult.DataMessage);
            }

            if (buffer.Length == 0)
            {
                break;
            }
        }

        _pipeline.MoveForward(buffer.Length);
    }

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

        CancellationSource?.Dispose();
        CancellationSource = null;
    }
}