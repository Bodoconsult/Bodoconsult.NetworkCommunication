// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for data messages used for commands
/// </summary>
public interface ICommandDataMessage : IInboundDataMessage
{

    /// <summary>
    /// Command as char
    /// </summary>
    char Command { get; }


    /// <summary>
    /// Block and RC of SMDTowerMessage message
    /// </summary>
    byte BlockAndRc { get; }

    /// <summary>
    /// Error code delivered with the message
    /// </summary>
    byte Error { get; set; }
}