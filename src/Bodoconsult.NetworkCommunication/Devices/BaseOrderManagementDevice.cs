// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Bodoconsult.NetworkCommunication.Devices;

/// <summary>
/// Base class for order management devices
/// </summary>
public abstract class BaseOrderManagementDevice : IOrderManagementDevice
{
    /// <summary>
    /// Is order processing avtivated?
    /// </summary>
    protected bool IsOrderProcessingActivatedInternal;

    /// <summary>
    ///  Current app logger
    /// </summary>
    protected readonly IAppLoggerProxy AppLogger;

    /// <summary>
    /// Current monitor logger
    /// </summary>
    protected readonly IAppLoggerProxy MonitorLogger;


    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    protected BaseOrderManagementDevice(IDataMessagingConfig dataMessagingConfig, ICentralClientNotificationManager clientNotificationManager)
    {
        DataMessagingConfig = dataMessagingConfig ?? throw new ArgumentNullException(nameof(dataMessagingConfig));
        AppLogger = dataMessagingConfig.AppLogger;
        MonitorLogger = dataMessagingConfig.MonitorLogger;
        ClientNotificationManager = clientNotificationManager;
    }

    /// <summary>
    /// Device configuration for data messaging
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; protected set; }

    /// <summary>
    /// Communication adapter to use for order management
    /// </summary>

    public ICommunicationAdapter? CommunicationAdapter { get; protected set; }

    /// <summary>
    /// Current instance of the Device order processor
    /// </summary>

    public IOrderProcessor? OrderProcessor { get; protected set; }

    /// <summary>
    /// Current order manager
    /// </summary>
    public IOrderManager? OrderManager { get; protected set; }

    /// <summary>
    /// Client notification manager
    /// </summary>
    public ICentralClientNotificationManager ClientNotificationManager { get; }

    /// <summary>
    /// Current <see cref="IDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public IDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter { get; protected set; }

    /// <summary>
    /// Device states used for init process
    /// </summary>
    public IList<IDeviceState> DeviceStatesInitProcess { get; } = new List<IDeviceState>();

    /// <summary>
    /// Current <see cref="IOrderManagementDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public IOrderManagementDeviceBusinessLogicAdapter? OrderManagementDeviceBusinessLogicAdapter { get; protected set; }

    /// <summary>
    /// Is the device connected?
    /// </summary>
    public bool IsConnected
    {
        get
        {
            if (CommunicationAdapter == null)
            {
                return false;
            }

            if (!CommunicationAdapter.IsConnected)
            {
                // RL: ComDev should be reset to initial state here under all circumstances to avoid any hanging comm parts
                CommunicationAdapter.ComDevReset();
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
    public bool IsRunningOrdersAllowed
    {
        get
        {
            if (CommunicationAdapter == null)
            {
                AppLogger.LogInformation("CommAdapter is null");
                return false;
            }

            var isConnected = CommunicationAdapter.IsConnected;

            // Connected?
            if (isConnected)
            {
                return isConnected;
            }

            // Not connected
            CommunicationAdapter.ComDevInit();

            isConnected = CommunicationAdapter.IsConnected;

            if (!isConnected)
            {
                AppLogger.LogWarning("Not connected");
            }

            return isConnected;
        }
    }

    /// <summary>
    /// Is the order processing currently activated
    /// </summary>
    public bool IsOrderProcessingActivated
    {
        get => IsOrderProcessingActivatedInternal;
        set
        {
            if (IsOrderProcessingActivatedInternal != value)
            {
                AppLogger.LogInformation(value
                    ? $"{DataMessagingConfig.LoggerId}order processing is activated now"
                    : $"{DataMessagingConfig.LoggerId}order processing is deactivated now");

                IsOrderProcessingActivatedInternal = value;
            }

            if (OrderProcessor != null)
            {
                OrderProcessor.IsRunnerStopped = !IsOrderProcessingActivatedInternal;
            }

            if (OrderManager?.OrderReceiver != null)
            {
                OrderManager.OrderReceiver.IsReceivedMessageProcessingActivated = IsOrderProcessingActivatedInternal;
            }
        }
    }

    ///// <summary>
    ///// Current master order factory. This prop is intended for testing purposes. Do not use directly
    ///// </summary>

    //public IMasterOrderFactory MasterOrderFactory { get; protected set; }

    /// <summary>
    /// Get all orders in the queue
    /// </summary>
    public IList<IOrder> OrdersInQueue
    {
        get
        {
            if (OrderProcessor == null)
            {
                return [];
            }

            var result = new List<IOrder>();
            result.AddRange(OrderProcessor.OrdersInQueue);
            result.AddRange(OrderProcessor.OrdersInQueueWithPriority);
            return result;
        }
    }

    /// <summary>
    /// Get all orders currently in processing
    /// </summary>
    public IList<IOrder> OrdersInProcessing => OrderProcessor?.OrdersInProcessing ?? [];

    /// <summary>
    /// No order in processing
    /// </summary>
    public bool IsNoOrderInProcessing => OrderProcessor?.IsNoOrderInProcessing ?? false;

    /// <summary>
    /// Number of orders in processing
    /// </summary>
    public int OrdersInProcessingCount => OrderProcessor?.OrdersInProcessingCount ?? 0;

    /// <summary>
    /// Is any order to process?
    /// </summary>
    public bool IsAnyOrderToProcess => OrderProcessor?.IsAnyOrderToProcess ?? false;

    /// <summary>
    /// Load the current <see cref="IDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    /// <param name="businessLogicAdapter">Current <see cref="IDeviceBusinessLogicAdapter"/> instance</param>
    public virtual void LoadDeviceBusinessLogicAdapter(IDeviceBusinessLogicAdapter businessLogicAdapter)
    {
        if (businessLogicAdapter is not IOrderManagementDeviceBusinessLogicAdapter)
        {
            throw new ArgumentException($"businessLogicAdapter is not {nameof(IOrderManagementDeviceBusinessLogicAdapter)}");
        }

        DeviceBusinessLogicAdapter = businessLogicAdapter;
    }

    /// <summary>
    /// Start the communication
    /// </summary>
    public virtual void StartComm()
    {
        CommunicationAdapter?.ComDevInit();
    }

    /// <summary>
    /// Reset the communication
    /// </summary>
    public virtual void ResetComm()
    {
        CommunicationAdapter?.ComDevReset();
    }

    /// <summary>
    /// Stop the communication
    /// </summary>
    public void StopComm()
    {
        CommunicationAdapter?.ComDevClose();
    }

    /// <summary>
    /// Is running the order allowed at the current timepoint
    /// </summary>
    /// <param name="order">Order to run</param>
    /// <param name="runningOrders">Current running orders</param>
    /// <returns>True if it is allowed to run the order else false</returns>
    public virtual bool IsRunningTheOrderAllowed(IOrder order, IList<IOrder> runningOrders)
    {
        // No orders running at all
        if (!runningOrders.Any())
        {
            return true;
        }

        // Check if the type of order is allowed to run parallel to the current orders
        foreach (var rOrder in runningOrders)
        {
            var pOrders = rOrder.AllowedParallelOrderTypes;

            // If the new order is allowed to run at the moment leave here
            if (pOrders.Contains(order.TypeName))
            {
                continue;
            }

            //_smdTower.AppLogger.LogInformation($"{_smdTower.LoggerId}: {order.Name} {order.Id} NOT allowed to run parallel");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Is the device pingable
    /// </summary>
    /// <returns>True if the device is pingable else false</returns>
    public virtual bool IsPingable => CommunicationAdapter?.IsPingableAsync().GetAwaiter().GetResult() ?? false;

    /// <summary>
    /// Load a communication adapter
    /// </summary>
    /// <param name="commAdapter">Current communication adapter</param>
    public void LoadCommAdapter(ICommunicationAdapter commAdapter)
    {
        CommunicationAdapter = commAdapter;
    }

    /// <summary>
    /// Clear the internal state without breaking comm
    /// </summary>
    public virtual void ResetInternalState()
    {
        CommunicationAdapter?.ResetInternalState();
        //_isRunning = false;
        if (OrderProcessor != null)
        {
            OrderProcessor.IsInitInProcessing = false;
            OrderProcessor.IsRunnerStopped = false;
        }
        AppLogger.LogInformation("Internal state was reset");
    }

    /// <summary>
    /// Connected devices
    /// </summary>
    public List<IIpDevice> ConnectedDevices { get; } = [];

    /// <summary>
    /// Start the software side of device init process
    /// </summary>
    public void RequestDeviceInit()
    {
        throw new NotSupportedException("Override in derived classes if needed");
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
    public virtual IOrder? GetNextOrderToRun()
    {
        ArgumentNullException.ThrowIfNull(OrderProcessor);

        IOrder? order;
        //var isState = DeviceStatesInitProcess.Contains(DeviceState);
        var isInit = OrderProcessor.IsInitInProcessing;

        var s = $"{DataMessagingConfig.LoggerId}{OrderProcessor.OrderPipeline.CurrentOrderState} Init {isInit}";
        Trace.TraceInformation(s);
        AppLogger.LogDebug(s);

        // ****************************
        // StSys tower init is running: run only priority orders stsys init and tower hardware init
        // ****************************
        if (isInit)
        {
            order = OrderProcessor.OrderPipeline.GetNextPriorityOrder();
            if (order == null)
            {

                return null;
            }

            order.IsHighPriorityOrder = true;
            return order;
        }

        // ****************************
        // If tower is READY: try to run priority orders first, then all other orders
        // ****************************

        // Check if priority orders have to be built: CheckSlot for open unload orders
        CheckIfThereAreOrdersToBeCreated();

        // Now check the priority orders
        order = OrderProcessor.OrderPipeline.GetNextPriorityOrder();
        if (order != null)
        {
            order.IsHighPriorityOrder = true;
            return order;
        }

        // Do not run normal orders if runner is deactivated
        if (OrderProcessor.IsRunnerStopped)
        {
            return null;
        }

        order = OrderProcessor.OrderPipeline.GetNextNonPriorityOrder();
        if (order == null)
        {
            return null;
        }

        order.IsHighPriorityOrder = false;
        return order;
    }

    /// <summary>
    /// Check if other orders following the current order have to be removed from the queue by cancelling them
    /// </summary>
    /// <param name="order">Current order</param>
    public virtual void Check4ConcurrentOrders(IOrder order)
    {
        // do nothing
    }

    /// <summary>
    /// The order was finished successfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    public virtual void OrderFinishedSuccessful(IOrder order)
    {
        // Now run an order specific successfully finished action
        if (order.OrderFinishedSuccessfulAction != null)
        {
            // Get the transport object from the last request. May be null
            var tro = order.RequestSpecs[^1].TransportObject;
            order.OrderFinishedSuccessfulAction(tro, order.ParameterSet);
        }

        //// Now logging and notifications
        //if (order.StateToNotifyOnSuccess != DefaultDeviceStates.DeviceStateOffline)
        //{
        //    DoNotify(order.StateToNotifyOnSuccess);
        //}

        order.ExecutionState = OrderState.FinishedSuccessfully;
        AppLogger.LogDebug($"{DataMessagingConfig.LoggerId}{order.LoggerId}has finished successful");
        //MessagingBusinessDelegate?.DoNotifyOrderStateChanged(this, order);
    }

    /// <summary>
    /// The order was NOT finished succesfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    public virtual void OrderFinishedUnsuccessful(IOrder order)
    {
        var pso = order.ParameterSet;

        try
        {
            var requestSpecs = order.RequestSpecs;

            // Now run an order specific not succesfully finished action
            if (order.OrderFinishedUnsuccessfulAction != null)
            {
                // Get the transport object from the last request. May be null
                if (requestSpecs.Any())
                {
                    var tro = order.RequestSpecs[^1].TransportObject;
                    order.OrderFinishedUnsuccessfulAction(tro, pso);
                    AppLogger.LogDebug("OrderFinishedUnsuccessfulAction TRO was successful");
                }
                else
                {
                    order.OrderFinishedUnsuccessfulAction(null, pso);
                    AppLogger.LogDebug("OrderFinishedUnsuccessfulAction NULL was successful");
                }
            }
        }
        catch (Exception e1)
        {
            try
            {
                if (order.OrderFinishedUnsuccessfulAction != null)
                {
                    order.OrderFinishedUnsuccessfulAction(null, pso);
                    LogError($"failed on level 1: {e1}");
                }
            }
            catch (Exception e2)
            {
                LogError($"failed on level 2: {e2}");
            }
        }


        // Fire a ComDevClose if required for this order
        if (order.FailingOrderRequiresComDevClose)
        {
            CommunicationAdapter?.ComDevClose();
        }

        // Logging
        LogInformation($"{order.LoggerId}has finished unsuccessful");
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
    public virtual bool DoBasicCheckForReceivedMessage(IInboundDataMessage? receivedMessage)
    {
        // Do nothing
        return false;
    }

    /// <summary>
    /// Check if the message is an error message. Default implementaion returns always false. Override this method for other behaviour
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    /// <returns>True if the message was a handled as error message else false</returns>
    public virtual bool DoCheckForErrorMessage(IInboundDataMessage? receivedMessage)
    {
        return false;
    }

    /// <summary>
    /// Cancel the currently running operation
    /// </summary>
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
        AppLogger.LogDebug($"{DataMessagingConfig.LoggerId}Run SlotCheck.CheckCurrentAbortedUnloadOrLoadOrder");
    }


    /// <summary>
    /// Log in DEBUG mode
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="memberName">Calling method name (filled automatically by compiler)</param>
    /// <param name="filepath">Calling file name (filled automatically by compiler)</param>
    /// <param name="lineNumber">Calling method line number (filled automatically by compiler)</param>
    public virtual void LogDebug(string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        MonitorLogger.LogDebug(message, memberName, filepath, lineNumber);
        AppLogger.LogDebug($"{DataMessagingConfig.LoggerId}{message}", memberName, filepath, lineNumber);
        Trace.TraceInformation($"BaseOrdermanagementDevice: {message}");
    }

    /// <summary>
    /// Log in INFORMATION mode
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="memberName">Calling method name (filled automatically by compiler)</param>
    /// <param name="filepath">Calling file name (filled automatically by compiler)</param>
    /// <param name="lineNumber">Calling method line number (filled automatically by compiler)</param>
    public virtual void LogInformation(string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        MonitorLogger.LogInformation(message, memberName, filepath, lineNumber);
        AppLogger.LogInformation($"{DataMessagingConfig.LoggerId}{message}", memberName, filepath, lineNumber);
        Trace.TraceInformation($"BaseOrdermanagementDevice: {message}");
    }

    /// <summary>
    /// Log in WARNING mode
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="memberName">Calling method name (filled automatically by compiler)</param>
    /// <param name="filepath">Calling file name (filled automatically by compiler)</param>
    /// <param name="lineNumber">Calling method line number (filled automatically by compiler)</param>
    public virtual void LogWarning(string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        MonitorLogger.LogWarning(message, memberName, filepath, lineNumber);
        AppLogger.LogWarning($"{DataMessagingConfig.LoggerId}{message}", memberName, filepath, lineNumber);
        Trace.TraceInformation($"BaseOrdermanagementDevice: {message}");
    }

    /// <summary>
    /// Log in ERROR mode
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="memberName">Calling method name (filled automatically by compiler)</param>
    /// <param name="filepath">Calling file name (filled automatically by compiler)</param>
    /// <param name="lineNumber">Calling method line number (filled automatically by compiler)</param>
    public virtual void LogError(string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        MonitorLogger.LogError(message, memberName, filepath, lineNumber);
        AppLogger.LogError($"{DataMessagingConfig.LoggerId}{message}", memberName, filepath, lineNumber);
        Trace.TraceInformation($"BaseOrdermanagementDevice: {message}");
    }


    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        //Stop();
        //_statusWatchdog?.StopWatchDog();
        OrderManager?.StopOrderProcessing();
        OrderProcessor?.Dispose();
        CommunicationAdapter?.Dispose();
        MonitorLogger.Dispose();

        var package = DataMessagingConfig.DataMessageProcessingPackage;

        if (package == null)
        {
            return;
        }

        package.WaitStateManager.Dispose();

        foreach (var logger in package.DataLoggers)
        {
            logger.Stop();
        }
    }

    ///// <summary>
    ///// Load Device master order factory instance as current <see cref="MasterOrderFactory"/> instance
    ///// </summary>
    ///// <param name="masterOrderFactory">Current master order factory to load</param>
    //public void LoadMasterOrderFactory(IMasterOrderFactory masterOrderFactory)
    //{
    //    MasterOrderFactory = masterOrderFactory;
    //}

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

    ///// <summary>
    ///// Create a new order of a requested type
    ///// </summary>
    ///// <param name="orderType">Order type</param>
    ///// <param name="parameterSet">Current parameter set to use for the newly created order</param>
    ///// <returns>An order to let it execute by TOM</returns>
    //public IOrder CreateOrder(Type orderType, IParameterSet parameterSet)
    //{
    //    return MasterOrderFactory.GetOrder(orderType.Name, parameterSet);
    //}

    /// <summary>
    /// Cancel ongoing Device order task
    /// </summary>
    /// <param name="order"></param>
    public void CancelDeviceOrder(long order)
    {
        OrderProcessor?.CancelOrder(order);
    }

    /// <summary>
    /// Starts the Device order processing
    /// </summary>
    public void Start()
    {
        // Start order processing now
        OrderManager?.StartOrderProcessing();
    }


    /// <summary>
    /// Stops the Device order processing
    /// </summary>
    public void Stop()
    {
        //_statusWatchdog?.StopWatchDog();
        OrderManager?.StopOrderProcessing();
        MonitorLogger.LogError($"{DataMessagingConfig.LoggerId}DeviceServer stopped - ComDevClose called");
        StopComm();
    }

    ///// <summary>
    ///// Send an app notfication
    ///// </summary>
    ///// <param name="state">State management state</param>
    //public void DoNotify(IStateManagementState state)
    //{
    //    if (OrderProcessor == null)
    //    {
    //        return;
    //    }

    //    ClientNotificationManager.DoNotifyStateManagementStateEvent(this, state);


    //    //IOrder currentOrder = null;
    //    //try
    //    //{
    //    //    currentOrder = OrderProcessor.GetCurrentProcessingOrder();
    //    //}
    //    //catch (Exception e)
    //    //{
    //    //    _appLogger.LogError($"{DataMessagingConfig.LoggerId}: exception {e.Message} happened", e);
    //    //}

    //    //var terminal = 1;
    //    //try
    //    //{
    //    //    if (currentOrder?.ParameterSet is ITerminalParameterSet ps)
    //    //    {
    //    //        terminal = ps.Terminal;
    //    //        if (terminal == 0)
    //    //        {
    //    //            terminal = 1;

    //    //        }
    //    //    }
    //    //}
    //    //catch
    //    //{
    //    //    // Do nothing
    //    //}

    //    //MessagingBusinessDelegate.DoNotifyApplicationBusinessEvent(this, state.Name,
    //    //    SmdDevice.DeviceSn, state,
    //    //    ApplicationMessageEnum.Device,
    //    //    currentOrder?.Id ?? 0,
    //    //    PreviousDeviceState, terminal);

    //}

    /// <summary>
    /// Cancel the execution of orders by its sourceUid
    /// </summary>
    /// <param name="uid">Uid of the object the orders are bound to</param>
    public void CancelDeviceOrdersBySourceUid(Guid uid)
    {
        OrderProcessor?.CancelOrderBySourceUid(uid);
    }

    /// <summary>
    /// Set the order processing state
    /// </summary>
    /// <param name="isActivated">Is the order processing activated: yes or no (ComDevClose)?</param>
    protected void SetOrderProcessingStateDelegate(bool isActivated)
    {
        IsOrderProcessingActivated = isActivated;

        if (IsOrderProcessingActivated || OrderProcessor==null)
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