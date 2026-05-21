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
        //Trace.TraceInformation($"TryReadCommand: {GetStringFromArray(buffer.ToArray())}");

        if (buffer.Length == 0)
        {
            command = default;
            return false;
        }

        // Find the end byte 
        byte firstByte;

        var posEndByte = 0;

        var isReply = false;

        for (var i = 0; i < buffer.Length; i++)
        {
            firstByte = buffer.Slice(i, 1).FirstSpan[0];
            // First byte is message start byte

            if (DeviceCommunicationBasics.HandshakeMessageStartTokens.Contains(firstByte))
            {
                command = buffer.Slice(i, 1);
                buffer = buffer.Slice(i);
                return true;
            }

            // Only at position 0
            if (i == 0)
            {
                // First byte is LF
                if (firstByte == 0xa)
                {
                    command = default;
                    return false;
                }

                // Check if <BEGIN> follows
                isReply = CheckIfReply(buffer, i);
            }

            // LF?
            if (firstByte == DeviceCommunicationBasics.Lf)
            {
                if (i == buffer.Length - 1)
                {
                    break;
                }

                if (buffer.Slice(i + 1, 1).FirstSpan[0] != 0x3c)
                {
                    break;
                }

                if (isReply)
                {
                    if (i > 0 && i < buffer.Length - 6 && CheckIfReply(buffer, i + 1))
                    {
                        break;
                    }

                    if (CheckIfEnd(buffer, i))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            posEndByte++;
        }

        if (posEndByte > 0 && posEndByte + 1 <= buffer.Length)
        {
            command = buffer.Slice(0, posEndByte + 1);
            return true;
        }
        command = default;
        return false;
    }

    /// <summary>
    /// Check if the buffer end with END before LF
    /// </summary>
    /// <param name="buffer">Current buffer</param>
    /// <param name="i">Position of LF</param>
    /// <returns>True if the chars before LF are END</returns>
    public static bool CheckIfEnd(ReadOnlySequence<byte> buffer, int i)
    {
        if (i - 6 < 0)
        {
            return false;
        }

        if (buffer.Slice(i - 5, 1).FirstSpan[0] != 0x3c) // <
        {
            return false;
        }

        if (buffer.Slice(i - 4, 1).FirstSpan[0] != 0x45) // E
        {
            return false;
        }

        if (buffer.Slice(i - 3, 1).FirstSpan[0] != 0x4e) // N
        {
            return false;
        }

        if (buffer.Slice(i - 2, 1).FirstSpan[0] != 0x44) // D
        {
            return false;
        }

        if (buffer.Slice(i - 1, 1).FirstSpan[0] != 0x3e) // <
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Check if the buffer starts with BEGIN
    /// </summary>
    /// <param name="buffer">Current buffer</param>
    /// <param name="i">Position</param>
    /// <returns>True if the chars after are BEGIN</returns>
    public static bool CheckIfReply(ReadOnlySequence<byte> buffer, int i)
    {
        if (buffer.Slice(i, 1).FirstSpan[0] != 0x3c) // <
        {
            return false;
        }
        if (buffer.Slice(i + 1, 1).FirstSpan[0] != 0x42) // B
        {
            return false;
        }
        if (buffer.Slice(i + 2, 1).FirstSpan[0] != 0x45) // E
        {
            return false;
        }
        if (buffer.Slice(i + 3, 1).FirstSpan[0] != 0x47) // G
        {
            return false;
        }
        if (buffer.Slice(i + 4, 1).FirstSpan[0] != 0x49) // I
        {
            return false;
        }
        if (buffer.Slice(i + 5, 1).FirstSpan[0] != 0x4e) // N
        {
            return false;
        }
        return buffer.Slice(i + 6, 1).FirstSpan[0] == 0x3e; // >
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