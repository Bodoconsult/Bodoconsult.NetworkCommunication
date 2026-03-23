// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;
using IpCommunicationSample.Device.Bll.Interfaces;

namespace IpCommunicationSample.Device.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for UPD data channel from backend to IP device
/// </summary>
public class SdcpBackendUdpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IIpDeviceUdpBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public SdcpBackendUdpBusinessLogicAdapter(IIpDevice device) : base(device)
    {
    }
}