// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for digital data message factories simulating the real behaviour of a IP device
/// </summary>
public interface IDigitalTwinMessageFactory
{
    /// <summary>
    /// Generate messages
    /// </summary>
    /// <returns>List with messages to send</returns>
    List<ReadOnlySequence<byte>> GenerateMessages();
}