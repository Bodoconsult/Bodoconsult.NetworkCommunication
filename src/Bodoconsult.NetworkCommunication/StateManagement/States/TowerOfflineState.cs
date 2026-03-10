//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//using Bodoconsult.NetworkCommunication.Interfaces;
//using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

//namespace Bodoconsult.NetworkCommunication.StateManagement.States;

///// <summary>
///// App is checking if the device is in update mode state
///// </summary>
//public class TowerOnlineStateV100 : BaseTowerOnlineState
//{
//    public static readonly List<string> AllowedNextStatesInternal = new()
//    {
//        nameof(StSysInitProcessingState),
//        nameof(TowerHardwareInitState)
//    };

//    /// <summary>
//    /// Default ctor
//    /// </summary>
//    public TowerOnlineStateV100(IStateMachineContext currentContext) : base(currentContext)
//    {
//        Id = (int)StSysTowerBusinessStateEnum.TowerOnlineState;
//        Name = string.Intern(StateConstants.TowerOnlineStateString);  // Name has to be firmware independent

//        IsRunningOrdersCancellationRequired = true;
//        IsTurningOffStateRequestsRequired = true;

//        AllowedNextStates = AllowedNextStatesInternal;
//    }


//}

///// <summary>
///// Base class for tower online states depending on firmware version
///// </summary>
//public class BaseTowerOnlineState : BaseStateMachineState
//{
//    /// <summary>
//    /// Default ctor
//    /// </summary>
//    protected BaseTowerOnlineState(IStateManagementDevice currentContext) : base(currentContext)
//    {
//        CurrentContext = currentContext;
//    }

//    /// <summary>
//    /// Set the inital states for this business state
//    /// </summary>
//    public override void SetInitalStates()
//    {
//        CurrentContext.SetStates(DefaultDeviceStates.DeviceStateOnline, DefaultBusinessSubStates.NotSet);
//    }

//    /// <summary>
//    /// Initiate the state
//    /// </summary>
//    public override void InitiateState()
//    {
//        // Start the communication now
//        CurrentTowerServer.StartComm();

//        if (!CurrentTowerServer.IsConnected)
//        {
//            CurrentTowerServer.ResetComm();
//            CurrentTowerServer.StartComm();
//        }

//        if (!CurrentTowerServer.IsConnected)
//        {
//            var stateNew = CurrentContext.StateMachineStateFactory.CreateInstance(nameof(TowerOfflineState));
//            NextState = stateNew;
//            return;
//        }

//        CurrentTowerServer.ResetInternalState();
//        CurrentTowerServer.OrderProcessor.IsInitInProcessing = true;

//        var orders = CurrentTowerServer.PrepareUpdateModeCheck();
//        Orders.AddRange(orders);
//    }

//    /// <summary>
//    /// The order has been finished successfully
//    /// </summary>
//    /// <param name="orderId">Current order ID</param>
//    public override void OrderFinishedSucessfully(long orderId)
//    {
//        CurrentContext.SetBusinessSubState(StSysTowerBusinessSubState.NormalModeReceived);
//        var stateNew = CurrentContext.StateMachineStateFactory.CreateInstance(nameof(StSysInitProcessingState));
//        NextState = stateNew;
//    }

//    /// <summary>
//    /// The order has been finished successfully
//    /// </summary>
//    /// <param name="orderId">Current order ID</param>
//    public override void OrderFinishedUnsucessfully(long orderId)
//    {
//        CurrentTowerServer.OrderProcessor.IsInitInProcessing = false;
//        Thread.Sleep(TowerCommunicationBasics.TowerStatusWatchdogInterval);


//        if (!CurrentTowerServer.IsConnected)
//        {
//            CurrentTowerServer.ResetComm();
//            CurrentTowerServer.StartComm();
//        }

//        if (!CurrentTowerServer.IsConnected)
//        {
//            var stateNew = CurrentContext.StateMachineStateFactory.CreateInstance(nameof(TowerOfflineState));
//            NextState = stateNew;
//            return;
//        }

//        Orders.AddRange(CurrentTowerServer.PrepareUpdateModeCheck());
//        RunNextOrder();
//    }

//    /// <summary>
//    /// Handle a received error message from the device
//    /// </summary>
//    /// <param name="receivedMessage">Received error message from the device</param>
//    public override void HandleErrorMessage(IInboundDataMessage receivedMessage)
//    {
//        string msg;
//        var stateNew = CurrentContext.CreateStateInstance(nameof(DeviceOfflineState));

//        if (receivedMessage is not SmdTowerDataMessage rm)
//        {
//            msg = $"wrong message received {receivedMessage.ToInfoString()}. Set tower offline state";
//            CurrentTowerServer.LogDebug(msg);


//            NextState = stateNew;
//            RequestNextState();

//            return;
//        }

//        var isHardwareError = TowerHelper.IsHardwareError(rm.Error);

//        // Get the tower state from tower message
//        var state = CurrentTowerServer.GetTowerStateFromDatablock(rm);

//        msg = $"command X received: state {state} {(isHardwareError ? "hardware error" : "error")} {rm.Error}. Set tower offline state";
//        CurrentTowerServer.LogDebug(msg);

//        NextState = stateNew;
//        RequestNextState();
//    }

//    /// <summary>
//    /// Handle async sent message from device
//    /// </summary>
//    /// <param name="message">Received message</param>
//    /// <returns>The result of the message handling</returns>
//    public override MessageHandlingResult HandleAsyncMessage(IInboundDataMessage message)
//    {
//        // Do nothing
//        return MessageHandlingResultHelper.Success();
//    }

//    /// <summary>
//    /// Prepare orders for the regular state reqeust
//    /// </summary>
//    /// <returns>List with orders</returns>
//    public override List<IOrder> PerpareRegularStateRequest()
//    {
//        // Do nothing
//        var orders = new List<IOrder>();
//        return orders;
//    }
//}

//public static class StateConstants
//{
//    public const string TowerOnlineStateString = "TowerOnlineState";
//    public const string TowerCalibrationSensorStateString = "TowerCalibrationSensorState";
//    public const string TowerCalibrationArmStateString = "TowerCalibrationArmState";
//    public const string TowerCalibrationLiftStateString = "TowerCalibrationLiftState";
//    public const string TowerCalibrationMagazineStateString = "TowerCalibrationMagazineState";
//    public const string TowerCalibrationRotorStateString = "TowerCalibrationRotorState";
//    public const string TowerSlotHeightCheckStateString = "TowerSlotHeightCheckState";
//    public const string PerformGripHeightCheckStateString = "PerformGripHeightCheckState";
//    public const string UnloadProcessingStateString = "UnloadProcessingState";
//    public const string LoadProcessingStateString = "LoadProcessingState";
//    public const string JoblistUnloadProcessingStateString = "JoblistUnloadProcessingState";
//    public const string PerformEmptySlotsStateString = "PerformEmptySlotsState";
//    public const string TowerRunProcessingStateString = "TowerRunProcessingState";
//    public const string TrialRunProcessingStateString = "TrialRunProcessingState";
//}