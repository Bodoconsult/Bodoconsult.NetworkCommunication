// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for checking device state manager implementations
/// </summary>
public interface IDeviceStateCheckManager : IDisposable
{
    /// <summary>
    /// Is the state check activated currently
    /// </summary>
    bool IsActivated { get; }

    /// <summary>
    /// Activate the state check
    /// </summary>
    void ActivateStateCheck();

    /// <summary>
    /// Deactivate the state check
    /// </summary>
    void DeactivateStateCheck();

}