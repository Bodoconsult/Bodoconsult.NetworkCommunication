// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Current implementation of <see cref="IDeviceStateCheckManager"/> for SMD towers
/// </summary>
public class StSysStateCheckManager : IDeviceStateCheckManager, IDisposable
{
    /// <summary>
    /// Current status (and humidity) request watchdog
    /// </summary>
    private readonly IWatchDog _statusWatchdog;

    private bool _isActivated;
    private readonly Lock _isActivatedLock = new();

    private readonly IOrderManagementDevice _towerServer;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="towerServer">Current tower server</param>
    public StSysStateCheckManager(IOrderManagementDevice towerServer)
    {
        _towerServer = towerServer;
        _statusWatchdog = new WatchDog(WatchDogRunnerDelegate, DeviceCommunicationBasics.TowerStatusWatchdogInterval);
        _statusWatchdog.StartWatchDog();
        _towerServer.LogInformation($"{_towerServer.LoggerId}state check init done");
    }

    private void WatchDogRunnerDelegate()
    {
        // State check is not activate
        if (!IsActivated)
        {
            return;
        }

        // Check connection
        if (!_towerServer.IsConnected)
        {
            return;
        }

        // Check if there are any priority orders
        _towerServer.CheckIfThereAreOrdersToBeCreated();

        // If connected, do the state request now
        var orders = _towerServer.CurrentState.PerpareRegularStateRequest();
        foreach (var order in orders)
        {
            var result = _towerServer.OrderProcessor.TryToExecuteOrderSync(order, false, true);
            if (result.LastStepExecutionResult != OrderExecutionResultState.Successful)
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