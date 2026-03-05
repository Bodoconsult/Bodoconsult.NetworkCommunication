// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factory for device request step processor instances
/// </summary>
public interface IRequestStepProcessorFactory
{
    /// <summary>
    /// Create a device request step processor
    /// </summary>
    /// <param name="requestSpec">Current request</param>
    /// <returns>A valid device request step processor instance</returns>
    IDeviceRequestStepProcessor CreateDeviceProcessor(IDeviceRequestSpec requestSpec);

    /// <summary>
    /// Create a device request step processor not waiting for an answer
    /// </summary>
    /// <param name="requestSpec">Current request</param>
    /// <returns>A valid device request step processor instance</returns>
    INoAnswerDeviceRequestStepProcessor CreateNoAnswerDeviceProcessor(INoAnswerDeviceRequestSpec requestSpec);

    /// <summary>
    /// Create a device request step processor not waiting for a handshake nor an answer
    /// </summary>
    /// <param name="requestSpec">Current request</param>
    /// <returns>A valid device request step processor instance</returns>
    INoHandshakeNoAnswerDeviceRequestStepProcessor CreateNoHandshakeNoAnswerDeviceProcessor(INoHandshakeNoAnswerDeviceRequestSpec requestSpec);

    /// <summary>
    /// Create an internal request step processor
    /// </summary>
    /// <param name="requestSpec">Current request</param>
    /// <returns>A valid device request step processor instance</returns>
    IInternalRequestStepProcessor CreateInternalProcessor(IInternalRequestSpec requestSpec);
}