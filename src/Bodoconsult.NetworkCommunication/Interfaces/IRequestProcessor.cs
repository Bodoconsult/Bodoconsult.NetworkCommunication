// Copyright (c) Mycronic. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for a request processor implementations
/// </summary>
public interface IRequestProcessor: IDisposable
{
    /// <summary>
    /// Current order to process
    /// </summary>
    IOrder Order { get; }

    /// <summary>
    /// The currently processed request step of the order
    /// </summary>
    IRequestStepProcessor CurrentRequestStepProcessor { get; }

    /// <summary>
    /// Prepare the chain by creating the required elements
    /// </summary>
    public void PrepareTheChain();

    /// <summary>
    /// Execute the order request by request
    /// </summary>
    /// <returns>Execution result</returns>
    IOrderExecutionResultState ExecuteOrder();


    /// <summary>
    /// Execute a single step
    /// </summary>
    /// <param name="requestSpec">Current request spec</param>
    /// <returns>Execution result</returns>
    IOrderExecutionResultState ExecuteRequest(IRequestSpec requestSpec);


    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the tower</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    bool CheckReceivedMessage(ICommandDataMessage receivedMessage);


    /// <summary>
    /// Cancel a not running processor
    /// </summary>
    void Cancel();

    /// <summary>
    /// Is the request processor cancelled
    /// </summary>
    public bool IsCancelled { get; }

    /// <summary>
    /// Cancel the processor
    /// </summary>
    /// <param name="isRunning">Is the order running at the moment</param>
    /// <param name="isHardwareError">Is the reason for cancelling a hardware error</param>
    void Cancel(bool isRunning, bool isHardwareError);


    /// <summary>
    /// A delegate to implement a call back to say the <see cref="IOrderProcessor"/> that order is processed
    /// </summary>
    OrderProcessingFinishedDelegate OrderProcessingFinishedDelegate { get; set; }


    /// <summary>
    /// The task the <see cref="ExecuteOrder"/> command is running in
    /// </summary>
    Task CurrentTask { get; set; }

    /// <summary>
    /// Used to cancel <see cref="CurrentTask"/> if required
    /// </summary>
    CancellationTokenSource CancellationTokenSource { get; } 

}