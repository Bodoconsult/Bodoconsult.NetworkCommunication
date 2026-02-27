// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

public abstract class BaseOrderManagementDevice : IOrderManagementDevice
{
    protected bool _isOrderProcessingActivated;

    protected readonly IAppLoggerProxy _appLogger;
    protected readonly IAppLoggerProxy _monitorLogger;

    protected BaseOrderManagementDevice(IDataMessagingConfig dataMessagingConfig)
    {
        DataMessagingConfig = dataMessagingConfig ?? throw new ArgumentNullException(nameof(dataMessagingConfig));
        _appLogger = dataMessagingConfig.AppLogger; 
        _monitorLogger = dataMessagingConfig.MonitorLogger;
    }

    /// <summary>
    /// Device configuration for data messaging
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; protected set; }

    /// <summary>
    /// Communication adapter to use for order management
    /// </summary>
    
    public IOrderManagementCommunicationAdapter CommunicationAdapter { get; protected set; }

    /// <summary>
    /// Current instance of the Device order processor
    /// </summary>
    
    public IOrderProcessor OrderProcessor { get; protected set; }

    /// <summary>
    /// Current order manager
    /// </summary>
    public IOrderManager OrderManager { get; protected set; }

    /// <summary>
    /// Is the device connected?
    /// </summary>
    public bool IsConnected
    {
        get
        {
            if (!CommunicationAdapter.IsConnected)
            {
                // RL: ComDev should be reset to initial state here under all circumstances to avoid any hanging comm parts
                CommunicationAdapter.ComDevReset();
                DoNotify(DefaultDeviceStates.DeviceStateOffline);
                //_monitorLogger?.LogError($"Ping NOT successful for IP {SmdDevice.IpAddress}");
                SetOrderProcessingStateDelegate(false);
                return false;
            }

            SetOrderProcessingStateDelegate(true);
            return true;
        }
    }

    /// <summary>
    /// Is the running of orders allowed currently
    /// </summary>
    public bool IsRunningOrdersAllowed => CommunicationAdapter.IsConnected;

    /// <summary>
    /// Is the order processing currently activated
    /// </summary>
    public bool IsOrderProcessingActivated
    {
        get => _isOrderProcessingActivated;
        set
        {
            if (_isOrderProcessingActivated != value)
            {
                _appLogger?.LogInformation(value
                    ? $"{DataMessagingConfig.LoggerId}order processing is activated now"
                    : $"{DataMessagingConfig.LoggerId}order processing is deactivated now");

                _isOrderProcessingActivated = value;
            }

            if (OrderProcessor != null)
            {
                OrderProcessor.IsRunnerStopped = !_isOrderProcessingActivated;
            }

            if (OrderManager?.OrderReceiver != null)
            {
                OrderManager.OrderReceiver.IsReceivedMessageProcessingActivated = _isOrderProcessingActivated;
            }

        }
    }

    /// <summary>
    /// Current master order factory. This prop is intended for testing purposes. Do not use directly
    /// </summary>
    
    public IMasterOrderFactory MasterOrderFactory { get; protected set; }

    /// <summary>
    /// Get all orders in the queue
    /// </summary>
    public IList<IOrder> OrdersInQueue
    {
        get
        {
            var result = new List<IOrder>();
            result.AddRange(OrderProcessor.OrdersInQueue);
            result.AddRange(OrderProcessor.OrdersInQueueWithPriority);
            return result;
        }
    }

    /// <summary>
    /// Get all orders currently in processing
    /// </summary>
    public IList<IOrder> OrdersInProcessing => OrderProcessor.OrdersInProcessing;

    /// <summary>
    /// No order in processing
    /// </summary>
    public bool IsNoOrderInProcessing => OrderProcessor.IsNoOrderInProcessing;

    /// <summary>
    /// Is a previous unload still processing
    /// </summary>
    public bool IsPreviousUnloadProcessing => OrderProcessor.IsPreviousUnloadProcessing;

    /// <summary>
    /// Number of orders in processing
    /// </summary>
    public int OrdersInProcessingCount => OrderProcessor.OrdersInProcessingCount;

    /// <summary>
    /// Is any order to process?
    /// </summary>
    public bool IsAnyOrderToProcess => OrderProcessor.IsAnyOrderToProcess;

    /// <summary>
    /// Start the communication
    /// </summary>
    public virtual void StartComm()
    {
        CommunicationAdapter.ComDevInit();
    }

    /// <summary>
    /// Reset the communication
    /// </summary>
    public virtual void ResetComm()
    {
        CommunicationAdapter.ComDevReset();
    }

    public virtual bool IsRunningTheOrderAllowed(IOrder order, IList<IOrder> runningOrders)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Is the device pingable
    /// </summary>
    /// <returns>True if the device is pingable else false</returns>
    public virtual bool IsPingable { get; protected set; }

    /// <summary>
    /// Clear the internal state without breaking comm
    /// </summary>
    public virtual void ResetInternalState()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Start the software side of device init process
    /// </summary>
    public void RequestDeviceInit()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Prepare the orders required to check if the device is in update mode
    /// </summary>
    /// <returns>Orders to be processed for the update mode check</returns>
    public virtual List<IOrder> PrepareUpdateModeCheck()
    {
        throw new NotSupportedException("Override in derived classes if needed");
    }

    /// <summary>
    /// Prepare the orders for the software side of device init process
    /// </summary>
    public virtual List<IOrder> PrepareDeviceInit()
    {
        throw new NotSupportedException("Override in derived classes if needed");
    }

    /// <summary>
    /// Get next order to run
    /// </summary>
    public virtual IOrder GetNextOrderToRun()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check if other orders following the current order have to be removed from the queue  by cancelling them
    /// </summary>
    /// <param name="order">Current order</param>
    public virtual void Check4ConcurrentOrders(IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// The order was finished successfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    public virtual void OrderFinishedSuccessful(IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// The order was NOT finished succesfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    public virtual void OrderFinishedUnsuccessful(IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel orders with special handling
    /// </summary>
    public virtual void CancelOrdersWithSpecialHandling()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancel all running orders
    /// </summary>
    /// <param name="errorCode">Error code received from device or business logic</param>
    public virtual void CancelRunningOrders(byte errorCode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Create a hardware init order
    /// </summary>
    public IOrder CreateHardwareInitOrder()
    {
        throw new NotSupportedException("Override in derived classes if needed");
    }

    /// <summary>
    /// Do notify that a hardware init was requested
    /// </summary>
    public void DoNotifyHardwareInitRequested()
    {
        throw new NotSupportedException("Override in derived classes if needed");
    }

    /// <summary>
    /// Do a basic check for the received message
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    /// <returns>True if the message was an expected answer of the current request or should not be handled at all else false</returns>
    public bool DoBasicCheckForReceivedMessage(IInboundDataMessage receivedMessage)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check if the message is an error message
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    /// <returns>True if the message was an handled as error message else false</returns>
    public bool DoCheckForErrorMessage(IInboundDataMessage receivedMessage)
    {
        throw new NotImplementedException();
    }

    public virtual void CancelRunningOperation()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check if there are any orders to be created 
    /// </summary>
    public virtual void CheckIfThereAreOrdersToBeCreated()
    {
        // ToDo: new delegate
        //SlotCheckBusinessDelegate?.CheckCurrentAbortedUnloadOrLoadOrder();
        _appLogger.LogDebug($"{DataMessagingConfig.LoggerId}Run SlotCheck.CheckCurrentAbortedUnloadOrLoadOrder");
    }

    /// <summary>
    /// Log in DEBUG mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public virtual void LogDebug(string message)
    {
        _monitorLogger.LogDebug(message);
        _appLogger.LogDebug($"{DataMessagingConfig.LoggerId}{message}");
        Debug.Print($"STS: {message}");
    }

    /// <summary>
    /// Log in INFORMATION mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public virtual void LogInformation(string message)
    {
        _monitorLogger.LogInformation(message);
        _appLogger.LogInformation($"{DataMessagingConfig.LoggerId}{message}");
        Debug.Print($"STS: {message}");
    }

    /// <summary>
    /// Log in WARNING mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public virtual void LogWarning(string message)
    {
        _monitorLogger.LogWarning(message);
        _appLogger.LogWarning($"{DataMessagingConfig.LoggerId}{message}");
        Debug.Print($"STS: {message}");
    }

    /// <summary>
    /// Log in ERROR mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public virtual void LogError(string message)
    {
        _monitorLogger.LogError(message);
        _appLogger.LogError($"{DataMessagingConfig.LoggerId}{message}");
        Debug.Print($"STS: {message}");
    }


    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        //Stop();
        //_statusWatchdog?.StopWatchDog();
        OrderManager?.StopOrderProcessing();
        OrderProcessor.Dispose();
        CommunicationAdapter?.Dispose();
        _monitorLogger?.Dispose();
        DataMessagingConfig.DataMessageProcessingPackage?.WaitStateManager?.Dispose();
    }

    /// <summary>
    /// Load Device master order factory instance as current <see cref="MasterOrderFactory"/> instance
    /// </summary>
    /// <param name="masterOrderFactory">Current master order factory to load</param>
    public void LoadMasterOrderFactory(IMasterOrderFactory masterOrderFactory)
    {
        MasterOrderFactory = masterOrderFactory;
    }

    /// <summary>
    /// Load the Device order manager to use for this device
    /// </summary>
    /// <param name="orderManager">Device order manager</param>
    /// <remarks>Injecting via ctor not possible due to circular references otherwise</remarks>
    public void LoadDeviceOrderManager(IOrderManager orderManager)
    {
        OrderManager = orderManager;
        OrderProcessor = OrderManager.OrderProcessor;

        IsOrderProcessingActivated = true;
    }

    /// <summary>
    /// Create a new order of a requested type
    /// </summary>
    /// <param name="orderType">Order type</param>
    /// <param name="parameterSet">Current parameter set to use for the newly created order</param>
    /// <returns>An order to let it execute by TOM</returns>
    public IOrder CreateOrder(Type orderType, IParameterSet parameterSet)
    {
        return MasterOrderFactory.GetOrder(orderType.Name, parameterSet);
    }

    /// <summary>
    /// Cancel ongoing Device order task
    /// </summary>
    /// <param name="order"></param>
    public void CancelDeviceOrder(long order)
    {
        OrderProcessor.CancelOrder(order);
    }

    /// <summary>
    /// Starts the Device order processing
    /// </summary>
    public void Start()
    {
        // Start order processing now
        OrderManager.StartOrderProcessing();
    }


    /// <summary>
    /// Stops the Device order processing
    /// </summary>
    public void Stop()
    {
        //_statusWatchdog?.StopWatchDog();
        OrderManager?.StopOrderProcessing();
        _monitorLogger?.LogError($"{DataMessagingConfig.LoggerId}DeviceServer stopped - Com Dev Close called");
        CommunicationAdapter.ComDevClose();
    }

    /// <summary>
    /// Send an app notfication
    /// </summary>
    /// <param name="state">Business or Device state to send the notification for</param>
    /// <param name="memberName">Do not set this value</param>
    /// <param name="lineNumber">Do not set this value</param>
    public void DoNotify(IDeviceState state,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {

        if (OrderProcessor == null)
        {
            return;
        }

        //Debug.Print($"State: {state} Caller: {memberName} Line: {lineNumber}");

        //IOrder currentOrder = null;
        //try
        //{
        //    currentOrder = OrderProcessor.GetCurrentProcessingOrder();
        //}
        //catch (Exception e)
        //{
        //    _appLogger.LogError($"{DataMessagingConfig.LoggerId}: exception {e.Message} happened", e);
        //}

        //var terminal = 1;
        //try
        //{
        //    if (currentOrder?.ParameterSet is ITerminalParameterSet ps)
        //    {
        //        terminal = ps.Terminal;
        //        if (terminal == 0)
        //        {
        //            terminal = 1;

        //        }
        //    }
        //}
        //catch
        //{
        //    // Do nothing
        //}

        //MessagingBusinessDelegate.DoNotifyApplicationBusinessEvent(this, state.Name,
        //    SmdDevice.DeviceSn, state,
        //    ApplicationMessageEnum.Device,
        //    currentOrder?.Id ?? 0,
        //    PreviousDeviceState, terminal);

    }

    /// <summary>
    /// Send an app notification
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="terminal">Current terminal</param>
    /// <param name="memberName">Do not set a value</param>
    /// <param name="lineNumber">Do not set a value</param>
    public void DoNotify(IDeviceState state,
        int terminal,
        [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)

    {
        Debug.Print($"State: {state} Caller: {memberName} Line: {lineNumber}");

        if (OrderProcessor == null)
        {
            return;
        }

        _appLogger.LogDebug($"{DataMessagingConfig.LoggerId}: IsAnyOrderToProcess: {IsAnyOrderToProcess} OrdersInProcessingCount: {OrdersInProcessingCount}");
        IOrder currentOrder = null;
        try
        {
            currentOrder = OrderProcessor.GetCurrentProcessingOrder();
        }
        catch (Exception e)
        {
            _appLogger.LogError($"{DataMessagingConfig.LoggerId}: exception {e.Message} happened", e);
        }

        //MessagingBusinessDelegate.DoNotifyApplicationBusinessEvent(this,
        //    state.Name,
        //    SmdDevice.DeviceSn, state, ApplicationMessageEnum.Device,
        //    currentOrder?.Id ?? 0,
        //    PreviousDeviceState, terminal
        //);
    }

    /// <summary>
    /// Cancel the execution of orders by its sourceUid
    /// </summary>
    /// <param name="uid">Uid of the object the orders are bound to</param>
    public void CancelDeviceOrdersBySourceUid(Guid uid)
    {
        OrderProcessor.CancelOrderBySourceUid(uid);
    }

    /// <summary>
    /// Set the order processing state
    /// </summary>
    /// <param name="isActivated">Is the order processing activated: yes or no (ComDevClose)?</param>
    protected void SetOrderProcessingStateDelegate(bool isActivated)
    {
        IsOrderProcessingActivated = isActivated;

        if (IsOrderProcessingActivated)
        {
            return;
        }

        // Cancel all waiting orders to be cancelled on ComDevClose
        OrderProcessor.CancelWaitingOrdersOnComDevClose();

        //if (OrderProcessor.IsNoOrderInProcessing)
        //{
        //    return;
        //}
        // Cancel orders with non hardware error
        //OrderProcessor.CancelAllOrders();
        OrderProcessor.CancelRunningOrders(174);

    }
}