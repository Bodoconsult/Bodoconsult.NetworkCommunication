// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.StateCheckManagers;

/// <summary>
/// Current implementation of <see cref="IDeviceStateCheckManager"/> for IP devices
/// </summary>
public class DefaultStateCheckManager : IDeviceStateCheckManager
{
    /// <summary>
    /// Current status (and humidity) request watchdog
    /// </summary>
    private readonly IWatchDog _statusWatchdog;

    private bool _isActivated;
    private readonly Lock _isActivatedLock = new();

    private readonly IStateManagementDevice _device;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current tower server</param>
    public DefaultStateCheckManager(IStateManagementDevice device)
    {
        _device = device;
        _statusWatchdog = new WatchDog(WatchDogRunnerDelegate, DeviceCommunicationBasics.DeviceStatusWatchdogInterval);
        _statusWatchdog.StartWatchDog();
        _device.LogInformation($"{_device.DataMessagingConfig.LoggerId}state check init done");
    }

    private void WatchDogRunnerDelegate()
    {
        ArgumentNullException.ThrowIfNull(_device.OrderManager);

        // State check is not activate
        if (!IsActivated)
        {
            return;
        }

        // Check connection
        if (!_device.IsConnected)
        {
            return;
        }

        // Check if there are any priority orders
        _device.CheckIfThereAreOrdersToBeCreated();

        if (_device.CurrentState == null)
        {
            return;
        }

        // If connected, do the state request now
        var orders = _device.CurrentState.PerpareRegularStateRequest();
        foreach (var order in orders)
        {
            var result = _device.OrderManager.OrderProcessor.TryToExecuteOrderSync(order, false, true);
            if (result != OrderExecutionResultState.Successful)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Is the state check activated currently
    /// </summary>
    public bool IsActivated
    {
        get
        {
            lock (_isActivatedLock)
            {
                return _isActivated;
            }
        }
        private set
        {
            lock (_isActivatedLock)
            {
                _isActivated = value;
            }
        }
    }

    /// <summary>
    /// Activate the state check
    /// </summary>
    public void ActivateStateCheck()
    {
        IsActivated = true;
    }

    /// <summary>
    /// Deactivate the state check
    /// </summary>
    public void DeactivateStateCheck()
    {
        IsActivated = false;
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _statusWatchdog.StopWatchDog();
    }
}