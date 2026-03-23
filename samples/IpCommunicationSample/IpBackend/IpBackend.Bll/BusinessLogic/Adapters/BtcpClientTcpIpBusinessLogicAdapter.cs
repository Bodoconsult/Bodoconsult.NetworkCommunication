// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;
using IpCommunicationSample.Backend.Bll.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for TCP/IP channel from backend to client
/// </summary>
public class BtcpClientTcpIpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IClientTcpIpDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current IP device</param>
    public BtcpClientTcpIpBusinessLogicAdapter(IIpDevice device) : base(device)
    {
    }
}