// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Codec to encode and decode device data messages for Tncp protocol
/// </summary>
public class TncpDataMessageCodec : BaseDataMessageCodec
{
    /// <summary>
    /// Current <see cref="IDataBlockCodingProcessor"/> instance
    /// </summary>
    public readonly IDataBlockCodingProcessor DataBlockCodingProcessor;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataBlockCodingProcessor">Current <see cref="IDataBlockCodingProcessor"/> instance</param>
    public TncpDataMessageCodec(IDataBlockCodingProcessor dataBlockCodingProcessor)
    {
        DataBlockCodingProcessor = dataBlockCodingProcessor;

        ExpectedMinimumLength = DeviceCommunicationBasics.DataMessageMinPacketSize;
        ExpectedMaximumLength = DeviceCommunicationBasics.DataMessageMaxPacketSize;
    }

    /// <summary>
    /// Decode a data message to an <see cref="IInboundDataMessage"/> instance
    /// </summary>
    /// <param name="data">Data message bytes received</param>
    /// <returns>Decoding result</returns>
    public override InboundCodecResult DecodeDataMessage(Memory<byte> data)
    {
        // Check STX
        if (data[..1].Span[0] != DeviceCommunicationBasics.Stx)
        {
            return new InboundCodecResult
            {
                ErrorCode = 98,
                ErrorMessage = "STX is missing"
            };
        }

        // Check ETX
        if (data.Slice(data.Length - 1, 1).Span[0] != DeviceCommunicationBasics.Etx)
        {
            return new InboundCodecResult
            {
                ErrorCode = 99,
                ErrorMessage = "ETX is missing"
            };
        }

        // Check length
        var result = CheckExpectedLengths(data.Length);

        if (result.ErrorCode != 0)
        {
            return result;
        }

        try
        {
            ITypedInboundDataBlock? dataBlock;

            var blockCode = data.Slice(1, 1).Span[0];

            var dataBlockBytes = data.Slice(2, data.Length - 2);

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


            var dataMessage = new TncpInboundDataMessage
            {
                DataBlock = dataBlock,
                BlockCode = blockCode
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
        if (message is not TncpOutboundDataMessage tMessage)
        {
            result.ErrorMessage = "TncpDataMessage required for TncpDataMessageCodec";
            result.ErrorCode = 1;
            return result;
        }

        var data = new List<byte> { DeviceCommunicationBasics.Stx, tMessage.BlockCode };

        if (tMessage.DataBlock != null)
        {

            // Add the datablock now if required
            try
            {
                DataBlockCodingProcessor.FromDataBlockToBytes(data, tMessage.DataBlock);
            }
            catch (Exception exception)
            {
                result.ErrorMessage = $"TncpDataMessageCodec: exception raised during encoding: {exception}";
                result.ErrorCode = 4;
                return result;
            }
        }
        else
        {
            result.ErrorMessage = "TncpDataMessageCodec: no datablock provided";
            result.ErrorCode = 5;
            return result;
        }

        // Add the final ETX now
        data.Add(DeviceCommunicationBasics.Etx);

        tMessage.RawMessageData = data.ToArray().AsMemory();

        return result;
    }
}
