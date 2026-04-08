// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Base interface for connecting a comm layer with a device specific business logic
/// </summary>
public interface IDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Current app logger
    /// </summary>
    public IAppLoggerProxy AppLogger { get; }

    /// <summary>
    /// Current device
    /// </summary>
    public IIpDevice IpDevice { get; }

    /// <summary>
    /// Send an outbound datamessage
    /// </summary>
    /// <param name="message">Outbound datamessage</param>
    /// <returns>Message sending result</returns>
    public MessageSendingResult SendMessage(IOutboundDataMessage message);
}