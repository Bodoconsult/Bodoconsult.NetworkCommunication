// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DeviceStartStreamingState
/// </summary>
public class DeviceStartStreamingStateBuilder : BaseOrderBasedStateMachineStateBuilder
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
        state.CancelStateDelegate = DelegateHelper.CancelStateDelegate;

        state.AllowedNextStates.AddRange(AllowedNextStatesInternal);
    }
}