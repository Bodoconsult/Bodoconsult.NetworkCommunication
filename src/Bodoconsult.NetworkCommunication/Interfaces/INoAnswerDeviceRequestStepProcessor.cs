// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for executing request steps of a device request without expecting answer(s)
/// </summary>
public interface INoAnswerDeviceRequestStepProcessor : IRequestStepProcessor
{
    /// <summary>
    /// Current device request spec
    /// </summary>
    INoAnswerDeviceRequestSpec NoAnswerDeviceRequestSpec { get; }

    /// <summary>
    /// The number of messages to be sent
    /// </summary>
    int NumberOfMessagesToBeSent { get; }

    /// <summary>
    /// The current number of messages already sent
    /// </summary>
    int CurrentNumberOfMessagesSent { get; }
}

/// <summary>
/// Interface for executing request steps of a device request without expecting handshake or answer(s)
/// </summary>
public interface INoHandshakeNoAnswerDeviceRequestStepProcessor : IRequestStepProcessor
{
    /// <summary>
    /// Current device request spec
    /// </summary>
    INoHandshakeNoAnswerDeviceRequestSpec NoHandshakeNoAnswerDeviceRequestSpec { get; }

    /// <summary>
    /// The number of messages to be sent
    /// </summary>
    int NumberOfMessagesToBeSent { get; }

    /// <summary>
    /// The current number of messages already sent
    /// </summary>
    int CurrentNumberOfMessagesSent { get; }
}