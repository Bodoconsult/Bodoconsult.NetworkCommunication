// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for executing request steps of a device request
/// </summary>
public interface IRequestStepProcessor : IDisposable
{
    /// <summary>
    /// Current execution result
    /// </summary>
    IOrderExecutionResultState Result { get; set; } 

    /// <summary>
    /// Current request spec to use for the processor
    /// </summary>
    IRequestSpec RequestSpec { get; set; }

    /// <summary>
    /// The number of messages to be sent
    /// </summary>
    int NumberOfMessagesToBeSent { get; }

    /// <summary>
    /// The current number of messages already sent
    /// </summary>
    int CurrentNumberOfMessagesSent { get; }

    /// <summary>
    /// Is the request processor cancelled
    /// </summary>
    bool IsCancelled { get; }

    /// <summary>
    /// Prepare the chain by creating the required elements
    /// </summary>
    void PrepareTheChain();

    /// <summary>
    /// Execute the request
    /// </summary>
    /// <returns>Execution result</returns>
    IOrderExecutionResultState ExecuteRequest();

    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the device</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    bool CheckReceivedMessage(IInboundDataMessage receivedMessage);

    /// <summary>
    /// Cancel the current request step processor
    /// </summary>
    void Cancel();

    /// <summary>
    /// The current processed chain element
    /// </summary>
    IRequestAnswerStep CurrentChainElement { get; set; }

    /// <summary>
    /// Set the result state
    /// </summary>
    /// <param name="state">State to set as result</param>
    void SetResult(IOrderExecutionResultState state);

    /// <summary>
    /// Check if cancelled
    /// </summary>
    /// <returns>True if cancelled else false</returns>
    bool CheckIsCancelled();
}