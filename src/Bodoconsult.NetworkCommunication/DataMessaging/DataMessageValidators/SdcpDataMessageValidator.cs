// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

/// <summary>
/// SDCP protocol implementation of <see cref="IDataMessageValidator"/>
/// </summary>
public class SdcpDataMessageValidator : IDataMessageValidator
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

        // Raw message: always valid
        if (dataMessage is RawInboundDataMessage)
        {
            return new DataMessageValidatorResult(true, "Message is valid");
        }

        // No SDCP data message: always valid
        if (dataMessage is not SdcpInboundDataMessage)
        {
            return new DataMessageValidatorResult(false, "Message is NOT a valid SDCP message");
        }

        return new DataMessageValidatorResult(true, "Message is a valid SDCP message");
    }
}