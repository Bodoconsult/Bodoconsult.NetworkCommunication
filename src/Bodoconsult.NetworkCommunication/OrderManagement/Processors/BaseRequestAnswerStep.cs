// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Defines a base class for request steps
/// </summary>
public class BaseRequestAnswerStep : IRequestAnswerStep
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="requestSpec">The request spec this object is bound to</param>
    public BaseRequestAnswerStep(IRequestSpec requestSpec)
    {
        RequestSpec = requestSpec ?? throw new ArgumentNullException(nameof(requestSpec));

        if (RequestSpec.ParameterSet == null)
        {
            throw new ArgumentNullException(nameof(RequestSpec.ParameterSet));
        }
    }

    /// <summary>
    /// Current request spec
    /// </summary>
    public IRequestSpec RequestSpec { get; }

    /// <summary>
    /// Allowed request answers: one of the items has to be received as answer from tower
    /// </summary>
    public List<IRequestAnswer> AllowedRequestAnswers { get; } = [];

    /// <summary>
    /// The step was successfully processed in all steps
    /// </summary>
    public virtual bool WasSuccessful { get; set; }

    /// <summary>
    /// Cancel the step
    /// </summary>
    public virtual void Cancel()
    {
        Debug.Print("RAS: cancel");
    }

    /// <summary>
    /// Total timeout for the answer(s) of a request in milliseconds
    /// </summary>
    public int Timeout { get; set; } = DeviceCommunicationBasics.DefaultTimeout;

    /// <summary>
    /// Delegate for action after cancelling the request step
    /// </summary>
    public ActionRequestStepDelegate? CancelActionDelegate { get; set; }

    /// <summary>
    /// Delegate for action before the step is executed
    /// </summary>
    public ActionRequestStepDelegate? BeforeExecuteActionDelegate { get; set; }

    /// <summary>
    /// Delegate for doing something if a RequestAnswerStep failed
    /// </summary>
    public HandleRequestAnswerStepFailedDelegate? HandleRequestAnswerStepFailedDelegate { get; set; }

    /// <summary>
    /// The state to notify the app before the step is running
    /// </summary>
    public IDeviceState StateToNotifyBeforeRunning { get; set; } = DefaultDeviceStates.DeviceStateOffline;

    /// <summary>
    /// Defines a list of async received command who break or unbreak the current request step
    /// </summary>
    public IList<char> BreakingAsyncCommands { get; } = new List<char>();

    /// <summary>
    /// Handles the answer of the request
    /// </summary>
    /// <returns>Message handling result</returns>
    public virtual MessageHandlingResult HandleResult()
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>
    /// Reset the answers for a step
    /// </summary>
    public virtual void Reset()
    {
        WasSuccessful = false;
        RequestSpec.RequestStepProcessorSetResultDelegate?.Invoke(OrderExecutionResultState.Unsuccessful);

        foreach (var answer in AllowedRequestAnswers)
        {
            answer.Reset();
        }
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public virtual void Dispose()
    {
        Reset();
    }
}