// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageValidators;

/// <summary>
/// SFXP protocol implementation of <see cref="IDataMessageValidator"/>
/// </summary>
public class SfxpDataMessageValidator : IDataMessageValidator
{
    public DataMessageValidatorResult IsMessageValid(IInboundMessage dataMessage)
    {
        // Raw message is ok i.e. for client hello
        if (dataMessage is RawInboundDataMessage)
        {
            return new DataMessageValidatorResult(true, "Message is a valid SFXP message");
        }

        // No SFXP data message: always valid
        if (dataMessage is not SfxpInboundDataMessage bt)
        {
            return new DataMessageValidatorResult(false, "Message is NOT a valid SFXP message");
        }

        return bt.RawMessageData.Length switch
        {
            <= 0 => new DataMessageValidatorResult(false, "Message is NOT a valid SFXP message: too short"),
            > 16384 => new DataMessageValidatorResult(false, "Message is NOT a valid SFXP message: too long"),
            _ => new DataMessageValidatorResult(true, "Message is a valid SFXP message")
        };
    }
}