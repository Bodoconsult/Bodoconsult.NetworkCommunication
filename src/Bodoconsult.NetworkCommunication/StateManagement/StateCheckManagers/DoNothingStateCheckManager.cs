// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.StateCheckManagers;

/// <summary>
/// Current implementation of <see cref="IDeviceStateCheckManager"/> for IP devices doing nothing. Use this implemenattion if you do not need device state checks
/// </summary>
public class DoNothingStateCheckManager : IDeviceStateCheckManager
{
    private bool _isActived;

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        // Do nothing
    }

    /// <summary>
    /// Is the state check activated currently
    /// </summary>
    public bool IsActivated => _isActived;

    /// <summary>
    /// Activate the state check
    /// </summary>
    public void ActivateStateCheck()
    {
        _isActived = true;
    }

    /// <summary>
    /// Deactivate the state check
    /// </summary>
    public void DeactivateStateCheck()
    {
        _isActived = false;
    }
}