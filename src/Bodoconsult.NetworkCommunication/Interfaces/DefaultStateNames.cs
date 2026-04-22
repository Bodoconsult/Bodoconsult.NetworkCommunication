// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Default state names
/// </summary>
public static class DefaultStateNames
{
    /// <summary>
    /// Device state offline
    /// </summary>
    public const string DeviceOfflineState = "DeviceOfflineState";
    /// <summary>
    /// Device state online
    /// </summary>
    public const string DeviceOnlineState = "DeviceOnlineState";
    /// <summary>
    /// Device state init
    /// </summary>
    public const string DeviceInitState = "DeviceInitState";

    /// <summary>
    /// Device state ready
    /// </summary>
    public const string DeviceReadyState = "DeviceReadyState";

    /// <summary>
    /// Device state start streaming
    /// </summary>
    public const string DeviceStartStreamingState = "DeviceStartStreamingState";

    /// <summary>
    /// Device state streaming is active
    /// </summary>
    public const string DeviceStreamingState = "DeviceStreamingState";

    /// <summary>
    /// Device state stop streaming
    /// </summary>
    public const string DeviceStopStreamingState = "DeviceStopStreamingState";

    /// <summary>
    /// Device state start snapshot
    /// </summary>
    public const string DeviceStartSnapshotState = "DeviceStartSnapshotState";

    /// <summary>
    /// Device state snapshot is active
    /// </summary>
    public const string DeviceSnapshotState = "DeviceSnapshotState";

    /// <summary>
    /// Device state stop snapshot
    /// </summary>
    public const string DeviceStopSnapshotState = "DeviceStopSnapshotState";
}

/// <summary>
/// Default state IDs
/// </summary>
public static class DefaultStateIds
{
    /// <summary>
    /// ID of the device state offline
    /// </summary>
    public const int DeviceOfflineState = 1;

    /// <summary>
    /// ID of the device state online
    /// </summary>
    public const int DeviceOnlineState = 2;

    /// <summary>
    /// ID of the device state init
    /// </summary>
    public const int DeviceInitState = 3;

    /// <summary>
    /// ID of the device state ready
    /// </summary>
    public const int DeviceReadyState = 4;

    /// <summary>
    /// ID of the device state start streaming
    /// </summary>
    public const int DeviceStartStreamingState = 5;

    /// <summary>
    /// ID of the device state streaming ia active
    /// </summary>
    public const int DeviceStreamingState = 6;

    /// <summary>
    /// ID of the device state stop streaming
    /// </summary>
    public const int DeviceStopStreamingState = 7;

    /// <summary>
    /// ID of the device state start snapshot
    /// </summary>
    public const int DeviceStartSnapshotState = 8;

    /// <summary>
    /// ID of the device state snapshot is active
    /// </summary>
    public const int DeviceSnapshotState = 9;

    /// <summary>
    /// ID of the device state stop snapshot
    /// </summary>
    public const int DeviceStopSnapshotState = 10;
}