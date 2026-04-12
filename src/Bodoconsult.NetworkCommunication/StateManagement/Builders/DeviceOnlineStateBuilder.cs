// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DeviceOnlineState
/// </summary>
public class DeviceOnlineStateBuilder : BaseOrderlessStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceInitState, DefaultStateNames.DeviceOfflineState, DefaultStateNames.DeviceReadyState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceOnlineStateBuilder() : base(DefaultStateIds.DeviceOnlineState, DefaultStateNames.DeviceOnlineState)
    { }

    /// <summary>
    /// Configure a no action state
    /// </summary>
    /// <param name="state">Current <see cref="IOrderlessActionStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public override void ConfigureOrderlessActionState(IOrderlessActionStateMachineState state, IOrderlessActionStateConfiguration config)
    {
        state.InitialDeviceState = DefaultDeviceStates.DeviceStateOnline;
        state.InitialBusinessSubState = DefaultBusinessSubStates.TryToConnect;

        state.HandleAsyncMessageDelegate = config.HandleAsyncMessageDelegate;
        state.HandleComDevCloseDelegate = config.HandleComDevCloseDelegate;
        state.HandleErrorMessageDelegate = config.HandleErrorMessageDelegate;
        state.HandleRegularStateRequestAnswerDelegate = config.HandleRegularStateRequestAnswerDelegate;
        state.PrepareRegularStateRequestDelegate = config.PrepareRegularStateRequestDelegate;

        state.ExecuteActionForStateDelegate = config.ExecuteActionForStateDelegate ?? ExecuteActionForStateDelegate;

        state.IsRunningOrdersCancellationRequired = true;
        state.IsTurningOffStateRequestsRequired = true;
        state.CancellationTokenSource = new CancellationTokenSource();
        state.CancelStateDelegate = DelegateHelper.CancelStateDelegate;

        state.AllowedNextStates.AddRange(AllowedNextStatesInternal);
    }

    private static void ExecuteActionForStateDelegate(IOrderlessActionStateMachineState state)
    {
        ArgumentNullException.ThrowIfNull(state.CancellationTokenSource);

        //ArgumentNullException.ThrowIfNull(IpDeviceTcpIp?.IpDevice);
        //IpDeviceTcpIp.IpDevice.StartComm();

        var context = state.CurrentContext;
        var commAdapter = context.CommunicationAdapter;

        ArgumentNullException.ThrowIfNull(commAdapter);

        // Try to connect to device
        var nextState = DefaultStateNames.DeviceReadyState;

        if (!commAdapter.ComDevInit())
        {
            nextState = DefaultStateNames.DeviceOfflineState;
        }
        else
        {
            context.SetBusinessSubState(DefaultBusinessSubStates.Connected);
        }

        // New state now
        var stateNew = context.CreateStateInstance(nextState);
        state.NextState = stateNew;
        state.RequestNextState();
    }
}