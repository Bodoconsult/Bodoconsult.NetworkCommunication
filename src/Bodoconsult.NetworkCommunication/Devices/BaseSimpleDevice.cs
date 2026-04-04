// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices;

public abstract class BaseSimpleDevice : IIpDevice
{
    protected readonly IAppLoggerProxy AppLogger;
    protected readonly IAppLoggerProxy MonitorLogger;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    protected BaseSimpleDevice(IDataMessagingConfig dataMessagingConfig, ICentralClientNotificationManager clientNotificationManager)
    {
        DataMessagingConfig = dataMessagingConfig ?? throw new ArgumentNullException(nameof(dataMessagingConfig));
        AppLogger = dataMessagingConfig.AppLogger;
        MonitorLogger = dataMessagingConfig.MonitorLogger;
        ClientNotificationManager = clientNotificationManager;
    }

    /// <summary>
    /// Device configuration for data messaging
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; protected set; }

    /// <summary>
    /// Communication adapter to use for order management
    /// </summary>
    public ICommunicationAdapter? CommunicationAdapter { get; protected set; }

    /// <summary>
    /// Client notification manager
    /// </summary>
    public ICentralClientNotificationManager ClientNotificationManager { get; }

    /// <summary>
    /// Current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public IDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter { get; protected set; }

    /// <summary>
    /// Is the device connected?
    /// </summary>
    public bool IsConnected
    {
        get
        {
            if (CommunicationAdapter == null)
            {
                return false;
            }

            if (CommunicationAdapter.IsConnected)
            {
                return true;
            }

            // RL: ComDev should be reset to initial state here under all circumstances to avoid any hanging comm parts
            CommunicationAdapter.ComDevReset();
            //_monitorLogger?.LogError($"Ping NOT successful for IP {SmdDevice.IpAddress}");
            return false;
        }
    }

    /// <summary>
    /// Load the current <see cref="IDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    /// <param name="businessLogicAdapter">Current <see cref="IDeviceBusinessLogicAdapter"/> instance</param>
    public void LoadDeviceBusinessLogicAdapter(IDeviceBusinessLogicAdapter businessLogicAdapter)
    {
        DeviceBusinessLogicAdapter = businessLogicAdapter;
    }

    /// <summary>
    /// Start the communication
    /// </summary>
    public virtual void StartComm()
    {
        CommunicationAdapter?.ComDevInit();
    }

    /// <summary>
    /// Reset the communication
    /// </summary>
    public virtual void ResetComm()
    {
        CommunicationAdapter?.ComDevReset();
    }

    /// <summary>
    /// Stop the communication
    /// </summary>
    public void StopComm()
    {
        CommunicationAdapter?.ComDevClose();
    }

    /// <summary>
    /// Is the device pingable
    /// </summary>
    /// <returns>True if the device is pingable else false</returns>
    public virtual bool IsPingable => CommunicationAdapter?.IsPingableAsync().GetAwaiter().GetResult() ?? false;

    /// <summary>
    /// Load a communication adapter
    /// </summary>
    /// <param name="commAdapter">Current communication adapter</param>
    public void LoadCommAdapter(ICommunicationAdapter commAdapter)
    {
        CommunicationAdapter = commAdapter;
    }

    /// <summary>
    /// Clear the internal state without breaking comm
    /// </summary>
    public virtual void ResetInternalState()
    {
        CommunicationAdapter?.ResetInternalState();
        AppLogger.LogInformation("Internal state was reset");
    }

    /// <summary>
    /// Log in DEBUG mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public virtual void LogDebug(string message)
    {
        MonitorLogger.LogDebug(message);
        AppLogger.LogDebug($"{DataMessagingConfig.LoggerId}{message}");
        Debug.Print($"STS: {message}");
    }

    /// <summary>
    /// Log in INFORMATION mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public virtual void LogInformation(string message)
    {
        MonitorLogger.LogInformation(message);
        AppLogger.LogInformation($"{DataMessagingConfig.LoggerId}{message}");
        Debug.Print($"STS: {message}");
    }

    /// <summary>
    /// Log in WARNING mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public virtual void LogWarning(string message)
    {
        MonitorLogger.LogWarning(message);
        AppLogger.LogWarning($"{DataMessagingConfig.LoggerId}{message}");
        Debug.Print($"STS: {message}");
    }

    /// <summary>
    /// Log in ERROR mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public virtual void LogError(string message)
    {
        MonitorLogger.LogError(message);
        AppLogger.LogError($"{DataMessagingConfig.LoggerId}{message}");
        Debug.Print($"STS: {message}");
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        CommunicationAdapter?.Dispose();
        MonitorLogger.Dispose();
        DataMessagingConfig.DataMessageProcessingPackage?.WaitStateManager.Dispose();
    }
}