// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication.Sending;

/// <summary>
/// Fake implementation of <see cref="ISendPacketProcess"/> to avoid real TCP/IP sending of messages
/// </summary>
public class FakeSendPacketProcess : ISendPacketProcess
{


    /// <summary>
    /// Excute the step
    /// </summary>
    public virtual void Execute()
    {
        SendMessage();
        ProcessExecutionResult = OrderExecutionResultState.Successful;
    }

    /// <summary>
    /// Current monitor logger
    /// </summary>
    public IAppLoggerProxy MonitorLogger { get; set; }

    public void LoadDependencies(IDuplexIo duplexIo, IDataMessage message, IDataMessagingConfig smdtower)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Current SMD tower
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; set; }

    /// <summary>
    /// Maximum number of send attemps
    /// </summary>
    public int MaxSendAttemptCount { get; set; } = DeviceCommunicationBasics.MaxSendAttemptCount;

    /// <summary>
    /// Timeout for waiting for an ackknowledgement
    /// </summary>
    public int Timeout { get; set; } = 12000;

    /// <summary>
    /// The time interval in ms between 2 tries to send the message
    /// </summary>
    public int WaitingIntervalBetweenRetries { get; set; } = 5000;

    /// <summary>
    /// The total transaction time for all retries for loading the next try
    /// </summary>
    public int TotalTransactionTime { get; set; } = 5000;

    /// <summary>
    /// Timeout in ms for sending messages to the device
    /// </summary>
    public int SendTimeout { get; set; } = 5000;

    /// <summary>
    /// The total timeout for sending a message and receiving a valid handshake for it (<see cref="ISendPacketProcess.Timeout"/> * NumberOfRetries + <see cref="ISendPacketProcess.WaitingIntervalBetweenRetries"/> * (NumberOfRetries - 1) + <see cref="ISendPacketProcess.TotalTransactionTime"/>
    /// </summary>
    public int TotalTimeout => Timeout * DeviceCommunicationBasics.MaxSendAttemptCount +
                               (DeviceCommunicationBasics.MaxSendAttemptCount - 1) * WaitingIntervalBetweenRetries +
                               TotalTransactionTime;

    /// <summary>
    /// Has the send process finished without timeout
    /// </summary>
    public bool HasFinishedWithoutTimeout { get; set; }

    /// <summary>
    /// Current tower data message to send
    /// </summary>
    public IDataMessage Message { get; set; }

    /// <summary>
    /// Is the tower communication online?
    /// </summary>
    public bool IsComOnline { get; set; }

    /// <summary>
    /// Current send attempts counter
    /// </summary>
    public int CurrentSendAttempsCount { get; set; }

    /// <summary>
    /// Send the message
    /// </summary>
    public virtual bool SendMessage()
    {
        // Do nothing
        DataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(Message.RawMessageData);
        return true;
    }

    /// <summary>
    /// Is the socket connected?
    /// </summary>
    /// <returns>True if the socket is connected else false</returns>
    public bool IsSocketConnected { get; set; } = true;

    /// <summary>
    /// The execution result of the send process
    /// </summary>
    public IOrderExecutionResultState ProcessExecutionResult { get; set; }

    /// <summary>
    /// Register a wait for ACK state to be handled
    /// </summary>
    public void RegisterWaitState()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Unregister a wait for ACK state to be handled
    /// </summary>
    public void UnregisterWaitState()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Terminate the waiting task if all process states are processed
    /// </summary>
    public void ProcessDone()
    {
        // Do nothing
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        // Do nothing
    }

    public void Reset()
    {
  
    }
}