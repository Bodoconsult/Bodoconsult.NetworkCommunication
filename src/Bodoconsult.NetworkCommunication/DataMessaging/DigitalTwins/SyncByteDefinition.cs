// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.DataMessaging.DigitalTwins;

/// <summary>
/// Definition for sync bytes and sync byte blocks
/// </summary>
public struct SyncByteDefinition
{
    /// <summary>
    /// First byte of the sync byte
    /// </summary>
    public byte SyncByte { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Length { get; set; }
}