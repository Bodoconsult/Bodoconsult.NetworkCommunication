// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// State management context
/// </summary>
public interface IStateManagementContext
{
    /// <summary>
    /// Current instance of the Device order processor
    /// </summary>
    IOrderProcessor OrderProcessor { get; }

    /// <summary>
    /// Current device state
    /// </summary>
    IDeviceState DeviceState { get; }

    /// <summary>
    /// Current business state
    /// </summary>
    IBusinessState BusinessState { get; }

    /// <summary>
    /// Current business substate
    /// </summary>
    IBusinessSubState BusinessSubState { get; }

    /// <summary>
    /// The current <see cref="IStateManagementState"/>
    /// </summary>
    IStateManagementState CurrentState { get; }

    /// <summary>
    /// Set the states for the context. DO NOT USE this method from your code. Should be called only from a <see cref="IStateManagementState"/>.
    /// </summary>
    /// <param name="deviceState"></param>
    /// <param name="businessState"></param>
    /// <param name="businessSubState"></param>
    void SetInitialStates(IDeviceState deviceState, IBusinessState businessState, IBusinessState businessSubState);

    /// <summary>
    /// Load a <see cref="IStateManagementState"/> instance. DO NOT USE this method from your code. Should be called only from a <see cref="IStateManagementState"/>.
    /// </summary>
    /// <param name="state"></param>
    void LoadState(IStateManagementState state);

    /// <summary>
    /// Send an app notfication
    /// </summary>
    /// <param name="state">State management state</param>
    void DoNotify(IStateManagementState state);

    #region Logging

    /// <summary>
    /// Log in DEBUG mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public void LogDebug(string message);

    /// <summary>
    /// Log in INFORMATION mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public void LogInformation(string message);

    /// <summary>
    /// Log in WARNING mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public void LogWarning(string message);

    /// <summary>
    /// Log in ERROR mode
    /// </summary>
    /// <param name="message">Message to log</param>
    public void LogError(string message);

    #endregion
}