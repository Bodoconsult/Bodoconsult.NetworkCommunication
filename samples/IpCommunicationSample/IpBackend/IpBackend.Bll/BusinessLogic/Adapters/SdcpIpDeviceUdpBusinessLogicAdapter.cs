// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Backend.Bll.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for UPD channel from backend to IP device
/// </summary>
public class SdcpIpDeviceUdpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IIpDeviceUdpDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public SdcpIpDeviceUdpBusinessLogicAdapter(IIpDevice device) : base(device)
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
}