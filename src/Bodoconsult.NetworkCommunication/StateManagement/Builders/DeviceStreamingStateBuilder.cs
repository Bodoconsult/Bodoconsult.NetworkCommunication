// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DevicStreamingState
/// </summary>
public class DeviceStreamingStateBuilder : BaseNoActionStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceStopStreamingState, DefaultStateNames.DeviceOfflineState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceStreamingStateBuilder() : base(DefaultStateIds.DeviceStreamingState, DefaultStateNames.DeviceStreamingState)
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
        state.CheckJobstatesActionForStateDelegate =
            config.CheckJobstatesActionForStateDelegate ?? DelegateHelper.DefaultCheckJobstatesActionForStateDelegate;

        state.CancelStateDelegate = DelegateHelper.CancelStateDelegate;
        state.AllowedNextStates.AddRange(AllowedNextStatesInternal);
    }
}