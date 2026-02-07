// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Communication.Sending;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Delegates;

#region Data messaging basics

/// <summary>
/// A delegate fired when a TCP/IP client connection was accepted
/// </summary>
/// <param name="clientSocket">Client connection socket to use by <see cref="ISocketProxy"/> implementation</param>
public delegate bool ClientConnectionAcceptedDelegate(Socket clientSocket);

/// <summary>
/// Work is in progress delegate for DuplexIO
/// </summary>
/// <returns></returns>
public delegate bool DuplexIoIsWorkInProgressDelegate();

/// <summary>
/// No data available delegate for DuplexIO
/// </summary>
public delegate void DuplexIoNoDataDelegate();

/// <summary>
/// Delegate for handling central exception handling in <see cref="IDuplexIo"/> implementations
/// </summary>
public delegate void DuplexIoErrorHandlerDelegate(Exception e);

/// <summary>
/// Delegate for unregistering a wait state from a <see cref="IWaitStateManager"/> implementation
/// </summary>
/// <param name="state">wait state to unregister</param>
public delegate void UnregisterWaitStateDelegate(SendPacketProcess state);

/// <summary>
/// A delegate for a method returning true if the communications is online or false if offline
/// </summary>
/// <returns>True if the device communication is online else false</returns>
public delegate bool CheckIfCommunicationIsOnlineDelegate();

/// <summary>
/// A delegate for a method returning true if the device is or false if not
/// </summary>
/// <returns>true if the device is ready else false</returns>
public delegate bool CheckIfDeviceIsReadyDelegate();

/// <summary>
/// Data message not sent delegate
/// </summary>
public delegate void  RaiseDataMessageNotSentDelegate(ReadOnlyMemory<byte> message, string reason);

/// <summary>
/// Message sent delegate
/// </summary>
public delegate void RaiseDataMessageSentDelegate(ReadOnlyMemory<byte> message);

/// <summary>
/// Delegate fired if a handshake has been received by the wait state manager
/// </summary>
/// <param name="message">Handshake message</param>
public delegate void RaiseDataMessageHandshakeReceivedDelegate(IDataMessage message);

/// <summary>
/// Update the data message processing package
/// </summary>
public delegate void UpdateDataMessageProcessingPackageDelegate();

/// <summary>
/// Delegate for delivering a received data message to the app next level
/// </summary>
public delegate void RaiseDataMessageReceivedDelegate(IDataMessage message);

/// <summary>
/// Unexpected data message received delegate
/// </summary>
/// <param name="message"></param>
public delegate void RaiseUnexpectedDataMessageReceivedDelegate(IDataMessage message);

/// <summary>
/// Request a closing of the current communication connection from the business logic delegate
/// </summary>
/// <param name="requestSource">Source location of the request</param>
public delegate void RaiseComDevCloseRequestDelegate(string requestSource);

#endregion

// ****** Delegates for sender and reciever ****** 
    
// ****** Receiver delegates ****** 
    
public delegate void RaisedeviceMessageNotReceivedDelegate(IDataMessage message);
public delegate void RaisedeviceMessageCorruptedDelegate(byte messageBlockAndRc, string reason);

// ****** Sender delegates ****** 

#region Order management related delegates

/// <summary>
/// Delegate for handling a received tower message
/// </summary>
/// <param name="message">Message received from tower as expected</param>
/// <returns>True if the message was handled successfully else false</returns>
public delegate bool OrderReceiverCheckMessageDelegate(IDataMessage message);

/// <summary>
/// Delegate for actions bound to request steps messages
/// </summary>
/// <param name="transportObject">Transport object to deliver a state to the method</param>
/// <param name="parameterSet">Current parameter set</param>
/// <returns>Result of the message handling</returns>
public delegate MessageHandlingResult ActionRequestStepDelegate(object transportObject, IParameterSet parameterSet);

/// <summary>
/// Delegate for sending a notification if the order state has changed
/// </summary>
/// <param name="order">The order the state was changed</param>
public delegate void DoNotifyOrderStateChangedDelegate(IOrder order);


/// <summary>
/// A delegate to implement a call back to say the <see cref="IOrderProcessor"/> that order is processed
/// </summary>
/// <param name="orderId">The ID of the order which processing has finished</param>
public delegate void OrderProcessingFinishedDelegate(long orderId);

/// <summary>
/// Delegate for handling request answer messages
/// </summary>
/// <param name="message">Message received from tower as expected</param>
/// <param name="transportObject">Transport object to deliver a state to the method</param>
/// <param name="parameterSet">Current parameter set</param>
/// <returns>Result of the message handling</returns>
public delegate MessageHandlingResult HandleRequestAnswerDelegate(ICommandDataMessage message, object transportObject, IParameterSet parameterSet);

/// <summary>
/// Delegate for handling request answer messages
/// </summary>
/// <param name="message">Message received from tower as expected</param>
/// <param name="messageHandlingResult">Message handling result built so far</param>
/// <param name="parameterSet">Current parameter set</param>
/// <returns>Result of the message handling</returns>
public delegate MessageHandlingResult HandleUnexpectedRequestAnswerDelegate(ICommandDataMessage message, MessageHandlingResult messageHandlingResult, IParameterSet parameterSet);

/// <summary>
/// Delegate for doing something if a RequestAnswerStep failed
/// </summary>
public delegate void HandleRequestAnswerStepFailedDelegate();



#endregion