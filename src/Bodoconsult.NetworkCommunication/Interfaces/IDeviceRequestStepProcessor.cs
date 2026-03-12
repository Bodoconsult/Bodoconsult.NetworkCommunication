// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for executing request steps of a device request
/// </summary>
public interface IDeviceRequestStepProcessor : IRequestStepProcessor
{
    /// <summary>
    /// Current device request spec
    /// </summary>
    IDeviceRequestSpec DeviceRequestSpec { get; }

    /// <summary>
    /// The number of messages to be sent
    /// </summary>
    int NumberOfMessagesToBeSent { get; }

    /// <summary>
    /// The current number of messages already sent
    /// </summary>
    int CurrentNumberOfMessagesSent { get; }

    /// <summary>
    /// Prepare the chain by creating the required elements
    /// </summary>
    void PrepareTheChain();

    /// <summary>
    /// The current processed chain element
    /// </summary>
    IDeviceRequestAnswerStep? CurrentChainElement { get; set; }

    /// <summary>
    /// Check a received message
    /// </summary>
    /// <param name="receivedMessage">A message received from the device</param>
    /// <returns>True if the message was an expected answer of the current request</returns>
    bool CheckReceivedMessage(IInboundDataMessage? receivedMessage);
}