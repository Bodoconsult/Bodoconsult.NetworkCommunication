// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpCommunicationSample.Backend.Bll.Interfaces;

/// <summary>
/// Interface for state handling in the TCP/IP channel from backend to IP device
/// </summary>
public interface IIpDeviceTcpIpDeviceBusinessLogicAdapter: IStateMachineDeviceBusinessLogicAdapter
{
    #region Device order handling

    /// <summary>
    /// Stopping snapshot was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StopSnapshotSuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Stopping snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StopSnapshotUnsuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Starting snapshot was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StartSnapshotSuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Starting snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StartSnapshotUnsuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Starting streaming was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StartStreamingSuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Starting streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StartStreamingUnsuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Stopping streaming was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StopStreamingSuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Stopping streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StopStreamingUnsuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Init device was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void DeviceInitSuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Init device was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void DeviceInitUnsuccessfully(IStateMachineState state, IOrder order);

    #endregion

    #region State management

    /// <summary>
    /// Request a start streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply RequestDeviceStartStreamingState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a start snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply RequestDeviceStartSnapshotState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a stop streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply RequestDeviceStopStreamingState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a stop snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply RequestDeviceStopSnapshotState(IBusinessTransactionRequestData request);

    #endregion
}