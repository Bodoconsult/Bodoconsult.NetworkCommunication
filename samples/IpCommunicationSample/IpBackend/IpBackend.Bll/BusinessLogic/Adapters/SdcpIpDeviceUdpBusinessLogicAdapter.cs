// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpBackend.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for UPD channel from backend to IP device
/// </summary>
public class SfxpIpDeviceUdpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IIpDeviceUdpDeviceBusinessLogicAdapter
{
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
        IpDevice.DataMessagingConfig.AppLogger.LogInformation("Message received: "+message.ToInfoString());

        // ToDo: add your business logic
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