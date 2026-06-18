// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Data chunk
/// </summary>
public class DataChunk: IResetable
{
    /// <summary>
    /// Channel not set identifier
    /// </summary>
    public const byte Identifier = 0xFF;

    /// <summary>
    /// Channele the data chunk is bound to
    /// </summary>
    public byte Channel { get; set; } = 0xFF;

    /// <summary>
    /// Data of the data chunk
    /// </summary>
    public Memory<byte>? Data { get; set; }

    /// <summary>
    /// Type of data chunk
    /// </summary>
    public DataChunkType DataChunkType { get; set; } = DataChunkType.DataChunk;

    /// <summary>
    /// Delegate for returning a <see cref="DataChunk"/> instance to the pool
    /// </summary>
    public ReturnDataChunkDelegate? ReturnDataChunkDelegate { get; set; }

    /// <summary>Reset the class to default values</summary>
    public void Reset()
    {
        Channel = 0xFF;
        Data = null;
        DataChunkType = DataChunkType.DataChunk;
    }
}