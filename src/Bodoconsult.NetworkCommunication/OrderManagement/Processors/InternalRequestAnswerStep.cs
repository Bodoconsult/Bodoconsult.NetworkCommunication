// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Defines a device bound request step
/// </summary>
public class InternalRequestAnswerStep : BaseRequestAnswerStep, IInternalRequestAnswerStep
{
    /// <summary>
    /// The internal request spec instance
    /// </summary>
    public IInternalRequestSpec InternalRequestSpec { get; }

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="requestSpec">The request spec this object is bound to</param>
    public InternalRequestAnswerStep(IInternalRequestSpec requestSpec) : base(requestSpec)
    {
        InternalRequestSpec = requestSpec;
    }

    /// <summary>
    /// Handles the answer of the request
    /// </summary>
    /// <returns>Message handling result</returns>
    public override MessageHandlingResult HandleResult()
    {
        var requestSpec = InternalRequestSpec;

        //if (requestSpec == null)
        //{
        //    return new MessageHandlingResult
        //    {
        //        Error = 1,
        //        ExecutionResult = OrderExecutionResultState.Unsuccessful,
        //        ErrorDescription = "No RequestSpec instance"
        //    };
        //}

        //var rsp = requestSpec.CurrentRequestStepProcessor;
        //if (rsp == null)
        //{
        //    return new MessageHandlingResult
        //    {
        //        ExecutionResult = OrderExecutionResultState.Unsuccessful,
        //        ErrorDescription = "No CurrentRequestStepProcessor instance"
        //    };
        //}

        var answer = AllowedRequestAnswers.FirstOrDefault();

        if (answer == null)
        {
            return new MessageHandlingResult
            {
                Error = 2,
                ExecutionResult = OrderExecutionResultState.Unsuccessful,
                ErrorDescription = "received msg doesn't fit"
            };
        }

        // If no answer on success delegate delivered leave here
        if (answer.HandleRequestAnswerOnSuccessDelegate == null)
        {
            return new MessageHandlingResult
            {
                ExecutionResult = OrderExecutionResultState.Successful
            };
        }

        var run = 0;
        while (true)
        {
            MessageHandlingResult result;
            try
            {
                if (answer.ReceivedMessage == null)
                {
                    return new MessageHandlingResult
                    {
                        Error = 5,
                        ExecutionResult = OrderExecutionResultState.Unsuccessful,
                        ErrorDescription = "Received message is null"
                    };
                }
                Debug.Print($"{answer.HandleRequestAnswerOnSuccessDelegate.Method.Name}");
                Debug.Print("Start delegate...");
                result = answer.HandleRequestAnswerOnSuccessDelegate.Invoke(answer.ReceivedMessage, requestSpec.TransportObject, requestSpec.ParameterSet);
                Debug.Print("End delegate...");
            }
            catch (Exception e)
            {
                return new MessageHandlingResult
                {
                    Error = 3,
                    ExecutionResult = OrderExecutionResultState.Unsuccessful,
                    ErrorDescription = e.ToString()
                };
            }

            if (result.ExecutionResult.Id == OrderExecutionResultState.Successful.Id)
            {
                // Return the result directly to transport the TransportObject to the next step
                return result;
            }

            run++;
            if (run <= 1)
            {
                continue;
            }

            return new MessageHandlingResult
            {
                Error = 4,
                ExecutionResult = OrderExecutionResultState.Unsuccessful,
                ErrorDescription = "run > 1"
            };
        }
    }
}