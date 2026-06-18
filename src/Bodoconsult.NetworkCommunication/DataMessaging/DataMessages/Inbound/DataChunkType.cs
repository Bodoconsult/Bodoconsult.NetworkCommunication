// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Enum with types of data chunk
/// </summary>
public enum DataChunkType
{
    /// <summary>
    /// Normal data chunk
    /// </summary>
    DataChunk,

    /// <summary>
    /// 0x0 sync byte chunk
    /// </summary>
    RegularSyncChunk,

    /// <summary>
    /// 0x0 sync byte chunk transporting a part of a uint64 sample counter
    /// </summary>
    SampleCounterSyncChunk
}