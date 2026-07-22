// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Numerics;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.DataCollectionServices;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.BusinessLogic.Fft;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Replies;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using ScottPlot;

namespace IpBackend.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for UPD channel from backend to IP device
/// </summary>
public class SfxpIpDeviceUdpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IIpDeviceUdpDeviceBusinessLogicAdapter
{
    private readonly IBusinessTransactionManager _businessTransactionManager;
    private long _messageCounter;
    private readonly DataCollectionService<Complex[]> _dataCollectionService;

    /// <summary>
    /// Forward the collected data
    /// </summary>
    /// <param name="data">List with arrays of complex numbers</param>
    public void ForwardCollectDataDelegate(List<Complex[]> data)
    {
        if (data.Count == 0)
        {
            return;
        }

        var i = data.Sum(item => item.Length);

        var result = new List<Complex>(i);

        foreach (var item in data)
        {
            result.AddRange(item);
        }

        var allNumbers = MathHelper.CheckListLengthForFft(result);

        var fft = new FftManager(allNumbers);
        fft.CalculatePsd();
        fft.CalculateFrequencyScale();

        // plot the sample audio
        Plot plt = new();
        plt.Add.ScatterLine(fft.FrequencyScale, fft.Psd);
        plt.Axes.AutoScale();
        plt.YLabel("Power (dB)");
        plt.XLabel("Frequency (Hz)");

        //// sample audio with tones at 2, 10, and 20 kHz plus white noise
        //double[] signal = FftSharp.SampleData.SampleAudio1();
        //int sampleRate = 48_000;

        //// calculate the power spectral density using FFT
        //System.Numerics.Complex[] spectrum = FftSharp.FFT.Forward(signal);
        //double[] psd = FftSharp.FFT.Power(spectrum);
        //double[] freq = FftSharp.FFT.FrequencyScale(psd.Length, sampleRate);

        //// plot the sample audio
        //ScottPlot.Plot plt = new();
        //plt.Add.ScatterLine(freq, psd);
        //plt.YLabel("Power (dB)");
        //plt.XLabel("Frequency (Hz)");

        var bytes = plt.GetImageBytes(1024, 768, ImageFormat.Jpeg);

        //File.WriteAllBytes("C:\\temp\\fft.png", bytes);


        var request = new FftReportBusinessTransactionRequestData
        {
            TransactionId = ServerSideBusinessTransactionIds.ReportFftData,
            JpegImageData = bytes
        };

        _businessTransactionManager.RunBusinessTransaction(ServerSideBusinessTransactionIds.ReportFftData, request);
    }

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public SfxpIpDeviceUdpBusinessLogicAdapter(IIpDevice device, IBusinessTransactionManager businessTransactionManager) : base(device)
    {
        _dataCollectionService = new DataCollectionService<Complex[]>(ForwardCollectDataDelegate);
        _businessTransactionManager = businessTransactionManager;
    }

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
            IpDevice.DataMessagingConfig.MonitorLogger.LogInformation(msg);
        }

        if (_messageCounter == long.MaxValue)
        {
            _messageCounter = 0;
        }

        var result = GetArray(db.DataChunks.Where(x => x.Channel != 0xc).ToList());

        // Process data from datablock here
        _dataCollectionService.Add(result);

        // Return chunks to pool
        foreach (var chunk in db.DataChunks)
        {
            chunk.ReturnDataChunkDelegate?.Invoke(chunk);
        }
        db.DataChunks.Clear();
    }

    /// <summary>
    /// Get an array from the data chunks
    /// </summary>
    /// <param name="chunks">List with data chunks</param>
    /// <returns>Array with a list chunks are rendered to 9 bytes each: first byte is the channel and the next 8 bytes are chunk data</returns>
    public static Complex[] GetArray(List<DataChunk> chunks)
    {
        var result = new List<Complex>(chunks.Count * 8);

        foreach (var chunk in chunks)
        {
            var value = chunk.Data!.Value;

            var c = GetComplex(value.Span[0]);
            result.Add(c);

            c = GetComplex(value.Span[1]);
            result.Add(c);

            c = GetComplex(value.Span[2]);
            result.Add(c);

            c = GetComplex(value.Span[3]);
            result.Add(c);

            c = GetComplex(value.Span[4]);
            result.Add(c);

            c = GetComplex(value.Span[5]);
            result.Add(c);

            c = GetComplex(value.Span[6]);
            result.Add(c);

            c = GetComplex(value.Span[7]);
            result.Add(c);
        }

        return result.ToArray();
    }

    public static Complex GetComplex(byte b)
    {
        var i = Convert.ToDouble(b & 0x0f);

        var q = Convert.ToDouble((b & 0xf0) >> 4);

        return new Complex(i, q);
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
    /// Start the binary data collector
    /// </summary>
    /// <param name="requestData">Start collector request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply StartDataCollector(IBusinessTransactionRequestData requestData)
    {
        if (requestData is StartCollectorBusinessTransactionRequestData startRequest)
        {
            _dataCollectionService.CollectionInterval = startRequest.CollectionInterval;
            _dataCollectionService.CollectionTime = startRequest.CollectionTime;
        }

        _dataCollectionService.Start();

        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Stop the binary data collector
    /// </summary>
    /// <param name="requestData">Empty request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply StopDataCollector(IBusinessTransactionRequestData requestData)
    {
        _dataCollectionService.Stop();

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