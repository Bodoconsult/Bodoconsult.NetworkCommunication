// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for executing request steps of a device request
/// </summary>
public interface IInternalRequestStepProcessor : IRequestStepProcessor
{
    /// <summary>
    /// Currentinternal request spec
    /// </summary>
    IInternalRequestSpec InternalRequestSpec { get; }
}