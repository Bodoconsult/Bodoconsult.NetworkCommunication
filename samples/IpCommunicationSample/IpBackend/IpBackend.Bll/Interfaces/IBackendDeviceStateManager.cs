// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpCommunicationSample.Backend.Bll.Interfaces
{
    /// <summary>
    /// Interface for state handling in the backend
    /// </summary>
    public interface IBackendDeviceStateManager : IDeviceStateManager
    {
        /// <summary>
        /// Delegate to handle a ComDevClose event in business logic
        /// </summary>
        void HandleComDevCloseDelegate(IStateMachineState state);

        /// <summary>
        /// Handle an error message received from the device
        /// </summary>
        void HandleErrorMessageDelegate(IStateMachineState state, IInboundDataMessage message);

        /// <summary>
        /// Handle an async received message
        /// </summary>
        MessageHandlingResult HandleAsyncMessageDelegate(IStateMachineState state, IInboundDataMessage? message);

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
    }
}
