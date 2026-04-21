// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Text;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Codec to encode and decode device data messages for BTCP protocol
/// </summary>
public class BtcpDataMessageCodec : BaseDataMessageCodec
{
    private readonly Encoding _encoding = Encoding.UTF8;

    /// <summary>
    /// Current <see cref="IDataBlockCodingProcessor"/> instance
    /// </summary>
    public readonly IDataBlockCodingProcessor DataBlockCodingProcessor;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataBlockCodingProcessor">Current <see cref="IDataBlockCodingProcessor"/> instance</param>
    public BtcpDataMessageCodec(IDataBlockCodingProcessor dataBlockCodingProcessor)
    {
        DataBlockCodingProcessor = dataBlockCodingProcessor;

        ExpectedMinimumLength = 2;
        ExpectedMaximumLength = DeviceCommunicationBasics.DataMessageMaxPacketSize;
    }

    /// <summary>
    /// Decode a data message to an <see cref="IInboundDataMessage"/> instance
    /// </summary>
    /// <param name="data">Data message bytes received</param>
    /// <returns>Decoding result</returns>
    public override InboundCodecResult DecodeDataMessage(Memory<byte> data)
    {
        var result = BasicInboundChecks(data);

        if (result.ErrorCode != 0)
        {
            return result;
        }

        //try
        //{

        var isRequest = data.Slice(1, 1).Span[0] == 1;

        ITypedInboundDataBlock? dataBlock = null;

        var posEot = FindNextEot(data, 2);

        // Find business transaction ID
        var nArray = data.Slice(2, posEot - 2).ToArray();

        var s = _encoding.GetString(nArray);

        var bt = Convert.ToInt32(s);

        // Find UID of the BT
        var altPosEot = posEot + 1;
        posEot = FindNextEot(data, altPosEot);

        nArray = data.Slice(altPosEot, posEot - altPosEot).ToArray();

        s = _encoding.GetString(nArray);

        var uid = new Guid(s);

        // Datablock delivered?
        if (posEot != data.Length - 1)
        {
            // Now get the delivered datablock
            var dataBlockBytes = data.Slice(posEot + 1, data.Length - posEot - 2);

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
        }

        if (isRequest)
        {
            var dataMessage = new BtcpRequestInboundDataMessage(bt, uid)
            {
                DataBlock = dataBlock,
                RawMessageData = data
            };

            result.DataMessage = dataMessage;
        }
        else
        {
            var dataMessage = new BtcpReplyInboundDataMessage(bt, uid)
            {
                DataBlock = dataBlock,
                RawMessageData = data
            };

            if (dataBlock != null)
            {
                ExtractReplyData(dataBlock.Data, dataMessage);
            }

            result.DataMessage = dataMessage;
        }

        return result;

        //}
        //catch (Exception exception)
        //{
        //    result.ErrorMessage = $"DataMessage {DataMessageHelper.ByteArrayToString(data)}: decoding failed: {exception.Message}";
        //    result.ErrorCode = 5;
        //    return result;
        //}
    }

    private static void ExtractReplyData(Memory<byte> data, BtcpReplyInboundDataMessage dataMessage)
    {
        var pos = 0;
        for (var i = data.Length - 1; i >= 0; i--)
        {
            if (data.Slice(i, 1).Span[0] == 0x7c)
            {
                pos = i;
                break;
            }
        }

        if (pos == 0)
        {
            dataMessage.Payload = data;
            return;
        }

        dataMessage.Payload = data.Slice(pos + 1, data.Length - pos - 1);

        var bytes = data.Slice(0, pos);

        var s = Encoding.UTF8.GetString(bytes.Span);

        var paras = s.Split('|');

        if (paras.Length < 3)
        {
            return;
        }

        dataMessage.ErrorCode = Convert.ToInt32(paras[0]);
        dataMessage.InfoMessage = paras[1];
        dataMessage.ErrorMessage = paras[2];
    }

    private static int FindNextEot(Memory<byte> data, int startPos)
    {
        for (var i = startPos; i < data.Length; i++)
        {
            var b = data.Slice(i, 1).Span[0];
            if (b is DeviceCommunicationBasics.Eot or DeviceCommunicationBasics.Etx)
            {
                return i;
            }
        }

        return 0;
    }

    private InboundCodecResult BasicInboundChecks(Memory<byte> data)
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
        return result;
    }

    /// <summary>
    /// Encodes a message to a byte array to send to receiver
    /// </summary>
    /// <param name="message">Data message to send</param>
    /// <returns>Byte array as optimized <see cref="ReadOnlyMemory{T}"/> to send</returns>
    public override OutboundCodecResult EncodeDataMessage(IOutboundMessage message)
    {
        var result = new OutboundCodecResult();

        switch (message)
        {
            case BtcpRequestOutboundDataMessage request:
                EncodeRequest(request, result);
                break;
            case BtcpReplyOutboundDataMessage reply:
                EncodeReply(reply, result);
                break;
            default:
                result.ErrorMessage = "BtcpOutboundDataMessage required for BtcpDataMessageCodec";
                result.ErrorCode = 1;
                break;
        }

        return result;
    }

    private void EncodeReply(BtcpReplyOutboundDataMessage reply, OutboundCodecResult result)
    {
        var data = new List<byte>
        {
            DeviceCommunicationBasics.Stx,
            0
        };

        // Now add the business transaction ID
        var s = reply.BusinessTransactionId.ToString("###0");
        data.AddRange(_encoding.GetBytes(s));

        data.Add(DeviceCommunicationBasics.Eot);

        // Now add the business transaction UID
        s = reply.BusinessTransactionUid.ToString();
        data.AddRange(_encoding.GetBytes(s));

        // Add the datablock now if required
        try
        {
            data.Add(DeviceCommunicationBasics.Eot);

            // Now add the default reply
            var defaultReply = $"{reply.ErrorCode}|{reply.InfoMessage}|{reply.ErrorMessage}|";

            var bytes = _encoding.GetBytes(defaultReply);

            data.AddRange(bytes.ToList());

            // Now add payload
            if (reply.DataBlock != null)
            {
                DataBlockCodingProcessor.FromDataBlockToBytes(data, reply.DataBlock);
            }
        }
        catch (Exception exception)
        {
            result.ErrorMessage = $"BtcpDataMessageCodec: exception raised during encoding: {exception}";
            result.ErrorCode = 4;
        }

        // Add the final ETX now
        data.Add(DeviceCommunicationBasics.Etx);

        reply.RawMessageData = data.ToArray().AsMemory();
    }

    private void EncodeRequest(BtcpRequestOutboundDataMessage request, OutboundCodecResult result)
    {
        var data = new List<byte>
        {
            DeviceCommunicationBasics.Stx,
            1 
        };

        // Now add the business transaction ID
        var s = request.BusinessTransactionId.ToString("###0");
        data.AddRange(_encoding.GetBytes(s));

        data.Add(DeviceCommunicationBasics.Eot);

        // Now add the business transaction UID
        s = request.BusinessTransactionUid.ToString();
        data.AddRange(_encoding.GetBytes(s));

        // Add the datablock now if required
        //try
        //{
            if (request.DataBlock != null)
            {
                data.Add(DeviceCommunicationBasics.Eot);
                DataBlockCodingProcessor.FromDataBlockToBytes(data, request.DataBlock);
            }
        //}
        //catch (Exception exception)
        //{
        //    result.ErrorMessage = $"BtcpDataMessageCodec: exception raised during encoding: {exception}";
        //    result.ErrorCode = 4;
        //}

        // Add the final ETX now
        data.Add(DeviceCommunicationBasics.Etx);

        request.RawMessageData = data.ToArray().AsMemory();
    }
}