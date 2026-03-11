// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Default state names
/// </summary>
public static class DefaultStateNames
{
    public const string DeviceOfflineState = "DeviceOfflineState";
    public const string DeviceOnlineState = "DeviceOnlineState";
    public const string DeviceReadyState = "DeviceReadyState";
    public const string DeviceStartStreamingState = "DeviceStartStreamingState";
    public const string DeviceStreamingState = "DeviceStreamingState";
    public const string DeviceStopStreamingState = "DeviceStopStreamingState";
    public const string DeviceStartSnapshotState = "DeviceStartSnapshotState";
    public const string DeviceSnapshotState = "DeviceSnapshotState";
    public const string DeviceStopSnapshotState = "DeviceStopSnapshotState";
}

/// <summary>
/// Default state IDs
/// </summary>
public static class DefaultStateIds
{
    public const int DeviceOfflineState =1;
    public const int  DeviceOnlineState = 2;
    public const int DeviceReadyState = 3;
    public const int DeviceStartStreamingState = 4;
    public const int DeviceStreamingState = 5;
    public const int DeviceStopStreamingState = 6;    
    public const int DeviceStartSnapshotState = 7;
    public const int DeviceSnapshotState = 8;
    public const int DeviceStopSnapshotState = 9;
}