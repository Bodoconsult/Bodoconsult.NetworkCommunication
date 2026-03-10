// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

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
                                                             CheckJobstatesActionForStateDelegate;

        noActionState.CancelStateDelegate = CancelStateDelegate;

        noActionState.AllowedNextStates.AddRange(AllowedNextStatesInternal);
    }

    private static void CheckJobstatesActionForStateDelegate(INoActionStateMachineState currentState)
    {
        while (!currentState.CancellationTokenSource.IsCancellationRequested)
        {
            // Check if there is a job state to restore after break
            if (currentState.CurrentContext.SavedJobState != null)
            {
                currentState.CurrentContext.RestoreSavedState();
                return;
            }

            // Check if a job state is waiting. If yes, process it now
            if (currentState.CurrentContext.JobStates.Count > 0)
            {
                // Get the first job state and process it
                var state = currentState.CurrentContext.JobStates.First();

                currentState.CurrentContext.JobStates.Remove(state);

                currentState.CurrentContext.RequestState(state);

                currentState.CancellationTokenSource.Dispose();
                return;
            }

            // Wait a bit then check again
            Thread.Sleep(DeviceCommunicationBasics.JobStateCheckTimeout);
        }
    }

    private static void CancelStateDelegate(IOrderlessActionStateMachineState state)
    {
        state.CancellationTokenSource?.Cancel();
    }
}