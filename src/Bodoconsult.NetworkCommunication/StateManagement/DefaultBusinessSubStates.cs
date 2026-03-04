// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Default business substates
/// </summary>
public static class DefaultBusinessSubStates
{
    /// <summary>
    ///  Not set substate
    /// </summary>
    public static IBusinessSubState NotSet { get; } = new BusinessSubState(0, "NotSet");
}