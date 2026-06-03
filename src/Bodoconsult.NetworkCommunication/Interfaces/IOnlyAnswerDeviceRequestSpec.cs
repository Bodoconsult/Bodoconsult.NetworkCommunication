// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for device request specifications waiting for only an answer
/// </summary>
public interface IOnlyAnswerDeviceRequestSpec : IDeviceRequestSpec
{
    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    HandleRequestAnswerDelegate? HandleRequestAnswerOnSuccessDelegate { get; set; }
}