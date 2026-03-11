// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DevicReadyState
/// </summary>
public class DeviceReadyStateBuilder : BaseNoActionStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceStartStreamingState, DefaultStateNames.DeviceOfflineState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceReadyStateBuilder() : base(DefaultStateIds.DeviceReadyState, DefaultStateNames.DeviceReadyState)
    { }

    /// <summary>
    /// Configure a no action state
    /// </summary>
    /// <param name="noActionState">Current <see cref="INoActionStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public override void ConfigureNoActionState(INoActionStateMachineState noActionState, INoActionStateConfiguration config)
    {
        noActionState.InitialDeviceState = DefaultDeviceStates.DeviceStateReady;
        noActionState.InitialBusinessSubState = DefaultBusinessSubStates.NotSet;

        noActionState.CheckJobstatesActionForStateDelegate = config.CheckJobstatesActionForStateDelegate ??
                                                             DelegateHelper.DefaultCheckJobstatesActionForStateDelegate;

        noActionState.CancelStateDelegate = DelegateHelper.CancelStateDelegate;

        noActionState.AllowedNextStates.AddRange(AllowedNextStatesInternal);
    }
}