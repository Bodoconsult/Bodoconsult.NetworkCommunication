// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

/// <summary>
/// EDCP protocol implementation of <see cref="IDataMessageValidator"/>
/// </summary>
public class EdcpDataMessageValidator : IDataMessageValidator
{
    public DataMessageValidatorResult IsMessageValid(IInboundDataMessage dataMessage)
    {
        // Update mode message or raw message: always valid
        if (dataMessage is RawInboundDataMessage)
        {
            return new DataMessageValidatorResult(true, "Message is valid");
        }

        // No SDCP data message: always valid
        if (dataMessage is not EdcpInboundDataMessage)
        {
            return new DataMessageValidatorResult(false, "Message is NOT a valid EDCP message");
        }

        return new DataMessageValidatorResult(true, "Message is a valid EDCP message");
    }
}