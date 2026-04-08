// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for digital data message factories simulating the real behaviour of a IP device
/// </summary>
public interface IDigitalTwinMessageFactory
{
    /// <summary>
    /// The number of messages created
    /// </summary>
    int NumberOfMessagesCreated { get; set; }

    /// <summary>
    /// Generate a set of messages as defined with <see cref="NumberOfMessagesCreated"/>
    /// </summary>
    /// <returns>List with messages to send</returns>
    List<Memory<byte>> GenerateMessages();

    /// <summary>
    /// Generate the next message in endless mode
    /// </summary>
    /// <returns>Message to send</returns>
    Memory<byte> GenerateNextMessage();

}