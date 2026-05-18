// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpBackend.Bll.Interfaces;

/// <summary>
/// Interface for state handling in the TCP/IP channel from backend to IP device
/// </summary>
public interface IIpDeviceTcpIpDeviceBusinessLogicAdapter: IStateMachineDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Current UDP port to use
    /// </summary>
    public int UdpPort { get; set; }

    #region Device order handling

    /// <summary>
    /// Starting streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StartMessagingUnsuccessfully(IStateMachineState state, IOrder order);

    /// <summary>
    /// Stopping streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    void StopMessagingUnsuccessfully(IStateMachineState state, IOrder order);

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
    /// Request a start messaging state
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply RequestDeviceStartMessagingState(IBusinessTransactionRequestData request);

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
    IBusinessTransactionReply RequestDeviceStopMessagingState(IBusinessTransactionRequestData request);

    /// <summary>
    /// Request a stop snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply RequestDeviceStopSnapshotState(IBusinessTransactionRequestData request);

    #endregion
}