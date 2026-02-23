// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based outbound messaging containing data i.e. via TCP/IP or UDP
/// </summary>
public interface IOutboundDataMessage: IOutboundMessage
{
    /// <summary>
    /// Is waiting for acknowledgement by the device required for the message
    /// </summary>
    bool WaitForAcknowledgement { get; }
}