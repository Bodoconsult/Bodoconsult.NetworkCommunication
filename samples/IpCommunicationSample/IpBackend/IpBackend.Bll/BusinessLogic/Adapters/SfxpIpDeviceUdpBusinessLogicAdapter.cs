// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions.Replies;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IpBackend.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for UPD channel from backend to IP device
/// </summary>
public class SfxpIpDeviceUdpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IIpDeviceUdpDeviceBusinessLogicAdapter
{
    private long _messageCounter;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public SfxpIpDeviceUdpBusinessLogicAdapter(IIpDevice device) : base(device)
    { }

    /// <summary>
    /// Default method to handle a received message from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public override void DefaultReceiveMessage(IInboundDataMessage message)
    {
        // No SFXP message
        if (message is not SfxpInboundDataMessage sfxp)
        {
            return;
        }

        // No datablock
        if (sfxp.DataBlock is not SfxpInboundDatablock db)
        {
            return;
        }

        // Reduced logging to avoid performance issues
        _messageCounter++;

        if (Math.Abs(_messageCounter % 1000.0) < 0.1)
        {
            var msg = $"Received message {_messageCounter} ({message.RawMessageData.Length}B)";
            //Debug.Print(msg);
            IpDevice.DataMessagingConfig.AppLogger.LogInformation(msg);
        }

        if (_messageCounter == long.MaxValue)
        {
            _messageCounter = 0;
        }

        // Process data from datablock here
        // ToDo: add your business logic

        // Return chunks to pool
        foreach (var chunk in db.DataChunks)
        {
            chunk.ReturnDataChunkDelegate?.Invoke(chunk);
        }
        db.DataChunks.Clear();
    }


    /// <summary>
    /// Flush the binary data loggers
    /// </summary>
    /// <param name="requestData">Empty request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply FlushDataLoggers(IBusinessTransactionRequestData requestData)
    {
        var loggers = IpDevice.DataMessagingConfig.DataLoggers;

        foreach (var logger in loggers)
        {
            logger.FlushCache();
        }

        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Start the binary data loggers
    /// </summary>
    /// <param name="requestData">Empty request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply StartDataLoggers(IBusinessTransactionRequestData requestData)
    {
        var loggers = IpDevice.DataMessagingConfig.DataLoggers;

        foreach (var logger in loggers)
        {
            logger.Start();
        }

        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Stop the binary data loggers
    /// </summary>
    /// <param name="requestData">Empty request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply StopDataLoggers(IBusinessTransactionRequestData requestData)
    {
        var loggers = IpDevice.DataMessagingConfig.DataLoggers;

        foreach (var logger in loggers)
        {
            logger.Stop();
        }

        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Send the required client hello to the server
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply CheckConnection(IBusinessTransactionRequestData requestData)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);
        IpDevice.CommunicationAdapter.ComDevClose();
        IpDevice.CommunicationAdapter.ComDevInit();

        //

        ////if (IpDevice.CommunicationAdapter.IsConnected)
        ////{
        ////    if (IpDevice.CommunicationAdapter.CommunicationHandler == null)
        ////    {
        ////        IpDevice.CommunicationAdapter.ComDevInit();
        ////    }
        ////    else
        ////    {
        ////        IpDevice.CommunicationAdapter.ComDevInit();

        ////        //IpDevice.CommunicationAdapter.CommunicationHandler.Disconnect();
        ////        //IpDevice.CommunicationAdapter.CommunicationHandler.Connect();

        ////        Trace.TraceInformation("SfxpIpDeviceUdpBusinessLogicAdapter: Connection reset");
        ////    }

        ////    return new DefaultBusinessTransactionReply();
        ////}

        //if (IpDevice.CommunicationAdapter.ComDevInit())
        //{
        return new DefaultBusinessTransactionReply();
        //}

        //return new DefaultBusinessTransactionReply
        //{
        //    ErrorCode = 1,
        //    Message = "No connection"
        //};
    }

    /// <summary>
    /// Start data logging
    /// </summary>
    /// <param name="requestData">Empty request</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply StartDataLogging(IBusinessTransactionRequestData requestData)
    {
        StartDataLoggers(requestData);

        IpDevice.DataMessagingConfig.IsDataLoggingActivated = true;

        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Stop data logging
    /// </summary>
    /// <param name="requestData">Empty request</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply StopDataLogging(IBusinessTransactionRequestData requestData)
    {
        IpDevice.DataMessagingConfig.IsDataLoggingActivated = false;

        var loggers = IpDevice.DataMessagingConfig.DataLoggers;

        foreach (var logger in loggers)
        {
            logger.Stop();
        }

        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Send the required client hello to the server
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply LoadStreamingConfig(IBusinessTransactionRequestData requestData)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.DataMessagingConfig.DataMessageProcessingPackage);

        if (requestData is not LoadStreamingConfigBusinessTransactionRequestData request)
        {
            throw new ArgumentException($"requestData is not {nameof(LoadStreamingConfigBusinessTransactionRequestData)}");
        }

        // Now find the datablock codec
        var codec = IpDevice.DataMessagingConfig.DataMessageProcessingPackage.DataBlockCodingProcessor
            .GetDatablockCodecCanBeNull('s');

        if (codec is not SfxpDataBlockCodec sfxp)
        {
            throw new ArgumentException($"codec is not {nameof(SfxpDataBlockCodec)}");
        }

        // Now load the streaming config
        sfxp.LoadStreamingConfig(request.Config);

        IpDevice.DataMessagingConfig.MonitorLogger.LogInformation($"Received config: {ArrayHelper.GetStringFromArrayCsharpStyle(request.Config, false)}");
        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Send the required client hello to the server
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply SendClientHello(IBusinessTransactionRequestData requestData)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);

        var msg = new RawOutboundDataMessage
        {
            RawMessageData = new Memory<byte>([0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x66, 0x72, 0x6f, 0x6d, 0x20, 0x63, 0x6c, 0x69, 0x65, 0x6e, 0x74])
        };


        var task = IpDevice.CommunicationAdapter.SendDataMessage(msg);
        var result = task.GetAwaiter().GetResult();

        if (result.ProcessExecutionResult.Id == OrderExecutionResultState.Successful.Id)
        {
            return new DefaultBusinessTransactionReply();
        }

        return new DefaultBusinessTransactionListReply
        {
            ErrorCode = result.ProcessExecutionResult.Id,
            Message = "Sending HELLO failed"
        };
    }

    public IBusinessTransactionReply CreateFftAnalysisReport(IBusinessTransactionRequestData requestData)
    {
        if (requestData is not FftReportBusinessTransactionRequestData fft)
        {
            throw new ArgumentException($"requestData is not {nameof(FftReportBusinessTransactionRequestData)}");
        }

        // ToDo: collect data here

        return new FftReportBusinessTransactionReply();
    }
}