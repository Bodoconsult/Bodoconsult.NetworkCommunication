// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;
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
    {
    }
}