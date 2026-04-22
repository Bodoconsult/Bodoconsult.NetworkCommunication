// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

/// <summary>
/// TNCP protocol implementation of <see cref="IDataMessageValidator"/>
/// </summary>
public class TncpDataMessageValidator : IDataMessageValidator
{
    /// <summary>
    /// Check if a data message is valid data message to be processed
    /// </summary>
    /// <param name="dataMessage">Received data message</param>
    /// <returns>True if the message was the handshake for the sent message</returns>
    public DataMessageValidatorResult IsMessageValid(IInboundMessage dataMessage)
    {
        // Handshake message: always valid
        if (dataMessage is IInboundHandShakeMessage)
        {
            return new DataMessageValidatorResult(true, "Message is valid");
        }

        // Update mode message or raw message: always valid
        if (dataMessage is RawInboundDataMessage)
        {
            return new DataMessageValidatorResult(true, "Message is valid");
        }

        // No TNCP data message: always valid
        if (dataMessage is not TncpInboundDataMessage)
        {
            return new DataMessageValidatorResult(false, "Message is NOT a valid TNCP message");
        }

        return new DataMessageValidatorResult(true, "Message is a valid TNCP message");
    }
}