// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Exceptions;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Codec to encode and decode device data messages for SFXP protocol
/// </summary>
public class SfxpDataMessageCodec : BaseDataMessageCodec
{
    private const int Length = 10000;
    private readonly List<ulong> _messageIds = new(Length);

    /// <summary>
    /// Current <see cref="IDataBlockCodingProcessor"/> instance
    /// </summary>
    public readonly IDataBlockCodingProcessor DataBlockCodingProcessor;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataBlockCodingProcessor">Current <see cref="IDataBlockCodingProcessor"/> instance</param>
    public SfxpDataMessageCodec(IDataBlockCodingProcessor dataBlockCodingProcessor)
    {
        DataBlockCodingProcessor = dataBlockCodingProcessor;

        ExpectedMinimumLength = 100;
        ExpectedMaximumLength = 16384;
    }

    /// <summary>
    /// Decode a data message to an <see cref="IInboundDataMessage"/> instance
    /// </summary>
    /// <param name="data">Data message bytes received</param>
    /// <returns>Decoding result</returns>
    public override InboundCodecResult DecodeDataMessage(Memory<byte> data)
    {
        // Check length
        var result = CheckExpectedLengths(data.Length);

        if (result.ErrorCode != 0)
        {
            return result;
        }

        try
        {
            var rawBytes = data[..8].Span;

            var messageId = BitConverter.ToUInt64(rawBytes);

            // Check if the current message ID was handled already
            if (_messageIds.Contains(messageId))
            {
                result.ErrorMessage = "Message ID already exists";
                result.ErrorCode = DataMessageCodecErrorCodes.MessageAlreadyReceived;
                return result;
            }

            // Store the current message ID
            if (_messageIds.Count == Length)
            {
                _messageIds.Remove(_messageIds[0]);
            }

            _messageIds.Add(messageId);

            // Now process decoding
            var dataBlockBytes = data.Slice(8); 
            var dataBlock = DataBlockCodingProcessor.FromBytesToDataBlock(dataBlockBytes, (char)0x73);

            var dataMessage = new SfxpInboundDataMessage
            {
                RawMessageData = data,
                OriginalMessageId = messageId,
                DataBlock = dataBlock,
                AnswerWithAcknowledgement = AnswerWithAcknowledgement
            };

            result.DataMessage = dataMessage;
            return result;

        }
        catch (DatablockCodecException dataBlockException)
        {
            result.ErrorMessage = $"DataMessage {ArrayHelper.GetStringFromArrayCsharpStyle(data, false)}: decoding failed: {dataBlockException}";
            result.ErrorCode = 4;
            return result;
        }
        catch (Exception exception)
        {
            result.ErrorMessage = $"DataMessage {ArrayHelper.GetStringFromArrayCsharpStyle(data, false)}: decoding failed: {exception}";
            result.ErrorCode = 5;
            return result;
        }
    }

    /// <summary>
    /// Encodes a message to a byte array to send to receiver
    /// </summary>
    /// <param name="message">Data message to send</param>
    /// <returns>Byte array as optimized <see cref="ReadOnlyMemory{T}"/> to send</returns>
    public override OutboundCodecResult EncodeDataMessage(IOutboundMessage message)
    {
        var result = new OutboundCodecResult();
        if (message is not SfxpOutboundDataMessage tMessage)
        {
            result.ErrorMessage = "SfxpDataMessage required for SfxpDataMessageCodec";
            result.ErrorCode = 1;
            return result;
        }

        tMessage.WaitForAcknowledgement = WaitForAcknowledgement;

        var data = new List<byte>();

        // Add the datablock now if required
        try
        {
            if (tMessage.DataBlock == null)
            {
                result.ErrorMessage = "SfxpDataMessageCodec: datablock must not be null";
                result.ErrorCode = 5;
                return result;
            }
            data.AddRange(tMessage.DataBlock.Data.Span);
        }
        catch (Exception exception)
        {
            result.ErrorMessage = $"SfxpDataMessageCodec: exception raised during encoding: {exception}";
            result.ErrorCode = 4;
            return result;
        }

        tMessage.RawMessageData = data.ToArray().AsMemory();

        return result;
    }
}