// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="IRequestStepProcessor"/> for internal requests
/// </summary>
public class InternalRequestStepProcessor : IInternalRequestStepProcessor
{
    private readonly Lock _cancelLockObject = new();
    private IRequestAnswerStep? _currentChainElement;
    private readonly Lock _currentChainElementObject = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="requestSpec">Current request spec to execute</param>
    public InternalRequestStepProcessor(IInternalRequestSpec requestSpec)
    {
        RequestSpec = requestSpec;
        InternalRequestSpec = requestSpec;

        AppLogger = requestSpec.AppLogger ?? throw new ArgumentNullException(nameof(requestSpec.AppLogger));
        OrderLoggerId = $"{requestSpec.OrderLoggerId}";
    }

    /// <summary>
    /// Currentinternal request spec
    /// </summary>
    public IInternalRequestSpec InternalRequestSpec { get; }

    /// <summary>
    /// Current app logger
    /// </summary>
    public IAppLoggerProxy AppLogger { get; }

    /// <summary>
    /// Current order ID for logging
    /// </summary>
    public string OrderLoggerId { get; }

    /// <summary>
    /// The current processed chain element
    /// </summary>
    public IRequestAnswerStep? CurrentChainElement
    {
        get
        {
            lock (_currentChainElementObject)
            {
                return _currentChainElement;
            }
        }
        set
        {
            lock (_currentChainElementObject)
            {
                _currentChainElement = value;
            }
        }
    }

    /// <summary>
    /// Check if cancelled
    /// </summary>
    /// <returns>True if cancelled else false</returns>
    public bool CheckIsCancelled()
    {
        return false;
    }

    /// <summary>
    /// Set the result state
    /// </summary>
    /// <param name="state">State to set as result</param>
    public void SetResult(IOrderExecutionResultState state)
    {
        Result = state;
    }

    /// <summary>
    /// Current request spec to use for the processor
    /// </summary>
    public IRequestSpec RequestSpec { get; set; }

    /// <summary>
    /// Is the request processor cancelled
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Current execution result
    /// </summary>
    public IOrderExecutionResultState Result { get; set; } = OrderExecutionResultState.Unsuccessful;

    /// <summary>
    /// Prepare the chain by creating the required elements
    /// </summary>
    public void PrepareTheChain()
    {
        // Do nothing
    }

    /// <summary>
    /// Execute the request
    /// </summary>
    /// <returns>Execution result</returns>
    public IOrderExecutionResultState ExecuteRequest()
    {
        try
        {
            // Fetch the request spec here to avoid multithread issues
            var requestSpec = InternalRequestSpec;

            if (IsCancelled)
            {
                return OrderExecutionResultState.Unsuccessful;
            }

            var result = ExecuteInternalLoop(requestSpec);

            if (result.Id == OrderExecutionResultState.Successful.Id)
            {
                requestSpec.WasSuccessful = true;
            }

            return result;

        }
        catch (Exception e)
        {
            AppLogger.LogError("Execution of internal request step failed", e);
            return OrderExecutionResultState.Unsuccessful;
        }
    }

    /// <summary>
    /// Execute internal request specs in a loop
    /// </summary>
    /// <param name="requestSpec">Current request spec</param>
    /// <returns>Execution result</returns>
    private IOrderExecutionResultState ExecuteInternalLoop(IInternalRequestSpec requestSpec)
    {
        var repeatCount = 0;
        IOrderExecutionResultState result;
        // Repeat loop if necessary if the execution result is NOT successful
        while (true)
        {
            if (IsCancelled)
            {
                return OrderExecutionResultState.Unsuccessful;
            }

            result = ExecuteInternal(requestSpec);
            repeatCount++;

            if (result.Id == OrderExecutionResultState.Successful.Id ||
                repeatCount >= requestSpec.NumberOfRepeatsInCaseOfNoSuccess)
            {
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Execute a request spec which is internal only
    /// </summary>
    /// <param name="requestSpec"></param>
    /// <returns></returns>
    private IOrderExecutionResultState ExecuteInternal(IInternalRequestSpec requestSpec)
    {
        var step = requestSpec.RequestAnswerSteps[0];

        var s = $"{OrderLoggerId}RequestAnswerStep {0}: prepare start";
        Trace.TraceInformation(s);
        AppLogger.LogDebug(s);

        if (IsCancelled)
        {
            return OrderExecutionResultState.Unsuccessful;
        }

        // Call action before step is executed if necessary
        step.BeforeExecuteActionDelegate?.Invoke(requestSpec.TransportObject, requestSpec.ParameterSet);

        if (IsCancelled)
        {
            return OrderExecutionResultState.Unsuccessful;
        }

        // Internal request are always successful at this point
        step.WasSuccessful = true;

        // Now execute the internal request
        var result = step.HandleResult();
        return result.ExecutionResult;
    }


    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the tower</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    public bool CheckReceivedMessage(IInboundDataMessage receivedMessage)
    {
        return false;
    }

    /// <summary>
    /// Cancel the current request step processor
    /// </summary>
    public void Cancel()
    {
        lock (_cancelLockObject)
        {
            IsCancelled = true;
        }

        foreach (var x in InternalRequestSpec.RequestAnswerSteps.Where(x => x.WasSuccessful))
        {
            x.WasSuccessful = false;
        }

        Dispose();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        CurrentChainElement = null;
    }
}