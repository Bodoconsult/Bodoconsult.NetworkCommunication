// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

/// <summary>
/// BTCP protocol implementation of <see cref="IDataMessageValidator"/>
/// </summary>
public class BtcpDataMessageValidator : IDataMessageValidator
{
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

        // No SDCP data message: always valid
        if (dataMessage is not BtcpRequestInboundDataMessage bt)
        {
            return new DataMessageValidatorResult(false, "Message is NOT a valid BTCP message");
        }

        if (bt.BusinessTransactionId == 0)
        {
            return new DataMessageValidatorResult(false, "Message is NOT a valid BTCP message (ID=0)");
        }

        if (bt.BusinessTransactionUid == Guid.Empty)
        {
            return new DataMessageValidatorResult(false, "Message is NOT a valid BTCP message (UID=Guid.Empty)");
        }

        return new DataMessageValidatorResult(true, "Message is a valid BTCP message");
    }
}