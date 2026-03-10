// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DeviceOnlineState
/// </summary>
public class DeviceOnlineStateBuilder : BaseOrderlessStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceReadyState, DefaultStateNames.DeviceOfflineState];

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

        state.ExecuteActionForStateDelegate = config.ExecuteActionForStateDelegate ?? ExecuteActionForStateDelegate;

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

    private static void ExecuteActionForStateDelegate(IOrderlessActionStateMachineState state)
    {
        if (state.CancellationTokenSource == null)
        {
            throw new ArgumentNullException(nameof(state.CancellationTokenSource));
        }

        var context = state.CurrentContext;
        var commAdapter = context.CommunicationAdapter;

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