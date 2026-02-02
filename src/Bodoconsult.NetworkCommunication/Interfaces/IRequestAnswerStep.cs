// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for defining a request answer step
/// </summary>
public interface IRequestAnswerStep : IDisposable
{
    /// <summary>
    /// Current request spec
    /// </summary>
    IRequestSpec RequestSpec { get; }

    /// <summary>
    /// Allowed request answers: one of the items has to be received as answer from tower
    /// </summary>
    IList<IRequestAnswer> AllowedRequestAnswers { get; }

    /// <summary>
    /// The next step in the chain
    /// </summary>
    IRequestAnswerStep NextChainElement { get; set; }

    ///// <summary>
    ///// The chain the step is member of
    ///// </summary>
    //IRequestStepProcessor CurrentRequestStepProcessor { get; set; }

    /// <summary>
    /// Total timeout for the answer(s) of an request in milliseconds
    /// </summary>
    int Timeout { get; set; }


    /// <summary>
    /// The step was successfully processed in all steps
    /// </summary>
    bool WasSuccessful { get; set; }

    /// <summary>
    /// Cancel the step
    /// </summary>
    void Cancel();

    /// <summary>
    /// Check the received message is fitting to the current request
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    /// <returns>True if the received message is fitting to the current request else false</returns>
    IEnumerable<string> CheckReceivedMessage(ICommandDataMessage receivedMessage);

    /// <summary>
    /// Handles the answer of the request step
    /// </summary>
    /// <returns>Message handling result</returns>
    MessageHandlingResult HandleResult();

    /// <summary>
    /// Reset the answers for a step
    /// </summary>
    void Reset();

    /// <summary>
    /// Delegate for action after cancelling the request step
    /// </summary>
    ActionRequestStepDelegate CancelActionDelegate { get; set; }

    /// <summary>
    /// Delegate for action before the step is executed
    /// </summary>
    ActionRequestStepDelegate BeforeExecuteActionDelegate { get; set; }

    /// <summary>
    /// Delegate for doing something if a RequestAnswerStep failed
    /// </summary>
    HandleRequestAnswerStepFailedDelegate HandleRequestAnswerStepFailedDelegate { get; set; }


    /// <summary>
    /// The state to notify the app before the step is running
    /// </summary>
    IDeviceState StateToNotifyBeforeRunning { get; set; }

    /// <summary>
    /// Defines a list of async received command who break or unbreak the current request step
    /// </summary>
    IList<char> BreakingAsyncCommands { get; }

    /// <summary>
    /// Process the current request answer step in a chain
    /// </summary>
    public void ProcessChainElement();

    /// <summary>
    /// Check if a received message is the expected answer to the request step.
    /// </summary>
    /// <param name="sentMessage">The message sent from the request to the tower</param>
    /// <param name="receivedMessage">A received message from the tower</param>
    /// <param name="errors">List with error messages to fill</param> <returns>True if the message was as expected as answer of the sent message else false</returns>
    bool CheckReceivedMessage(ICommandDataMessage sentMessage, ICommandDataMessage receivedMessage, IList<string> errors);

    /// <summary>
    /// The accepted message leading <see cref="WasSuccessful"/> being true
    /// </summary>
    ICommandDataMessage AcceptedMessage { get; }
}