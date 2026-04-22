// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.ClientNotifications;
using Bodoconsult.NetworkCommunication.ClientNotifications.Notifications;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.ClientNotifications;

/// <summary>
/// An <see cref="IClient"/> instance implementing client notifications for an <see cref="IIpDevice"/> using BTCP notifications
/// </summary>
public class BtcpIpDeviceClient: IClient
{
    private readonly IAppLoggerProxy _logger;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current IP device</param>
    /// <param name="logger">Current app logger</param>
    public BtcpIpDeviceClient(IIpDevice device, IAppLoggerProxy logger)
    {
        Device = device;
        _logger = logger;
        AllowedNotifications.Add(nameof(StateMachineStateNotification));
    }

    /// <summary>Wait until connection result is set</summary>
    /// <returns>true if connected</returns>
    public Task<bool> WaitForConnectionResult()
    {
        return Task.FromResult(true);
    }

    /// <summary>Start the connection with the client</summary>
    public void StartConnection()
    {
        // Do nothing
    }

    /// <summary>
    /// Check if the notification is allowed to be sent to the client
    /// </summary>
    /// <param name="notification">Notification to check</param>
    /// <returns>True if the notification should be sent to the client else false</returns>
    public bool CheckNotification(IClientNotification notification)
    {
        if (notification.NotificationObjectToSend == null)
        {
            _logger.LogDebug("No event message found", [notification]);
            return false;
        }

        // Filter the notifications to send to the clients
        var typeName = notification.GetType().Name;
        if (AllowedNotifications.Contains(typeName))
        {
            return true;
        }

        _logger.LogDebug($"Notification suppressed for {notification.GetType()}");
        return false;
    }

    /// <summary>Do notify the client</summary>
    /// <param name="notification">Current notification to send to the client instance</param>
    public void DoNotifyClient(IClientNotification notification)
    {
        if (notification.NotificationObjectToSend is not BtcpRequestOutboundDataMessage msg)
        {
            _logger.LogDebug("No event message to send found", [notification]);
            return;
        }

        if (Device.DeviceBusinessLogicAdapter is not IBtcpSimpleDeviceBusinessLogicAdapter adapter)
        {
            _logger.LogDebug(
                $"Device.DeviceBusinessLogicAdapter does not implement {nameof(IBtcpSimpleDeviceBusinessLogicAdapter)}", [notification]);
            return;
        }

        adapter.SendBtRequestNotWaitingForAnswer(msg);
    }

    /// <summary>Load the current client manager instance</summary>
    /// <param name="clientManager">Current client manager instance</param>
    public void LoadClientManager(IClientManager clientManager)
    {
        ClientManager = clientManager;
    }

    /// <summary>
    /// Current IP device
    /// </summary>
    public IIpDevice Device { get; set; }

    /// <summary>Current client manager instance</summary>
    public IClientManager? ClientManager { get; private set; }

    /// <summary>
    /// information about connected client. NOT used for this implementation
    /// </summary>
    public IClientLoginData ClientData { get; } = new FakeLoginData();

    /// <summary>Connection state</summary>
    public bool IsConnected => Device.IsConnected;

    /// <summary>All allowed notifications</summary>
    public IList<string> AllowedNotifications { get; } = new List<string>();
}