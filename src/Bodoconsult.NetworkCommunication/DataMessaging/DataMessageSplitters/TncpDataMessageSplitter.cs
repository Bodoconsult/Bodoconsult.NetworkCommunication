// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;

/// <summary>
/// Implementation for <see cref="IDataMessageSplitter"/> for TNCP protocol
/// </summary>
public class TncpDataMessageSplitter : IDataMessageSplitter
{

    // Array pool is okay as shared instance here
    private static readonly ArrayPool<byte> ArrayPool = ArrayPool<byte>.Shared;

    /// <summary>
    /// Length of handshake messages
    /// </summary>
    protected int HandshakeLength = 1;

    /// <summary>
    /// Main method for TCP/IP message receiving: split the inbound byte stream in commands to process later
    /// </summary>
    /// <param name="buffer">Receiving buffer</param>
    /// <param name="command">The received command. May have a length of zero. Then no valid message was received so far</param>
    /// <returns>True if a command was successfuly extract from the buffer else false</returns>
    public virtual bool TryReadCommand(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> command)
    {
        if (buffer.Length == 0)
        {
            command = ReadOnlySequence<byte>.Empty;
            return false;
        }

        var result = TryReadCommandInternal(ref buffer, out command);

        // Check for nulls string the

        command = DataMessageHelper.CheckCommandForNullAtTheEnd(command);

        // Now copy the command if required
        if (command.Length <= 0)
        {
            return result;
        }
        var array = ArrayPool.Rent((int)command.Length);

        command.CopyTo(array);
        command = new ReadOnlySequence<byte>(array).Slice(0, command.Length);

        // Now remove the command out of the buffer
        buffer = buffer.Slice(command.Length);

        ArrayPool.Return(array);

        return result;
    }

    private bool TryReadCommandInternal(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> command)
    {
        //Debug.Print($"TryReadCommand: {GetStringFromArray(buffer.ToArray())}");

        if (buffer.Length == 0)
        {
            command = default;
            return false;
        }

        // Find the end byte 
        byte firstByte;
        var posEndByte = 0;
        for (var i = 0; i < buffer.Length; i++)
        {
            firstByte = buffer.Slice(i, 1).FirstSpan[0];
            // First byte is message start byte
            if (firstByte == DeviceCommunicationBasics.Cr)
            {
                break;
            }

            posEndByte++;
        }

        if (posEndByte > 0 && posEndByte + 1 <= buffer.Length)
        {
            command = buffer.Slice(0, posEndByte + 1);
            return true;
        }

        // Handshake
        posEndByte = 0;
        for (var i = 0; i < buffer.Length; i++)
        {
            firstByte = buffer.Slice(i, 1).FirstSpan[0];
            // First byte is message start byte
            if (DeviceCommunicationBasics.HandshakeMessageStartTokens.Contains(firstByte))
            {
                break;
            }

            posEndByte++;
        }

        if (posEndByte > 0 && posEndByte + HandshakeLength < buffer.Length)
        {
            command = buffer.Slice(posEndByte, HandshakeLength);
            return true;
        }

        command = default;
        return false;
    }

    /// <summary>
    /// Compute the datablock length depending on firmware version
    /// </summary>
    /// <param name="messageBytes">Raw data as byte array</param>
    /// <returns>Length of the datablock</returns>
    public int ComputeDataLength(ref ReadOnlySequence<byte> messageBytes)
    {
        // Not needed for SDCP
        return 0;
    }

}