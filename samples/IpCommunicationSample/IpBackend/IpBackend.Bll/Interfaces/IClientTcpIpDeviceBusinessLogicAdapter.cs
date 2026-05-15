// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpBackend.Bll.Interfaces;

/// <summary>
/// Interface for the channel from client to backend via TCP/IP
/// </summary>
public interface IClientTcpIpDeviceBusinessLogicAdapter : ISimpleDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Report an error reported by the device to the client
    /// </summary>
    /// <param name="request">Error request</param>
    /// <returns>Default transaction reply</returns>
    IBusinessTransactionReply ReportDeviceError(IBusinessTransactionRequestData request);
}