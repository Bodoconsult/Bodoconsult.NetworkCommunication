// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Diagnostics;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodingProcessors;

/// <summary>
/// Default implementation of <see cref=" IDataMessageCodingProcessor"/>
/// </summary>
public class DefaultDataMessageCodingProcessor : IDataMessageCodingProcessor
{
    /// <summary>
    /// All loaded message codecs
    /// </summary>
    public IList<IDataMessageCodec> MessageCodecs { get; } = new List<IDataMessageCodec>();

    /// <summary>
    /// Decode a data message
    /// </summary>
    /// <param name="data">Byte array with message data</param>
    /// <returns>Coding result with a <see cref="IInboundDataMessage"/> instance if coding was successful</returns>
    public InboundCodecResult DecodeDataMessage(Memory<byte> data)
    {
        if (MessageCodecs.Count == 0)
        {
            return new InboundCodecResult
            {
                ErrorCode = 1,
                ErrorMessage = "No codecs loaded"
            };
        }

        foreach (var codec in MessageCodecs)
        {
            var result = codec.DecodeDataMessage(data);

            if (result.ErrorCode == 0)
            {
                return result;
            }
        }

        return new InboundCodecResult
        {
            ErrorCode = 2,
            ErrorMessage = "No codecs found for the message"
        };
    }

    /// <summary>
    /// Encode handshake messages to send to device
    /// </summary>
    /// <param name="dataMessage">Data message to encode</param>
    /// <returns>A result set with the message as byte array </returns>
    public OutboundCodecResult EncodeDataMessage(IOutboundMessage dataMessage)
    {
        if (dataMessage is RawOutboundDataMessage raw)
        {
            if (raw.RawMessageData.Length == 0)
            {
                return new OutboundCodecResult
                {
                    ErrorCode = 2,
                    ErrorMessage = "raw.RawMessageData.Length is zero"
                };
            }

            return new OutboundCodecResult
            {
                ErrorCode = 0
            };
        }


        if (MessageCodecs.Count == 0)
        {
            return new OutboundCodecResult
            {
                ErrorCode = 1,
                ErrorMessage = "No codecs loaded"
            };
        }

        foreach (var codec in MessageCodecs)
        {
            try
            {
                var result = codec.EncodeDataMessage(dataMessage);

                if (result.ErrorCode == 0)
                {
                    return result;
                }
            }
            catch (Exception e)
            {
                var s = e.ToString();
                Trace.TraceError(s);
                return new OutboundCodecResult
                {
                    ErrorCode = 1,
                    ErrorMessage = $"Exception{s}"
                };
            }
        }

        return new OutboundCodecResult
            {
                ErrorCode = 1,
                ErrorMessage = "No codecs found for the message"
            };
        }
    }