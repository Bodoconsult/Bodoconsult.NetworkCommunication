// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.States;

/// <summary>
/// The tower is ready for order processing
/// </summary>
public class DeviceReadyState : BaseStateMachineState
{

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private static readonly List<string> AllowedNextStatesInternal = new()
    {
        nameof(TowerOfflineState),
        nameof(TowerErrorState),
        StateConstants.UnloadProcessingStateString,
        StateConstants.LoadProcessingStateString,
        StateConstants.JoblistUnloadProcessingStateString,
        nameof(PerformEmptySlotsStateV55),
        nameof(TowerBusyState),
        StateConstants.TowerCalibrationArmStateString,
        StateConstants.TowerCalibrationMagazineStateString,
        StateConstants.TowerCalibrationLiftStateString,
        StateConstants.TowerCalibrationRotorStateString,
        StateConstants.TowerCalibrationSensorStateString,
        nameof(TowerRunProcessingStateV55),
        nameof(TrialRunProcessingStateV55),
        nameof(TowerUpdateState),
        StateConstants.TowerSlotHeightCheckStateString,
        StateConstants.PerformGripHeightCheckStateString,
        nameof(TowerHardwareInitState)
    };

    /// <summary>
    /// Default ctor
    /// </summary>
    public TowerReadyState(IStateMachineContext currentContext) : base(currentContext)
    {
        Id = (int)StSysTowerBusinessStateEnum.TowerReadyState;
        Name = string.Intern(nameof(TowerReadyState));
        CurrentTowerServer = (ITowerServer)currentContext;

        AllowedNextStates = AllowedNextStatesInternal;
    }

    /// <summary>
    /// Set the inital states for this business state
    /// </summary>
    public override void SetInitalStates()
    {
        CurrentContext.SetStates(StSysTowerHardwareState.TowerStateReady, StSysTowerBusinessSubState.Idle);
    }

    /// <summary>
    /// Run the next order for this state
    /// </summary>
    public override void RunNextOrder()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            // Check if there is a job state to restore after break
            if (CurrentContext.SavedJobState != null)
            {
                CurrentContext.RestoreSavedState();
                return;
            }

            // Check if a job state is waiting. If yes, process it now
            if (EnumerableExtensions.Any(CurrentContext.JobStates))
            {
                // Get the first job state and process it
                var state = CurrentContext.JobStates.First();

                CurrentContext.JobStates.Remove(state);

                CurrentContext.RequestState(state);

                _cancellationTokenSource.Dispose();
                return;
            }

            // Wait a bit then check again
            Thread.Sleep(TowerCommunicationBasics.ReadyStateCheckTimeout);
        }
    }

    /// <summary>
    /// Cancel this state
    /// </summary>
    public override void CancelState()
    {
        _cancellationTokenSource.Cancel(true);
    }
}