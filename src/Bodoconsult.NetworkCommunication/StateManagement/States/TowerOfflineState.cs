// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.States;

/// <summary>
/// The device is offline. Ths state is kept until a PING to the device was successfull
/// </summary>
public class DeviceOfflineState : BaseStateMachineState
{

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private static readonly List<string> AllowedNextStatesInternal = new()
    {
        StateConstants.TowerOnlineStateString
    };

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceOfflineState(IStateMachineContext currentContext) : base(currentContext)
    {
        Id = (int)StSysTowerBusinessStateEnum.TowerOfflineState;
        Name = string.Intern(nameof(TowerOfflineState));

        CurrentTowerServer = (ITowerServer)currentContext;
            
        IsRunningOrdersCancellationRequired = true;
        IsTurningOffStateRequestsRequired = true;

        AllowedNextStates = AllowedNextStatesInternal;
    }

    /// <summary>
    /// Set the inital states for this business state
    /// </summary>
    public override void SetInitalStates()
    {
        CurrentContext.SetStates(StSysTowerHardwareState.TowerStateOffline, StSysTowerBusinessSubState.NotSet);
    }

    /// <summary>
    /// Run the next order for this state
    /// </summary>
    public override void RunNextOrder()
    {
        // Wait until the device is pingable
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            CurrentContext.SetBusinessSubState (StSysTowerBusinessSubState.PingingTower);
            if (CurrentTowerServer.IsPingable)
            {
                break;
            }

            CurrentContext.SetBusinessSubState (StSysTowerBusinessSubState.WaitingForNextPingingTower);
            Thread.Sleep(TowerCommunicationBasics.PingRepeatInterval);
        }

        // New state now
        var stateNew = CurrentTowerServer.CreateStateInstance(StateConstants.TowerOnlineStateString);
        NextState = stateNew;
        RequestNextState();
    }

    /// <summary>
    /// Cancel this state
    /// </summary>
    public override void CancelState()
    {
        _cancellationTokenSource.Cancel(true);
    }

    /// <summary>
    /// The communication to the device was broken. Handle this event
    /// </summary>
    public override void HandleComDevClose()
    {
        // Do nothing
    }

    /// <summary>
    /// Handle a received error message from the device
    /// </summary>
    /// <param name="receivedMessage">Received error message from the device</param>
    public override void HandleErrorMessage(ICommandDataMessage receivedMessage)
    {
        string msg;

        if (receivedMessage is not SmdTowerDataMessage rm)
        {
            msg = $"wrong message received {receivedMessage.ToInfoString()}";
            CurrentTowerServer.LogDebug(msg);
            return;
        }

        var isHardwareError = TowerHelper.IsHardwareError(rm.Error);

        // Get the tower state from tower message
        var state = CurrentTowerServer.GetTowerStateFromDatablock(rm);

        msg = $"command X received: state {state} {(isHardwareError ? "hardware error" : "error")} {rm.Error}. Error not handled while tower offline";
        CurrentTowerServer.LogDebug(msg);

    }

    /// <summary>
    /// Check a received message from device if a state change is necessary
    /// </summary>
    /// <param name="message">Received message from device</param>
    /// <param name="doNotNotifyClient">Do notify the clients</param>
    public override MessageHandlingResult CheckReceivedStateMessage(ICommandDataMessage message,
        bool doNotNotifyClient)
    {

        // Do nothing

        return MessageHandlingResultHelper.Success();
    }

        
    /// <summary>
    /// Handle async sent message from device
    /// </summary>
    /// <param name="message">Received message</param>
    /// <returns>The result of the message handling</returns>
    public override MessageHandlingResult HandleAsyncMessage(IDataMessage message)
    {
        // Do nothing
        return MessageHandlingResultHelper.Success();
    }

    /// <summary>
    /// Prepare orders for the regular state reqeust
    /// </summary>
    /// <returns>List with orders</returns>
    public override IList<IOrder> PerpareRegularStateRequest()
    {
        // Do nothing
        var orders = new List<IOrder>();
        return orders;
    }
}


/// <summary>
/// App is checking if the device is in update mode state
/// </summary>
public class TowerOnlineStateV100 : BaseTowerOnlineState
{

    private static readonly List<string> AllowedNextStatesInternal = new()
    {
        nameof(StSysInitProcessingState),
        nameof(TowerHardwareInitState)
    };

    /// <summary>
    /// Default ctor
    /// </summary>
    public TowerOnlineStateV100(IStateMachineContext currentContext) : base(currentContext)
    {
        Id = (int)StSysTowerBusinessStateEnum.TowerOnlineState;
        Name = string.Intern(StateConstants.TowerOnlineStateString);  // Name has to be firmware independent

        IsRunningOrdersCancellationRequired = true;
        IsTurningOffStateRequestsRequired = true;

        AllowedNextStates = AllowedNextStatesInternal;
    }


}

/// <summary>
/// Base class for tower online states depending on firmware version
/// </summary>
public class BaseTowerOnlineState : BaseStateMachineState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseTowerOnlineState(IStateMachineContext currentContext) : base(currentContext)
    {
        CurrentTowerServer = (ITowerServer)currentContext;
    }

    /// <summary>
    /// Set the inital states for this business state
    /// </summary>
    public override void SetInitalStates()
    {
        CurrentContext.SetStates(StSysTowerHardwareState.TowerStateOnline, StSysTowerBusinessSubState.UpdateModeCheck);
    }

    /// <summary>
    /// Initiate the state
    /// </summary>
    public override void InitiateState()
    {
        // Start the communication now
        CurrentTowerServer.StartComm();

        if (!CurrentTowerServer.IsConnected)
        {
            CurrentTowerServer.ResetComm();
            CurrentTowerServer.StartComm();
        }

        if (!CurrentTowerServer.IsConnected)
        {
            var stateNew = CurrentContext.StateMachineStateFactory.CreateInstance(nameof(TowerOfflineState));
            NextState = stateNew;
            return;
        }

        CurrentTowerServer.ResetInternalState();
        CurrentTowerServer.OrderProcessor.IsInitInProcessing = true;

        var orders = CurrentTowerServer.PrepareUpdateModeCheck();
        Orders.AddRange(orders);
    }

    /// <summary>
    /// The order has been finished successfully
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    public override void OrderFinishedSucessfully(long orderId)
    {
        CurrentContext.SetBusinessSubState(StSysTowerBusinessSubState.NormalModeReceived);
        var stateNew = CurrentContext.StateMachineStateFactory.CreateInstance(nameof(StSysInitProcessingState));
        NextState = stateNew;
    }

    /// <summary>
    /// The order has been finished successfully
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    public override void OrderFinishedUnsucessfully(long orderId)
    {
        CurrentTowerServer.OrderProcessor.IsInitInProcessing = false;
        Thread.Sleep(TowerCommunicationBasics.TowerStatusWatchdogInterval);


        if (!CurrentTowerServer.IsConnected)
        {
            CurrentTowerServer.ResetComm();
            CurrentTowerServer.StartComm();
        }

        if (!CurrentTowerServer.IsConnected)
        {
            var stateNew = CurrentContext.StateMachineStateFactory.CreateInstance(nameof(TowerOfflineState));
            NextState = stateNew;
            return;
        }

        Orders.AddRange(CurrentTowerServer.PrepareUpdateModeCheck());
        RunNextOrder();
    }

    /// <summary>
    /// Handle a received error message from the device
    /// </summary>
    /// <param name="receivedMessage">Received error message from the device</param>
    public override void HandleErrorMessage(ICommandDataMessage receivedMessage)
    {
        string msg;
        var stateNew = CurrentTowerServer.CreateStateInstance(nameof(TowerOfflineState));

        if (receivedMessage is not SmdTowerDataMessage rm)
        {
            msg = $"wrong message received {receivedMessage.ToInfoString()}. Set tower offline state";
            CurrentTowerServer.LogDebug(msg);


            NextState = stateNew;
            RequestNextState();

            return;
        }

        var isHardwareError = TowerHelper.IsHardwareError(rm.Error);

        // Get the tower state from tower message
        var state = CurrentTowerServer.GetTowerStateFromDatablock(rm);

        msg = $"command X received: state {state} {(isHardwareError ? "hardware error" : "error")} {rm.Error}. Set tower offline state";
        CurrentTowerServer.LogDebug(msg);

        NextState = stateNew;
        RequestNextState();
    }

    /// <summary>
    /// Handle async sent message from device
    /// </summary>
    /// <param name="message">Received message</param>
    /// <returns>The result of the message handling</returns>
    public override MessageHandlingResult HandleAsyncMessage(IDataMessage message)
    {
        // Do nothing
        return MessageHandlingResultHelper.Success();
    }

    /// <summary>
    /// Prepare orders for the regular state reqeust
    /// </summary>
    /// <returns>List with orders</returns>
    public override IList<IOrder> PerpareRegularStateRequest()
    {
        // Do nothing
        var orders = new List<IOrder>();
        return orders;
    }
}

public static class StateConstants
{
    public const string TowerOnlineStateString = "TowerOnlineState";
    public const string TowerCalibrationSensorStateString = "TowerCalibrationSensorState";
    public const string TowerCalibrationArmStateString = "TowerCalibrationArmState";
    public const string TowerCalibrationLiftStateString = "TowerCalibrationLiftState";
    public const string TowerCalibrationMagazineStateString = "TowerCalibrationMagazineState";
    public const string TowerCalibrationRotorStateString = "TowerCalibrationRotorState";
    public const string TowerSlotHeightCheckStateString = "TowerSlotHeightCheckState";
    public const string PerformGripHeightCheckStateString = "PerformGripHeightCheckState";
    public const string UnloadProcessingStateString = "UnloadProcessingState";
    public const string LoadProcessingStateString = "LoadProcessingState";
    public const string JoblistUnloadProcessingStateString = "JoblistUnloadProcessingState";
    public const string PerformEmptySlotsStateString = "PerformEmptySlotsState";
    public const string TowerRunProcessingStateString = "TowerRunProcessingState";
    public const string TrialRunProcessingStateString = "TrialRunProcessingState";
}