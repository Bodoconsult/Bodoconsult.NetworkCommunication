// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

/// <summary>
/// TNCP protocol implementation of <see cref="IDataMessageValidator"/>
/// </summary>
public class TncpDataMessageValidator : IDataMessageValidator
{
    public DataMessageValidatorResult IsMessageValid(IInboundMessage dataMessage)
    {
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