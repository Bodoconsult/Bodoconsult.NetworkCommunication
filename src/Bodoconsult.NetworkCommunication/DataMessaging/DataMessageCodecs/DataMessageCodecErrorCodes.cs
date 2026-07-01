// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageCodecs;

/// <summary>
/// Important error codes for data message coding
/// </summary>
public class DataMessageCodecErrorCodes
{
    /// <summary>
    /// Message has been received already
    /// </summary>
    public const int MessageAlreadyReceived = 6;

    /// <summary>
    /// Message has no data chunks
    /// </summary>
    public const int MessageHasNoDataChunks = 7;
}