// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Fake implementation of <see cref="ICommunicationAdapter"/>
/// </summary>
public class FakeIpCommunicationAdapter : ICommunicationAdapter
{
    /// <summary>
    /// The expected PING result
    /// </summary>
    public bool PingResult { get; set; } = true;

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        // Do nothing
    }

    /// <summary>
    /// Is the device successfully pinged?
    /// </summary>
    /// <returns>True if the device was pinged successfully else false</returns>
    public Task<bool> IsPingableAsync()
    {
        return Task.FromResult(PingResult);
    }

    /// <summary>
    /// Is the adapter connected
    /// </summary>
    public bool IsConnected => PingResult;

    /// <summary>
    /// This property returns whether the communication object is valid and can be used
    /// </summary>
    public bool IsCommunicationHandlerNotNull => PingResult;

    /// <summary>
    /// Set or get the current order processing state delegate
    /// </summary>
    public SetOrderProcessingStateDelegate SetOrderProcessingStateDelegate { get; set; }

    /// <summary>
    /// Initialize the communication with the device
    /// </summary>
    /// <returns>True if the initialiazation was successfull else false</returns>
    public bool ComDevInit()
    {
        // Do nothing
        return PingResult;
    }

    public void ComDevClose()
    {
        // Do nothing
    }

    public void ComDevReset()
    {
        // Do nothing
    }

    public void ResetInternalState()
    {
        // Do nothing
    }

    /// <summary>
    /// Device configuration for data messaging
    /// </summary>
    public IIpDataMessagingConfig DataMessagingConfig { get; set; }

    /// <summary>
    /// Expected order execution result
    /// </summary>
    public List<OrderExecutionResultState> ExpectedExecutionResult { get; } = new();

    /// <summary>
    /// Send a data message to the device 
    /// </summary>
    /// <param name="command">Command to send</param>
    /// <returns>Reply of the device</returns>
    public MessageSendingResult SendDataMessage(IOutboundDataMessage command)
    {
        Task.Delay(500);
        return new MessageSendingResult(command, OrderExecutionResultState.Successful);
    }

    /// <summary>
    /// Cancel the currently running operation on the device
    /// </summary>
    public MessageSendingResult CancelRunningOperation()
    {
        return new MessageSendingResult(null, OrderExecutionResultState.Successful);
    }
}