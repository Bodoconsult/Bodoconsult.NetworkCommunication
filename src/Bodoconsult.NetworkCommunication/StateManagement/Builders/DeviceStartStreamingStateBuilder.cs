// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DeviceStartStreamingState
/// </summary>
public class DeviceStartStreamingStateBuilder : BaseJobStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceStreamingState, DefaultStateNames.DeviceOfflineState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceStartStreamingStateBuilder() : base(DefaultStateIds.DeviceStartStreamingState, DefaultStateNames.DeviceStartStreamingState)
    { }

    /// <summary>
    /// Configure a no action state
    /// </summary>
    /// <param name="state">Current <see cref="IJobStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public override void ConfigureOrderBasedActionState(IJobStateMachineState state, IJobStateConfiguration config)
    {
        state.InitialDeviceState = DefaultDeviceStates.DeviceStateOnline;
        state.InitialBusinessSubState = DefaultBusinessSubStates.TryToConnect;

        state.HandleAsyncMessageDelegate = config.HandleAsyncMessageDelegate;
        state.HandleComDevCloseDelegate = config.HandleComDevCloseDelegate;
        state.HandleErrorMessageDelegate = config.HandleErrorMessageDelegate;
        state.HandleRegularStateRequestAnswerDelegate = config.HandleRegularStateRequestAnswerDelegate;
        state.PrepareRegularStateRequestDelegate = config.PrepareRegularStateRequestDelegate;
        state.OrderFinishedSucessfullyDelegate = config.OrderFinishedSucessfullyDelegate;
        state.OrderFinishedUnsucessfullyDelegate = config.OrderFinishedUnsucessfullyDelegate;

        state.IsRunningOrdersCancellationRequired = true;
        state.IsTurningOffStateRequestsRequired = true;
        state.CancellationTokenSource = new CancellationTokenSource();
        state.CancelStateDelegate = DelegateHelper.CancelStateDelegate;

        state.AllowedNextStates.AddRange(AllowedNextStatesInternal);

        state.StateNameOnSuccess = DefaultStateNames.DeviceStreamingState;
    }
}