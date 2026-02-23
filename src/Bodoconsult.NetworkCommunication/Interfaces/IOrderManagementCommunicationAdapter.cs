// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Set the order processing state delegate
/// </summary>
/// <param name="isActivated">Is the order processing activated: true or false?</param>
public delegate void SetOrderProcessingStateDelegate(bool isActivated);

/// <summary>
/// Communication adapter used for order management
/// </summary>
public interface IOrderManagementCommunicationAdapter
{
    /// <summary>
    /// Is the device successfully pinged?
    /// </summary>
    /// <returns>True if the device was pinged successfully else false</returns>
    Task<bool> IsPingableAsync();

    /// <summary>
    /// Is the adapter connected
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    ///     This property returns whether the communication object is valid i.e
    ///     and can be used
    /// </summary>
    bool IsCommunicationHandlerNotNull { get; }

    /// <summary>
    /// Set or get the current order processing state delegate
    /// </summary>
    SetOrderProcessingStateDelegate SetOrderProcessingStateDelegate { get; set; }

    /// <summary>
    /// Initialize the communication with the device
    /// </summary>
    /// <returns>True if the initialiazation was successfull else false</returns>
    bool ComDevInit();

    /// <summary>
    /// Close the communication channel with the device
    /// </summary>
    void ComDevClose();

    /// <summary>
    /// Reset the com dev to a defined state as if there were never a communication with the device. No logging for ComDevClose activated
    /// </summary>
    void ComDevReset();

    /// <summary>
    /// Reset the com dev without breaking the communication
    /// </summary>
    void ResetInternalState();

    /// <summary>
    /// Device configuration for data messaging
    /// </summary>
    IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Send a command to the device
    /// </summary>
    /// <param name="command">Command to send</param>
    /// <returns>Reply of the device</returns>
    MessageSendingResult SendCommandDataMessage(IOutboundDataMessage command);

    /// <summary>
    /// Cancel the currently running operation on the device
    /// </summary>
    MessageSendingResult CancelRunningOperation();



}