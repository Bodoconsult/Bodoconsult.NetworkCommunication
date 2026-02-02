// Copyright (c) Mycronic. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for Order Management OM top level instances
/// </summary>
public interface IOrderManager
{

    /// <summary>
    /// Current messaging config
    /// </summary>
    IDataMessagingConfig MessagingConfig { get; }
        
    /// <summary>
    /// Current order processor instance
    /// </summary>
    IOrderProcessor OrderProcessor { get; }

    /// <summary>
    /// Current order processor instance
    /// </summary>
    IOrderReceiver OrderReceiver { get; }

    ///// <summary>
    ///// Get all orders in the queue
    ///// </summary>
    //IList<IOrder> OrdersInQueue { get; }

    ///// <summary>
    ///// Get all orders currently in processing
    ///// </summary>
    //IList<IOrder> OrdersInProcessing { get; }

    ///// <summary>
    ///// No order in processing
    ///// </summary>
    //bool IsNoOrderInProcessing { get; }

    ///// <summary>
    ///// Is a previous unload still processing
    ///// </summary>
    //bool IsPreviousUnloadProcessing { get; }

    ///// <summary>
    ///// Number of orders in processing
    ///// </summary>
    //int OrdersInProcessingCount { get; }



    /// <summary>
    /// Adds a received tower message to the receiver queue for further processing
    /// </summary>
    /// <param name="order">Order to add to the message queue</param>
    void AddOrder(IOrder order);

    ///// <summary>
    ///// Cancel a running order
    ///// </summary>
    ///// <param name="order">Order to cancel</param>
    //void CancelOrder(IOrder order);


    /// <summary>
    /// Event handling method for binding to <see cref="MessagingConfig"/>.NotifyTowerMessageReceived event
    /// </summary>
    /// <param name="dataMessage">Received message</param>
    void OnTowerMessageReceived(IDataMessage dataMessage);

    /// <summary>
    /// Starts the watchdog for the order processing
    /// </summary>
    void StartOrderProcessing();


    /// <summary>
    /// Stops the watchdog for the order processing
    /// </summary>
    void StopOrderProcessing();


}