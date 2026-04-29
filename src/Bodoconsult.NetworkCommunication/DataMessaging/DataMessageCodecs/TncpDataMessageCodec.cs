// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Codec to encode and decode device data messages for TNCP protocol
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

        ExpectedMinimumLength = 1;
        ExpectedMaximumLength = DeviceCommunicationBasics.DataMessageMaxPacketSize;
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

            if (data.Length <= 2)
            {
                result.ErrorMessage = "DataBlock decoding failed: no data";
                result.ErrorCode = 3;
                return result;
            }

            // Now get the delivered datablock
            var dataBlockBytes = new byte[data.Length];

            dataBlockBytes[0] = 0x78;

            for (var i = 1; i < dataBlockBytes.Length; i++)
            {
                dataBlockBytes[i] = data.Slice(i- 1, 1).Span[0];
            }

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
                RawMessageData = data
            };

            if (dataBlock != null)
            {
                dataMessage.TelnetCommand = Encoding.UTF8.GetString(dataBlock.Data.Span);
            }

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
            result.ErrorMessage = "TncpOutboundDataMessage required for TncpDataMessageCodec";
            result.ErrorCode = 1;
            return result;
        }

        var data = new List<byte>();

        // Add the TelnetCommand or the datablock
        try
        {
            if (string.IsNullOrEmpty(tMessage.TelnetCommand))
            {
                if (tMessage.DataBlock is TncpParameterSet ps)
                {
                    if (string.IsNullOrEmpty(ps.TelnetCommand))
                    {
                        result.ErrorMessage = "TncpDataMessageCodec: TncpParameterSet.TelnetCommand must not be empty";
                        result.ErrorCode = 6;
                        return result;
                    }
                    var bytes = Encoding.UTF8.GetBytes(ps.TelnetCommand!).ToList();
                    data.AddRange(bytes);
                }
                else
                {
                    if (tMessage.DataBlock == null || tMessage.DataBlock.Data.Length == 0)
                    {
                        result.ErrorMessage = "TncpDataMessageCodec: datablock or TelnetCommand must not be empty";
                        result.ErrorCode = 5;
                        return result;
                    }

                    data.AddRange(tMessage.DataBlock.Data.Span);
                }
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(tMessage.TelnetCommand).ToList();
                data.AddRange(bytes);
            }
        }
        catch (Exception exception)
        {
            result.ErrorMessage = $"SdcpDataMessageCodec: exception raised during encoding: {exception}";
            result.ErrorCode = 4;
            return result;
        }

        // Add the final CR now
        data.Add(DeviceCommunicationBasics.Lf);

        tMessage.RawMessageData = data.ToArray().AsMemory();

        return result;
    }
}