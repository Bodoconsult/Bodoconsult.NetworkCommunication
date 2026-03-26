// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;
using IpCommunicationSample.Device.Bll.Interfaces;

namespace IpCommunicationSample.Device.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for TCP/IP control channel from backend to IP device
/// </summary>
public class TncpBackendTcpIpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IIpDeviceTcpIpBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public TncpBackendTcpIpBusinessLogicAdapter(IIpDevice device) : base(device)
    {
    }

    /// <summary>
    /// Default method to handle a received message from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public override void DefaultReceiveMessage(IInboundDataMessage message)
    {
        throw new NotSupportedException("Override in derived classes");
    }

}