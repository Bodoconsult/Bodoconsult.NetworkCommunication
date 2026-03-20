// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.States;

/// <summary>
/// Current implementation of <see cref="INoActionStateMachineState"/>
/// </summary>
public class NoActionStateMachineState : BaseStateMachineState, INoActionStateMachineState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="id">ID of the current state</param>
    /// <param name="name">Name of the current state</param>
    public NoActionStateMachineState(IStateMachineDevice currentContext, int id, string name) : base(currentContext, id, name)
    {
        CheckJobstatesActionForStateDelegate = DefaultCheckJobstatesActionForState;
        CancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Delegate to be executed from a no action state machine state to check if job states are required to be executed
    /// </summary>
    public CheckJobstatesActionForStateDelegate? CheckJobstatesActionForStateDelegate { get; set; }

    /// <summary>
    /// Execute the action defined with <see cref="INoActionStateMachineState.CheckJobstatesActionForStateDelegate"/> for this state to check if job states are required to be executed
    /// </summary>
    public void CheckJobstates()
    {
        CheckJobstatesActionForStateDelegate?.Invoke(this);
    }

    public void DefaultCheckJobstatesActionForState(INoActionStateMachineState currentState)
    {
        while (!CancellationTokenSource?.IsCancellationRequested ?? false)
        {
            // Check if there is a job state to restore after break
            if (CurrentContext.SavedJobState != null)
            {
                CurrentContext.RestoreSavedJobState();
                return;
            }

            // Check if a job state is waiting. If yes, process it now
            if (CurrentContext.JobStates.Count > 0)
            {
                // Get the first job state and process it
                var state = CurrentContext.JobStates.First();

                CurrentContext.JobStates.Remove(state);

                CurrentContext.RequestState(state);

                CancellationTokenSource?.Dispose();
                return;
            }

            // Wait a bit then check again
            Thread.Sleep(DeviceCommunicationBasics.JobStateCheckTimeout);
        }
    }
}