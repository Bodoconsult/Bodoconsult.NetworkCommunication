// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpCommunicationSample.Device.Bll.Interfaces;

/// <summary>
/// Interface for the control channel from device to backend via TCP/IP
/// </summary>
public interface IBackendTcpIpBusinessLogicAdapter : ISimpleDeviceBusinessLogicAdapter
{ }