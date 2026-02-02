// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using System.Buffers;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for main functionality for TCP/IP message receiving: split the inbound byte stream in commands to process later
/// </summary>
public interface IDataMessageSplitter
{

    /// <summary>
    /// Main method for TCP/IP message receiving: split the inbound byte stream in commands to process later
    /// </summary>
    /// <param name="buffer">Receiving buffer</param>
    /// <param name="command">The received command. May have a length of zero. Then no valid message was received so far</param>
    /// <returns>True if a command was successfuly extract from the buffer else false</returns>
    bool TryReadCommand(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> command);

    /// <summary>
    /// Compute the datablock length depending on firmware version
    /// </summary>
    /// <param name="messageBytes">Raw data as byte array</param>
    /// <returns>Length of the datablock</returns>
    int ComputeDataLength(ref ReadOnlySequence<byte> messageBytes);

}