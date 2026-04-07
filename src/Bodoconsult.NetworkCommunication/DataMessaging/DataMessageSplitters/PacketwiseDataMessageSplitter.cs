// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;

/// <summary>
/// (Dummy) implementation for <see cref="IDataMessageSplitter"/> for packetwise receiving. One packet is resulting in one message
/// </summary>
public class PacketwiseDataMessageSplitter : IDataMessageSplitter
{
    // Array pool is okay as shared instance here
    private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;

    /// <summary>
    /// Main method for TCP/IP message receiving: split the inbound byte stream in commands to process later
    /// </summary>
    /// <param name="buffer">Receiving buffer</param>
    /// <param name="command">The received command. May have a length of zero. Then no valid message was received so far</param>
    /// <returns>True if a command was successfuly extract from the buffer else false</returns>
    public virtual bool TryReadCommand(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> command)
    {
        var array = ArrayPool.Rent((int)buffer.Length);

        buffer.CopyTo(array);
        command = new ReadOnlySequence<byte>(array).Slice(0, buffer.Length) ;
        buffer = buffer.Slice(buffer.Length);

        ArrayPool.Return(array);

        return command.Length > 0;
    }

    public int ComputeDataLength(ref ReadOnlySequence<byte> messageBytes)
    {
        // Not needed here
        return 0;
    }
}