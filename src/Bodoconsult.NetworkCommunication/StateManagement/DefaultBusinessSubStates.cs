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
    public static BusinessSubState NotSet =
        new BusinessSubState(0, "Not set");

    /// <summary>
    /// Pinging the tower
    /// </summary>
    public static BusinessSubState PingingTower =
        new BusinessSubState(1, "Pinging the tower");

    /// <summary>
    /// Pinging the tower
    /// </summary>
    public static BusinessSubState WaitingForNextPingingTower =
        new BusinessSubState(2, "Waiting for the next pinging the tower");
}