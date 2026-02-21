// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for implementations of sending a message to the device and waiting for response (ACK, NACK or CAN)
/// </summary>
public interface ISendPacketProcess : IResetable, IDisposable
{
    /// <summary>
    /// Load dependencies
    /// </summary>
    /// <param name="duplexIo">Current IO messaging system</param>
    /// <param name="message">Current data message to send to the device</param>
    /// <param name="dataMessagingConfig">Current device</param>
    void LoadDependencies(IDuplexIo duplexIo, IOutboundDataMessage message, IDataMessagingConfig dataMessagingConfig);

    /// <summary>
    /// Current SMD device
    /// </summary>
    IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Maximum number of send attemps
    /// </summary>
    int MaxSendAttemptCount { get; set; }

    /// <summary>
    /// Timeout for waiting for an ackknowledgement
    /// </summary>
    int Timeout { get; set; }

    /// <summary>
    /// The time interval in ms between 2 tries to send the message
    /// </summary>
    int WaitingIntervalBetweenRetries { get; set; }

    /// <summary>
    /// The total transaction time for all retries for loading the next try
    /// </summary>
    int TotalTransactionTime { get; set; } 

    /// <summary>
    /// Timeout in ms for sending messages to the device
    /// </summary>
    int SendTimeout { get; set; }

    /// <summary>
    /// The total timeout for sending a message and receiving a valid handshake for it (<see cref="Timeout"/> * NumberOfRetries + <see cref="WaitingIntervalBetweenRetries"/> * (NumberOfRetries - 1) + <see cref="TotalTransactionTime"/>
    /// </summary>
    int TotalTimeout { get;  }

    /// <summary>
    /// Has the send process finished without timeout
    /// </summary>
    bool HasFinishedWithoutTimeout { get; }

    /// <summary>
    /// Current device data message to send
    /// </summary>
    IOutboundDataMessage Message { get; }
        
    /// <summary>
    /// Is the communication online?
    /// </summary>
    bool IsComOnline { get;  }

    /// <summary>
    /// Current send attempts counter
    /// </summary>
    int CurrentSendAttempsCount { get; set; }

    /// <summary>
    /// Send the message
    /// </summary>
    bool SendMessage();

    /// <summary>
    /// Excute the process
    /// </summary>
    public void Execute();

    /// <summary>
    /// The execution result of the send process
    /// </summary>
    IOrderExecutionResultState ProcessExecutionResult { get; set; }

    /// <summary>
    /// Register a wait for ACK state to be handled
    /// </summary>
    void RegisterWaitState();

    /// <summary>
    /// Unregister a wait for ACK state to be handled
    /// </summary>
    void UnregisterWaitState();

    /// <summary>
    /// Terminate the waiting task if all process states are processed
    /// </summary>
    void ProcessDone();
}