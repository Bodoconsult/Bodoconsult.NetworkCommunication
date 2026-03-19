// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DeviceStopSnapshotState
/// </summary>
public class DeviceStopSnapshotStateBuilder : BaseJobStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceSnapshotState, DefaultStateNames.DeviceOfflineState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceStopSnapshotStateBuilder() : base(DefaultStateIds.DeviceStopSnapshotState, DefaultStateNames.DeviceStopSnapshotState)
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

        state.StateNameOnSuccess = DefaultStateNames.DeviceReadyState;
    }
}