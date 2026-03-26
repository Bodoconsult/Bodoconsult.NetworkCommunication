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
public delegate void RaiseDataMessageNotSentDelegate(ReadOnlyMemory<byte> message, string? reason);

/// <summary>
/// Message sent delegate
/// </summary>
public delegate void RaiseDataMessageSentDelegate(ReadOnlyMemory<byte> message);

/// <summary>
/// Delegate fired if a handshake has been received by the wait state manager
/// </summary>
/// <param name="message">Handshake message</param>
public delegate void RaiseDataMessageHandshakeReceivedDelegate(IInboundHandShakeMessage message);

/// <summary>
/// Update the data message processing package
/// </summary>
public delegate void UpdateDataMessageProcessingPackageDelegate();

/// <summary>
/// Delegate for delivering a received data message to the app next level
/// </summary>
public delegate void RaiseDataMessageReceivedDelegate(IInboundDataMessage message);

/// <summary>
/// Unexpected data message received delegate
/// </summary>
/// <param name="message"></param>
public delegate void RaiseUnexpectedDataMessageReceivedDelegate(IInboundDataMessage message);

/// <summary>
/// Request a closing of the current communication connection from the business logic delegate
/// </summary>
/// <param name="requestSource">Source location of the request</param>
public delegate void RaiseComDevCloseRequestDelegate(string requestSource);

#endregion

// ****** Delegates for sender and reciever ****** 

// ****** Receiver delegates ****** 

public delegate void RaisedeviceMessageNotReceivedDelegate(IInboundDataMessage message);
public delegate void RaisedeviceMessageCorruptedDelegate(byte messageBlockAndRc, string reason);

// ****** Sender delegates ****** 

#region Order management related delegates

/// <summary>
/// Set the order processing state delegate
/// </summary>
/// <param name="isActivated">Is the order processing activated: true or false?</param>
public delegate void SetOrderProcessingStateDelegate(bool isActivated);

/// <summary>
/// Delegate for handling a received device message
/// </summary>
/// <param name="message">Message received from device as expected</param>
/// <returns>True if the message was handled successfully else false</returns>
public delegate bool OrderReceiverCheckMessageDelegate(IInboundDataMessage message);

/// <summary>
/// Delegate for actions bound to request steps messages
/// </summary>
/// <param name="transportObject">Transport object to deliver a state to the method</param>
/// <param name="parameterSet">Current parameter set</param>
/// <returns>Result of the message handling</returns>
public delegate MessageHandlingResult ActionRequestStepDelegate(object? transportObject, IParameterSet? parameterSet);

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
/// <param name="message">Message received from device as expected</param>
/// <param name="transportObject">Transport object to deliver a state to the method</param>
/// <param name="parameterSet">Current parameter set</param>
/// <returns>Result of the message handling</returns>
public delegate MessageHandlingResult HandleRequestAnswerDelegate(IInboundDataMessage? message, object? transportObject, IParameterSet? parameterSet);

/// <summary>
/// Delegate for handling request answer messages
/// </summary>
/// <param name="message">Message received from device as expected</param>
/// <param name="messageHandlingResult">Message handling result built so far</param>
/// <param name="parameterSet">Current parameter set</param>
/// <returns>Result of the message handling</returns>
public delegate MessageHandlingResult HandleUnexpectedRequestAnswerDelegate(IInboundDataMessage message, MessageHandlingResult messageHandlingResult, IParameterSet parameterSet);

/// <summary>
/// Delegate for doing something if a RequestAnswerStep failed
/// </summary>
public delegate void HandleRequestAnswerStepFailedDelegate();

/// <summary>
/// Delegate to set the state for a <see cref="IRequestStepProcessor"/> instance
/// </summary>
/// <param name="state">State to set</param>
public delegate void RequestStepProcessorSetResultDelegate(IOrderExecutionResultState state);

/// <summary>
/// Is the <see cref="IRequestStepProcessor"/> instance cancelled?
/// </summary>
/// <returns>True, if cancelled else false</returns>
public delegate bool RequestStepProcessorIsCancelledDelegate();

/// <summary>
/// Send an app notfication
/// </summary>
/// <param name="stateMachineState">Business or device state to send the notification for</param>
public delegate void DoNotifyDelegate(IStateMachineState stateMachineState);

/// <summary>
/// Delegate to cancel running operation on comm adapter level
/// </summary>
public delegate MessageSendingResult CancelRunningOperationDelegate();

/// <summary>
/// Send a data message to the device
/// </summary>
/// <param name="command">Command to send</param>
/// <returns>Reply of the device</returns>
public delegate MessageSendingResult SendDataMessageDelegate(IOutboundDataMessage command);

/// <summary>
/// Reset the <see cref="IOutboundDataMessageFactory"/>
/// </summary>
public delegate void ResetOutboundDataMessageFactoryDelegate();

/// <summary>
/// Check if a received message is the expected answer to the request.
/// If the message is the requested answer from the device the properties <see cref="IRequestAnswer.WasReceived"/>
/// and <see cref="IRequestAnswer.ReceivedMessage"/> are set to true and the received message.
/// </summary>
/// <param name="sentMessage">The message sent from the request to the device</param>
/// <param name="receivedMessage">A received message from the device</param>
/// <param name="errors">List with error messages to fill</param>
/// <returns>True if the message was as expected as answer of the sent message else false</returns>
public delegate bool CheckReceivedMessageDelegate(IRequestAnswer requestAnswer, IOutboundDataMessage sentMessage, IInboundDataMessage? receivedMessage, IList<string> errors);

/// <summary>
/// Delegate for creating data messages to sent to the device
/// </summary>
/// <param name="parameterSet">Current parameter set</param>
/// <returns>List with orders to send to the device</returns>
public delegate List<IOutboundDataMessage> CreateMessagesToSentDelegate(IParameterSet? parameterSet);




#endregion

#region State management



/// <summary>
/// Delegate to create one or more orders needed as regular state request to the device
/// </summary>
/// <returns></returns>
public delegate List<IOrder> PrepareRegularStateRequestDelegate();

/// <summary>
/// Delegate to be executed from an orderless state machine state
/// </summary>
/// <param name="state">Current orderless state machine state</param>
public delegate void ExecuteActionForStateDelegate(IOrderlessActionStateMachineState state);

/// <summary>
/// Delegate to be executed from a no action state machine state to check if job states are required to be executed
/// </summary>
/// <param name="state">Current no action state machine state</param>
public delegate void CheckJobstatesActionForStateDelegate(INoActionStateMachineState state);

/// <summary>
/// Delegate to cancel a state
/// </summary>
public delegate void CancelStateDelegate(IOrderlessActionStateMachineState state);

/// <summary>
/// Delegate to handle a ComDevClose event
/// </summary>
/// <param name="state">Current state machine state</param>
public delegate void HandleComDevCloseDelegate(IStateMachineState state);

/// <summary>
/// Handle an async received message
/// </summary>
/// <param name="state">Current state machine state</param>
/// <param name="message">Current received message</param>
/// <returns>Message handling result</returns>
public delegate MessageHandlingResult HandleAsyncMessageDelegate(IStateMachineState state, IInboundDataMessage?  message);

/// <summary>
/// Handle an async received BTCP message
/// </summary>
/// <param name="message">Current received message</param>
/// <returns>Message handling result</returns>
public delegate void HandleAsyncBtcpMessageDelegate(IInboundBusinessTransactionDataMessage message);

/// <summary>
/// Handle an async received message without state machine
/// </summary>
/// <param name="message">Current received message</param>
/// <returns>Message handling result</returns>
public delegate MessageHandlingResult NoStateMachineHandleAsyncMessageDelegate(IInboundDataMessage? message);

/// <summary>
/// Handle an error message received from the device
/// </summary>
/// <param name="state">Current state machine state</param>
/// <param name="message">Current received message</param>
/// <returns>Message handling result</returns>
public delegate void HandleErrorMessageDelegate(IStateMachineState state, IInboundDataMessage message);

/// <summary>
/// Handle an error message received from the device without state machine
/// </summary>
/// <param name="message">Current received message</param>
/// <returns>Message handling result</returns>
public delegate void NoStateMachineHandleErrorMessageDelegate(IInboundDataMessage message);

/// <summary>
/// Delegate for handling device state check request answers in business logic
/// </summary>
/// <param name="state">Current state machine state</param>
/// <param name="message">Current received message</param>
/// <param name="doNotNotifyClient">Do not notify client? True or false.</param>
/// <returns>Message handling result</returns>
public delegate MessageHandlingResult HandleRegularStateRequestAnswerDelegate(IStateMachineState state, IInboundDataMessage message, bool doNotNotifyClient);

/// <summary>
/// Delegate fired when an order was finished successfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
/// </summary>
/// <param name="state">Current state machine state</param>
/// <param name="order">Current successful finished order</param>
public delegate void OrderFinishedSucessfullyDelegate(IStateMachineState state, IOrder order);

/// <summary>
/// Delegate fired when an order was finished unsuccessfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
/// </summary>
/// <param name="state">Current state machine state</param>
/// <param name="order">Current unsuccessfully finished order</param>
public delegate void OrderFinishedUnsucessfullyDelegate(IStateMachineState state, IOrder order);

/// <summary>
/// Delegate for creating preconfigured parametersets
/// </summary>
/// <returns>Preconfigured parameterset</returns>
public delegate IParameterSet CreateParameterSetDelegate();


#endregion