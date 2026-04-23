// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using System.Runtime.CompilerServices;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for implementing a simple device with IP interface
/// </summary>
public interface IIpDevice
{
    /// <summary>
    /// Device configuration for data messaging
    /// </summary>
    IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Communication adapter to use for order management
    /// </summary>
    ICommunicationAdapter? CommunicationAdapter { get; }

    /// <summary>
    /// Client notification manager
    /// </summary>
    ICentralClientNotificationManager ClientNotificationManager { get; }

    /// <summary>
    /// Current <see cref="IDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    IDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter { get; }

    /// <summary>
    /// Is the device connected?
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Is the device pingable
    /// </summary>
    /// <returns>True if the device is pingable else false</returns>
    bool IsPingable { get; }

    /// <summary>
    /// Load a communication adapter
    /// </summary>
    /// <param name="commAdapter">Current communication adapter</param>
    void LoadCommAdapter(ICommunicationAdapter commAdapter);

    /// <summary>
    /// Load the current <see cref="IDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    /// <param name="businessLogicAdapter">Current <see cref="IDeviceBusinessLogicAdapter"/> instance</param>
    void LoadDeviceBusinessLogicAdapter(IDeviceBusinessLogicAdapter businessLogicAdapter);

    /// <summary>
    /// Start the communication
    /// </summary>
    void StartComm();

    /// <summary>
    /// Reset the communication
    /// </summary>
    void ResetComm();

    /// <summary>
    /// Stop the communication
    /// </summary>
    void StopComm();

    /// <summary>
    /// Clear the internal state without breaking comm
    /// </summary>
    void ResetInternalState();


    #region Logging

    /// <summary>
    /// Log in DEBUG mode
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="memberName">Calling method name (filled automatically by compiler)</param>
    /// <param name="filepath">Calling file name (filled automatically by compiler)</param>
    /// <param name="lineNumber">Calling method line number (filled automatically by compiler)</param>
    public void LogDebug(string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int lineNumber = 0);

    /// <summary>
    /// Log in INFORMATION mode
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="memberName">Calling method name (filled automatically by compiler)</param>
    /// <param name="filepath">Calling file name (filled automatically by compiler)</param>
    /// <param name="lineNumber">Calling method line number (filled automatically by compiler)</param>
    public void LogInformation(string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int lineNumber = 0);

    /// <summary>
    /// Log in WARNING mode
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="memberName">Calling method name (filled automatically by compiler)</param>
    /// <param name="filepath">Calling file name (filled automatically by compiler)</param>
    /// <param name="lineNumber">Calling method line number (filled automatically by compiler)</param>
    public void LogWarning(string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int lineNumber = 0);

    /// <summary>
    /// Log in ERROR mode
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="memberName">Calling method name (filled automatically by compiler)</param>
    /// <param name="filepath">Calling file name (filled automatically by compiler)</param>
    /// <param name="lineNumber">Calling method line number (filled automatically by compiler)</param>
    public void LogError(string message,
        [CallerMemberName] string? memberName = null,
        [CallerFilePath] string? filepath = null,
        [CallerLineNumber] int lineNumber = 0);

    #endregion
}