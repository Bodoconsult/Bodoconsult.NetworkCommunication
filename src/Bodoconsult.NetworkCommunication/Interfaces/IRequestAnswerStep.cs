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
    /// Allowed request answers: one of the items has to be received as answer from device
    /// </summary>
    List<IRequestAnswer> AllowedRequestAnswers { get; }

    /// <summary>
    /// Total timeout for the answer(s) of a request in milliseconds
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
    ActionRequestStepDelegate? CancelActionDelegate { get; set; }

    /// <summary>
    /// Delegate for action before the step is executed
    /// </summary>
    ActionRequestStepDelegate? BeforeExecuteActionDelegate { get; set; }

    /// <summary>
    /// Delegate for doing something if a RequestAnswerStep failed
    /// </summary>
    HandleRequestAnswerStepFailedDelegate? HandleRequestAnswerStepFailedDelegate { get; set; }

    /// <summary>
    /// The state to notify the app before the step is running
    /// </summary>
    IDeviceState StateToNotifyBeforeRunning { get; set; }

    /// <summary>
    /// Defines a list of async received command who break or unbreak the current request step
    /// </summary>
    IList<char> BreakingAsyncCommands { get; }
}