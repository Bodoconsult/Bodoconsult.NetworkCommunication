// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for executing request steps of a device request
/// </summary>
public interface IOnlyAnswerDeviceRequestStepProcessor : IDeviceRequestStepProcessor
{
    /// <summary>
    /// Current device request spec
    /// </summary>
    public IOnlyAnswerDeviceRequestSpec OnlyAnswerDeviceRequestSpec { get; }
}