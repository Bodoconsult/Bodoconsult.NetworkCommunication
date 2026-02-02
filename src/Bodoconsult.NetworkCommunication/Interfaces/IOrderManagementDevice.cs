// Copyright (c) Mycronic. All rights reserved.

using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for implementing a device with order management
/// </summary>
public interface IOrderManagementDevice
{
    /// <summary>
    /// Device configuration for data messaging
    /// </summary>
    IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Communication adapter to use for order management
    /// </summary>
    IOrderManagementCommunicationAdapter CommunicationAdapter { get; }

    /// <summary>
    /// Current instance of the tower order processor
    /// </summary>
    [JsonIgnore]
    IOrderProcessor OrderProcessor { get; }

    /// <summary>
    /// Is the device connected?
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Is the running of orders allowed currently
    /// </summary>
    bool IsRunningOrdersAllowed { get; }

    /// <summary>
    /// Is running the order allowed at the current timepoint
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="runningOrders">Current running orders</param>
    /// <returns>True if it is allowed to run the order else false</returns>
    bool IsRunningTheOrderAllowed(IOrder order, IList<IOrder> runningOrders);

    /// <summary>
    /// Is the device pingable
    /// </summary>
    /// <returns>True if the device is pingable else false</returns>
    bool IsPingable { get; }

    /// <summary>
    /// Start the communication
    /// </summary>
    void StartComm();

    /// <summary>
    /// Reset the communication
    /// </summary>
    void ResetComm();

    /// <summary>
    /// Clear the internal state without breaking comm
    /// </summary>
    void ResetInternalState();

    /// <summary>
    /// Start the software side of device init process
    /// </summary>
    void RequestDeviceInit();

    /// <summary>
    /// Prepare the orders required to check if the device is in update mode
    /// </summary>
    /// <returns>Orders to be processed for the update mode check</returns>
    List<IOrder> PrepareUpdateModeCheck();

    /// <summary>
    /// Prepare the orders for the software side of device init process
    /// </summary>
    /// <returns>Orders to be processed for the software side of device init process</returns>
    List<IOrder> PrepareDeviceInit();

    /// <summary>
    /// Get next order to run
    /// </summary>
    IOrder GetNextOrderToRun();

    /// <summary>
    /// Check if other orders following the current order have to be removed from the queue  by cancelling them
    /// </summary>
    /// <param name="order">Current order</param>
    void Check4ConcurrentOrders(IOrder order);

    /// <summary>
    /// The order was finished successfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    public void OrderFinishedSuccessful(IOrder order);

    /// <summary>
    /// The order was NOT finished succesfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    void OrderFinishedUnsuccessful(IOrder order);

    /// <summary>
    /// Cancel orders with special handling
    /// </summary>
    void CancelOrdersWithSpecialHandling();

    /// <summary>
    /// Cancel all running orders
    /// </summary>
    /// <param name="errorCode">Error code received from device or business logic</param>
    void CancelRunningOrders(byte errorCode);

    ///// <summary>
    ///// Set the business state from an order to be execute. Runs before order execution starts
    ///// </summary>
    ///// <param name="order">Current order to execute</param>
    //void SetBusinessStateFromOrder(IOrder order);

    ///// <summary>
    ///// Handle a received error
    ///// </summary>
    ///// <param name="receivedMessage">Received message</param>
    //void HandleError(IDataMessage receivedMessage);

    ///// <summary>
    ///// Handle async sent message from device
    ///// </summary>
    ///// <param name="message">Received message</param>
    ///// <returns>The result of the message handling</returns>
    //MessageHandlingResult HandleAsyncMessage(IDataMessage message);


    /// <summary>
    /// Create a hardware init order
    /// </summary>
    IOrder CreateHardwareInitOrder();

    /// <summary>
    /// Do notify that a hardware init was requested
    /// </summary>
    void DoNotifyHardwareInitRequested();

    /// <summary>
    /// Do a basic check for the received message
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    /// <returns>True if the message was an expected answer of the current request or should not be handled at all else false</returns>
    bool DoBasicCheckForReceivedMessage(ICommandDataMessage receivedMessage);

    /// <summary>
    /// Check if the message is an error message
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    /// <returns>True if the message was an handled as error message else false</returns>
    bool DoCheckForErrorMessage(ICommandDataMessage receivedMessage);

    /// <summary>
    /// Cancel the currently running operation
    /// </summary>
    void CancelRunningOperation();

    /// <summary>
    /// Check if there are any orders to be created 
    /// </summary>
    void CheckIfThereAreOrdersToBeCreated();

    #region Logging

    /// <summary>
    /// Log in DEBUG mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public void LogDebug(string message);

    /// <summary>
    /// Log in INFORMATION mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public void LogInformation(string message);

    /// <summary>
    /// Log in WARNING mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public void LogWarning(string message);

    /// <summary>
    /// Log in ERROR mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public void LogError(string message);

    #endregion


    /// <summary>
    /// Send an app notfication
    /// </summary>
    /// <param name="state">Business or tower state to send the notification for</param>
    /// <param name="memberName">Do not set this value</param>
    /// <param name="lineNumber">Do not set this value</param>
    void DoNotify(IDeviceState state,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0);
}