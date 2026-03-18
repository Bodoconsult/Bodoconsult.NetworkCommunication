// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpCommunicationSample.Backend.Bll.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic;

public class TncpBackendDeviceStateManager: BaseDeviceStateManager, IBackendDeviceStateManager
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public TncpBackendDeviceStateManager(IStateManagementDevice device) : base(device)
    {
    }

    /// <summary>
    /// Delegate to handle a ComDevClose event in business logic
    /// </summary>
    public void HandleComDevCloseDelegate(IStateMachineState state)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Handle an error message received from the device
    /// </summary>
    public void HandleErrorMessageDelegate(IStateMachineState state, IInboundDataMessage message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Handle an async received message
    /// </summary>
    public MessageHandlingResult HandleAsyncMessageDelegate(IStateMachineState state, IInboundDataMessage? message)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stopping snapshot was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopSnapshotSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stopping snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopSnapshotUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starting snapshot was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartSnapshotSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starting snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartSnapshotUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starting streaming was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartStreamingSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starting streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartStreamingUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stopping streaming was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopStreamingSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stopping streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopStreamingUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Init device was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void DeviceInitSuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Init device was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void DeviceInitUnsuccessfully(IStateMachineState state, IOrder order)
    {
        throw new NotImplementedException();
    }
}