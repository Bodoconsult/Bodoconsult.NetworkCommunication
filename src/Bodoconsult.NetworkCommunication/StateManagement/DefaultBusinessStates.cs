// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement;

public static class DefaultBusinessStates
{
    /// <summary>
    ///  Not set state
    /// </summary>
    public static IBusinessState Offline { get; } = new BusinessState(1, "Offline");

    /// <summary>
    ///  Online state
    /// </summary>
    public static IBusinessState Online { get; } = new BusinessState(1, "Online");

    /// <summary>
    ///  Ready state
    /// </summary>
    public static IBusinessState Ready { get; } = new BusinessState(1, "Ready");
}