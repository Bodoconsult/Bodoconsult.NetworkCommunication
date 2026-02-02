// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Buffers;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageSplitters;

/// <summary>
/// Implementation for <see cref="IDataMessageSplitter"/> for SDCP protocol
/// </summary>
public class SdcpDataMessageSplitter : IDataMessageSplitter
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

        var firstByte = buffer.Slice(0, 1).FirstSpan[0];

        // First byte is no message start: remove byte until next message start
        while (true)
        {
            // First byte is message start byte
            if (DeviceCommunicationBasics.MessageStartTokens.Contains(firstByte))
            {
                break;
            }

            // Remove byte if no message start byte
            buffer = buffer.Slice(1);

            if (buffer.Length == 0)
            {
                command = default;
                return false;
            }

            firstByte = buffer.Slice(0, 1).FirstSpan[0];

        }

        // No other
        if (buffer.Length < 1)
        {
            command = default;
            return false;
        }

        // First token is not a message start token
        if (!DeviceCommunicationBasics.MessageStartTokens.Contains(firstByte))
        {
            // Handshake with length 1?
            if (HandshakeLength == 1)
            {
                command = buffer.Slice(0, 1);
                buffer = buffer.Slice(1);
                return true;
            }

            // Check if there is a message start token following: if yes return invalid message
            for (var i = 1; i < HandshakeLength; i++)
            {
                var nextByte = buffer.Slice(i, 1).FirstSpan[0];
                if (!DeviceCommunicationBasics.MessageStartTokens.Contains(nextByte))
                {
                    continue;
                }
                command = buffer.Slice(0, i);
                buffer = buffer.Slice(i);
                return true;
            }


            command = default;
            return false;
        }

        // Handshake
        if (DeviceCommunicationBasics.HandshakeMessageStartTokens.Contains(firstByte))
        {

            var nextByte = buffer.Slice(1, 1).FirstSpan[0];

            if (!DeviceCommunicationBasics.MessageStartTokens.Contains(nextByte))
            {
                command = buffer.Slice(0, HandshakeLength);
                buffer = buffer.Slice(HandshakeLength);
                return true;
            }

            command = buffer.Slice(0, 1);
            buffer = buffer.Slice(1);
            return true;
        }

        // Data message
        if (firstByte != DeviceCommunicationBasics.Stx)
        {
            command = default;
            buffer = buffer.Slice(1);
            return false;
        }

        // Find end of message ETX
        int etxPos;
        var etxFound = false;
        for (etxPos = 0; etxPos < buffer.Length; etxPos++)
        {
            if (buffer.Slice(etxPos, 1).FirstSpan[0] == DeviceCommunicationBasics.Etx)
            {
                etxFound = true;
                break;
            }
        }

        if (!etxFound)
        {
            command = default;
            return false;
        }

        command = buffer.Slice(0, etxPos + 1);
        return true;
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