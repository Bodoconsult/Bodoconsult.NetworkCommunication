// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Helpers;

/// <summary>
/// Helper class for SFXP protocol
/// </summary>
public static class SfxpProtocolHelper
{
    /// <summary>
    ///  The regular sync byte definition
    /// </summary>
    public static SyncByteDefinition RegularSyncByte { get; } = new()
    {
        Length = 8,
        SyncByte = 0x0
    };

    /// <summary>
    /// The sync byte block definition for the samplecounters
    /// </summary>
    public static SyncByteDefinition SampleCounterSyncByteBlock { get; } = new()
    {
        Length = 8,
        SyncByte = 0x9
    };

    /// <summary>
    /// Maximum message length to be sent
    /// </summary>
    public static int MaximumMessageLength { get; set; } = 16384;

    /// <summary>
    /// The length of a data chunk
    /// </summary>
    public static int DataChunkLength { get; set; } = 8;

    /// <summary>
    /// The number of data chunks sent before the enxt sync byte is sent
    /// </summary>
    public static int NumberOfChunksBeforeSyncByteIsSent { get; set; } = 252;
}