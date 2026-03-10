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
    public static BusinessSubState PingingTower = new(1, "Pinging the tower");

    /// <summary>
    /// Pinging the tower
    /// </summary>
    public static BusinessSubState WaitingForNextPingingTower = new(2, "Waiting for the next pinging the tower");

    /// <summary>
    /// Try to connect
    /// </summary>
    public static BusinessSubState TryToConnect = new(0, "Try to connect");

    /// <summary>
    /// Device connected
    /// </summary>
    public static BusinessSubState Connected = new(0, "Connected");
}