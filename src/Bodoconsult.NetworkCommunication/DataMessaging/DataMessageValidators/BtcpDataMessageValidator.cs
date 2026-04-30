// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

/// <summary>
/// BTCP protocol implementation of <see cref="IDataMessageValidator"/>
/// </summary>
public class BtcpDataMessageValidator : IDataMessageValidator
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

        // BTCP reply?
        if (dataMessage is BtcpReplyInboundDataMessage br)
        {
            // Now check reply details
            if (br.BusinessTransactionId == 0)
            {
                return new DataMessageValidatorResult(false, "Message is NOT a valid BTCP reply message (ID=0)");
            }

            if (br.BusinessTransactionUid == Guid.Empty)
            {
                return new DataMessageValidatorResult(false, "Message is NOT a valid BTCP reply message (UID=Guid.Empty)");
            }

            return new DataMessageValidatorResult(true, "Message is a valid BTCP reply message");
        }


        // No BTCP request data message: leave now
        if (dataMessage is not BtcpRequestInboundDataMessage bt)
        {
            return new DataMessageValidatorResult(false, "Message is NOT a valid BTCP message");
        }

        // Now check request details
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