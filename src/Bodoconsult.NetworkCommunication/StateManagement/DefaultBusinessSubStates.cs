// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// StSys SMD tower business sub states
/// </summary>
public static class DefaultBusinessSubStates
{
    /// <summary>
    /// Not set
    /// </summary>
    public static BusinessSubState NotSet = new(0, "Not set");

    /// <summary>
    /// Pinging the tower
    /// </summary>
    public static BusinessSubState PingingTower = new(1, "Pinging the device");

    /// <summary>
    /// Pinging the tower
    /// </summary>
    public static BusinessSubState WaitingForNextPingingTower = new(2, "Waiting for the next pinging the device");

    /// <summary>
    /// Try to connect
    /// </summary>
    public static BusinessSubState TryToConnect = new(3, "Try to connect");

    /// <summary>
    /// Device connected
    /// </summary>
    public static BusinessSubState Connected = new(4, "Connected");

    /// <summary>
    /// Try to stop messaging
    /// </summary>
    public static BusinessSubState TryToStopMessaging = new(5, "Try to stop messaging");

    /// <summary>
    /// Try to stop messaging
    /// </summary>
    public static BusinessSubState TryToStartMessaging = new(6, "Try to start messaging");
}