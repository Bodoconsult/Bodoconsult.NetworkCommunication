// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DeviceStopStreamingState
/// </summary>
public class DeviceStopStreamingStateBuilder : BaseOrderBasedStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceReadyState, DefaultStateNames.DeviceOfflineState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceStopStreamingStateBuilder() : base(DefaultStateIds.DeviceStopStreamingState, DefaultStateNames.DeviceStopStreamingState)
    { }


    public override void ConfigureOrderBasedActionState(IOrderBasedActionStateMachineState state, IOrderBasedActionStateConfiguration config)
    {
        state.InitialDeviceState = DefaultDeviceStates.DeviceStateOnline;
        state.InitialBusinessSubState = DefaultBusinessSubStates.TryToConnect;
        state.PrepareOrdersForStateMachineStateDelegate = config.PrepareOrdersForStateMachineStateDelegate;
        state.OrderFinishedSucessfullyDelegate = config.OrderFinishedSucessfullyDelegate;
        state.OrderFinishedUnsucessfullyDelegate = config.OrderFinishedUnsucessfullyDelegate;

        state.IsRunningOrdersCancellationRequired = true;
        state.IsTurningOffStateRequestsRequired = true;
        state.CancellationTokenSource = new CancellationTokenSource();
        state.CancelStateDelegate = CancelStateDelegate;

        state.AllowedNextStates.AddRange(AllowedNextStatesInternal);
    }

    private static void CancelStateDelegate(IStateMachineState state)
    {
        state.CancellationTokenSource?.Cancel();
    }
}