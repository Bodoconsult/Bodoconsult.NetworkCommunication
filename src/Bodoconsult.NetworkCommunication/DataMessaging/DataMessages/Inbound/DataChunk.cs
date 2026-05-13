// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Data chunk
/// </summary>
public struct DataChunk
{
    /// <summary>
    /// Channele the data chunk is bound to
    /// </summary>
    public byte Channel { get; set; }

    /// <summary>
    /// Data of the data chunk
    /// </summary>
    public Memory<byte>? Data { get; set; }
}