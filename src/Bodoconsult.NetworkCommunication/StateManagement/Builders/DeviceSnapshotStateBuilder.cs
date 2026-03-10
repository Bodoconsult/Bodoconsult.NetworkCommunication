// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DevicSnapshotState
/// </summary>
public class DeviceSnapshotStateBuilder : BaseNoActionStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceOfflineState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceSnapshotStateBuilder() : base(DefaultStateIds.DeviceSnapshotState, DefaultStateNames.DeviceSnapshotState)
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

        state.CheckJobstatesActionForStateDelegate =
            config.CheckJobstatesActionForStateDelegate ?? CheckJobstatesActionForStateDelegate;

        state.CancelStateDelegate = CancelStateDelegate;
        state.AllowedNextStates.AddRange(AllowedNextStatesInternal);
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

    private static void CancelStateDelegate(IStateMachineState state)
    {
        state.CancellationTokenSource?.Cancel();
    }
}