// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

public class DeviceOfflineStateBuilder : BaseOrderlessStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceOnlineState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceOfflineStateBuilder() : base(DefaultStateIds.DeviceOfflineState, DefaultStateNames.DeviceOfflineState)
    { }

    /// <summary>
    /// Configure an orderless action state
    /// </summary>
    /// <param name="state">Current <see cref="IOrderlessActionStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public override void ConfigureOrderlessActionState(IOrderlessActionStateMachineState state, IOrderlessActionStateConfiguration config)
    {
        state.InitialDeviceState = DefaultDeviceStates.DeviceStateOffline;
        state.InitialBusinessSubState = DefaultBusinessSubStates.NotSet;

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

        // Wait until the device is pingable
        while (!state.CancellationTokenSource.IsCancellationRequested)
        {
            context.SetBusinessSubState(DefaultBusinessSubStates.PingingTower);
            if (context.IsPingable)
            {
                break;
            }

            context.SetBusinessSubState(DefaultBusinessSubStates.WaitingForNextPingingTower);
            Thread.Sleep(DeviceCommunicationBasics.PingRepeatInterval);
        }

        // New state now
        var stateNew = context.CreateStateInstance(DefaultStateNames.DeviceOnlineState);
        state.NextState = stateNew;
        state.RequestNextState();
    }
}