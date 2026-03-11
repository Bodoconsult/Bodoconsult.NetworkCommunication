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
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceStartStreamingState, DefaultStateNames.DeviceStartSnapshotState, DefaultStateNames.DeviceOfflineState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceReadyStateBuilder() : base(DefaultStateIds.DeviceReadyState, DefaultStateNames.DeviceReadyState)
    { }

    /// <summary>
    /// Configure a no action state
    /// </summary>
    /// <param name="state">Current <see cref="INoActionStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public override void ConfigureNoActionState(INoActionStateMachineState state, INoActionStateConfiguration config)
    {
        state.InitialDeviceState = DefaultDeviceStates.DeviceStateReady;
        state.InitialBusinessSubState = DefaultBusinessSubStates.NotSet;

        state.HandleAsyncMessageDelegate = config.HandleAsyncMessageDelegate;
        state.HandleComDevCloseDelegate = config.HandleComDevCloseDelegate;
        state.HandleErrorMessageDelegate = config.HandleErrorMessageDelegate;
        state.HandleRegularStateRequestAnswerDelegate = config.HandleRegularStateRequestAnswerDelegate;
        state.PrepareRegularStateRequestDelegate = config.PrepareRegularStateRequestDelegate;

        state.CheckJobstatesActionForStateDelegate = config.CheckJobstatesActionForStateDelegate ??
                                                             DelegateHelper.DefaultCheckJobstatesActionForStateDelegate;

        state.CancelStateDelegate = DelegateHelper.CancelStateDelegate;

        state.AllowedNextStates.AddRange(AllowedNextStatesInternal);
    }
}