// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Codec to encode and decode device data messages for SDCP protocol
/// </summary>
public class SdcpDataMessageCodec : BaseDataMessageCodec
{

    public readonly IDataBlockCodingProcessor DataBlockCodingProcessor;


    public SdcpDataMessageCodec(IDataBlockCodingProcessor dataBlockCodingProcessor)
    {
        DataBlockCodingProcessor = dataBlockCodingProcessor;

        ExpectedMinimumLength = DeviceCommunicationBasics.DataMessageMinPacketSize;
        ExpectedMaximumLength = DeviceCommunicationBasics.DataMessageMaxPacketSize;
    }

    /// <summary>
    /// Decode a data message to an <see cref="IDataMessage"/> instance
    /// </summary>
    /// <param name="data">Data message bytes received</param>
    /// <returns>Decoding result</returns>
    public override InboundCodecResult DecodeDataMessage(Memory<byte> data)
    {

        var result = CheckExpectedLengths(data.Length);

        if (result.ErrorCode != 0)
        {
            return result;
        }

        try
        {

            IDataBlock dataBlock;

            // Extract header data from the byte array and store it to message properties if provided

            // Now get the delivered datablock

            var dataBlockBytes = data.Slice(1, data.Length - 1);

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



            var dataMessage = new SdcpDataMessage
            {
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
    public override OutboundCodecResult EncodeDataMessage(IDataMessage message)
    {
        var result = new OutboundCodecResult();
        if (message is not SdcpDataMessage tMessage)
        {
            result.ErrorMessage = "SdcpDataMessage required for SdcpDataMessageCodec";
            result.ErrorCode = 1;
            return result;
        }

        var data = new List<byte> { DeviceCommunicationBasics.Stx };

        // Add the datablock now if required
        try
        {
            if (tMessage.DataBlock == null)
            {
                result.ErrorMessage = "SdcpDataMessageCodec: datablock must not be null";
                result.ErrorCode = 5;
                return result;
            }
            DataBlockCodingProcessor.FromDataBlockToBytes(data, tMessage.DataBlock);
        }
        catch (Exception exception)
        {
            result.ErrorMessage = $"SdcpDataMessageCodec: exception raised during encoding: {exception}";
            result.ErrorCode = 4;
            return result;
        }

        // Add the final ETX now
        data.Add(DeviceCommunicationBasics.Etx);

        tMessage.RawMessageData = data.ToArray().AsMemory();

        return result;
    }
}