// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Codec to encode and decode device data messages for SFXP protocol
/// </summary>
public class SfxpDataMessageCodec : BaseDataMessageCodec
{
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

        ExpectedMinimumLength = 8;
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
            ITypedInboundDataBlock? dataBlock;

            var rawBytes = data[..^1].ToArray();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(rawBytes);
            }

            var messageId = BitConverter.ToInt64(rawBytes, 0);

            var dataBlockBytes = data.Slice(6, data.Length - 7);
            dataBlockBytes.Span[0] = 0x78;

            try
            {
                dataBlock = DataBlockCodingProcessor.FromBytesToDataBlock(dataBlockBytes);
            }
            catch (Exception dataBlockException)
            {
                result.ErrorMessage = $"DataBlock {DataMessageHelper.ByteArrayToString(dataBlockBytes)}: decoding failed: {dataBlockException}";
                result.ErrorCode = 4;
                return result;
            }

            var dataMessage = new SfxpInboundDataMessage
            {
                OriginalMessageId = messageId,
                DataBlock = dataBlock
            };

            result.DataMessage = dataMessage;
            return result;

        }
        catch (Exception exception)
        {
            result.ErrorMessage = $"DataMessage {DataMessageHelper.ByteArrayToString(data)}: decoding failed: {exception.Message}";
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