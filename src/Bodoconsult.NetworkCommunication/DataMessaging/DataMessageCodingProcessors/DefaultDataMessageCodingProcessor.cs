// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Exceptions;
using Bodoconsult.NetworkCommunication.Interfaces;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftAntimalwareEngine;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodingProcessors;

/// <summary>
/// Default implementation of <see cref=" IDataMessageCodingProcessor"/>
/// </summary>
public class DefaultDataMessageCodingProcessor : IDataMessageCodingProcessor
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="logger">Current logger</param>
    public DefaultDataMessageCodingProcessor(IAppLoggerProxy logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Current logger
    /// </summary>
    public IAppLoggerProxy Logger { get; }

    /// <summary>
    /// Should an acknowledgement be sent if a request message is received
    /// </summary>
    public bool AnswerWithAcknowledgement { get; set; }

    /// <summary>
    /// Is waiting for acknowledgement by the device required for the request message
    /// </summary>
    public bool WaitForAcknowledgement { get; set; }

    /// <summary>
    /// All loaded message codecs
    /// </summary>
    public IList<IDataMessageCodec> MessageCodecs { get; } = new List<IDataMessageCodec>();

    /// <summary>
    /// Add a message codex
    /// </summary>
    /// <param name="codec">Codec to add</param>
    public void AddMessageCodec(IDataMessageCodec codec)
    {
        codec.AnswerWithAcknowledgement = AnswerWithAcknowledgement;
        codec.WaitForAcknowledgement = WaitForAcknowledgement;
        MessageCodecs.Add(codec);
    }

    /// <summary>
    /// Decode a data message
    /// </summary>
    /// <param name="data">Byte array with message data</param>
    /// <returns>Coding result with a <see cref="IInboundDataMessage"/> instance if coding was successful</returns>
    public InboundCodecResult DecodeDataMessage(Memory<byte> data)
    {
        try
        {
            foreach (var codec in MessageCodecs)
            {
                var result = codec.DecodeDataMessage(data);

                if (result.ErrorCode == 0 || result.ErrorCode == DataMessageCodecErrorCodes.MessageAlreadyReceived)
                {
                    return result;
                }

                Logger.LogDebug($"DecodeDataMessage: {result.ErrorCode} {result.ErrorMessage}");
            }

            if (MessageCodecs.Count == 0)
            {
                return new InboundCodecResult
                {
                    ErrorCode = 1,
                    ErrorMessage = "No codecs loaded"
                };
            }

            return new InboundCodecResult
            {
                ErrorCode = 2,
                ErrorMessage = "No codecs found for the message"
            };
        }
        catch (Exception e)
        {
            throw new DatablockCodecException("Decoding error", e);
        }
    }

    /// <summary>
    /// Encode handshake messages to send to device
    /// </summary>
    /// <param name="dataMessage">Data message to encode</param>
    /// <returns>A result set with the message as byte array </returns>
    public OutboundCodecResult EncodeDataMessage(IOutboundMessage dataMessage)
    {
        try
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
                    Logger.LogError(s);
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
        catch (Exception e)
        {
            throw new DatablockCodecException("Encoding error", e);
        }
    }
}