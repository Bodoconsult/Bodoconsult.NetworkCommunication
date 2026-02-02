// Copyright (c) Mycronic. All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for executing request steps of a tower request
/// </summary>
public interface IRequestStepProcessor : IDisposable
{
    /// <summary>
    /// Current app logger
    /// </summary>
    IAppLoggerProxy AppLogger { get;  }

    /// <summary>
    /// Current order ID for logging
    /// </summary>
    string OrderLoggerId  { get;  }

    /// <summary>
    /// Current execution result
    /// </summary>
    IOrderExecutionResultState Result { get; set; } 

    /// <summary>
    /// Current request spec to use for the processor
    /// </summary>
    IRequestSpec RequestSpec { get; set; }

    /// <summary>
    /// Message to send to tower
    /// </summary>
    public ICommandDataMessage SentMessage { get; }

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
    public bool IsCancelled { get; }

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
    /// <param name="receivedMessage">A message received from the tower</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    bool CheckReceivedMessage(ICommandDataMessage receivedMessage);


    /// <summary>
    /// Cancel the current request step processor
    /// </summary>
    void Cancel();

    /// <summary>
    /// The current processed chain element
    /// </summary>
    IRequestAnswerStep CurrentChainElement { get; set; }

}