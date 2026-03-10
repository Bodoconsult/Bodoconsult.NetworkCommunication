//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//using Bodoconsult.NetworkCommunication.Interfaces;
//using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

//namespace Bodoconsult.NetworkCommunication.StateManagement.States;

///// <summary>
///// The device is offline. Ths state is kept until a PING to the device was successfull
///// </summary>
//public class DeviceOfflineState : BaseStateMachineState
//{

//    private readonly CancellationTokenSource _cancellationTokenSource = new();

//    private static readonly List<string> AllowedNextStatesInternal = new()
//    {
//        StateConstants.TowerOnlineStateString
//    };

//    /// <summary>
//    /// Default ctor
//    /// </summary>
//    public DeviceOfflineState(IStateManagementDevice currentContext) : base(currentContext)
//    {
//        Id = (int)StSysTowerBusinessStateEnum.TowerOfflineState;
//        Name = string.Intern(nameof(TowerOfflineState));

//        CurrentContext= currentContext;
            
//        IsRunningOrdersCancellationRequired = true;
//        IsTurningOffStateRequestsRequired = true;

//        AllowedNextStates = AllowedNextStatesInternal;
//    }

//    /// <summary>
//    /// Set the inital states for this business state
//    /// </summary>
//    public override void SetInitalStates()
//    {
//        CurrentContext.SetStates(DefaultDeviceStates.DeviceStateOffline, DefaultBusinessSubStates.NotSet);
//    }

//    /// <summary>
//    /// Run the next order for this state
//    /// </summary>
//    public override void RunNextOrder()
//    {
//        // Wait until the device is pingable
//        while (!_cancellationTokenSource.IsCancellationRequested)
//        {
//            CurrentContext.SetBusinessSubState (DefaultBusinessSubStates.PingingTower);
//            if (CurrentContext.IsPingable)
//            {
//                break;
//            }

//            CurrentContext.SetBusinessSubState (DefaultBusinessSubStates.WaitingForNextPingingTower);
//            Thread.Sleep(DeviceCommunicationBasics.PingRepeatInterval);
//        }

//        // New state now
//        var stateNew = CurrentContext.CreateStateInstance(StateConstants.TowerOnlineStateString);
//        NextState = stateNew;
//        RequestNextState();
//    }

//    /// <summary>
//    /// Cancel this state
//    /// </summary>
//    public override void CancelState()
//    {
//        _cancellationTokenSource.Cancel(true);
//    }

//    /// <summary>
//    /// The communication to the device was broken. Handle this event
//    /// </summary>
//    public override void HandleComDevClose()
//    {
//        // Do nothing
//    }

//    /// <summary>
//    /// Handle a received error message from the device
//    /// </summary>
//    /// <param name="receivedMessage">Received error message from the device</param>
//    public override void HandleErrorMessage(IInboundDataMessage receivedMessage)
//    {

//        // DoDo: what should happen heres

//    }

//    /// <summary>
//    /// Check a received message from device if a state change is necessary
//    /// </summary>
//    /// <param name="message">Received message from device</param>
//    /// <param name="doNotNotifyClient">Do notify the clients</param>
//    public override MessageHandlingResult CheckReceivedStateMessage(IInboundDataMessage message,
//        bool doNotNotifyClient)
//    {

//        // Do nothing

//        return MessageHandlingResultHelper.Success();
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
//}