// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;

namespace Bodoconsult.NetworkCommunication.Helpers;

/// <summary>
/// Helper class for basic delegates
/// </summary>
public static class DelegateHelper
{
    /// <summary>
    /// Default implementation of <see cref="HandleComDevCloseDelegate"/>: calls DefaultStateNames.DeviceOfflineState state
    /// </summary>
    /// <param name="state">Current state</param>
    public static void HandleComDevCloseDelegate(IStateMachineState state)
    {
        state.NextState = state.CurrentContext.CreateStateInstance(DefaultStateNames.DeviceOfflineState);
    }

    /// <summary>
    /// Default implementation of <see cref="HandleErrorMessageDelegate"/>: calls DefaultStateNames.DeviceOfflineState state
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="message">Received message</param>
    public static void HandleErrorMessageDelegate(IStateMachineState state, IInboundDataMessage message)
    {
        state.NextState = state.CurrentContext.CreateStateInstance(DefaultStateNames.DeviceOfflineState);
    }

    /// <summary>
    /// Default implementation of <see cref="HandleAsyncMessageDelegate"/>: doing nothing, returns success
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="message">Received message</param>
    public static MessageHandlingResult HandleAsyncMessageDelegate(IStateMachineState state, IInboundDataMessage? message)
    {
        return MessageHandlingResultHelper.Success();
    }

    /// <summary>
    /// Default implementation of <see cref="PrepareRegularStateRequestDelegate"/>: returns empty list of orders
    /// </summary>
    public static List<IOrder> PrepareRegularStateRequestDelegate()
    {
        return [];
    }

    /// <summary>
    /// Default implementation of <see cref="OrderFinishedSucessfullyDelegate"/>: doing nothing
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public static void OrderFinishedSucessfullyDelegate(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Default implementation of <see cref="OrderFinishedUnsucessfullyDelegate"/>: doing nothing
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public static void OrderFinishedUnsucessfullyDelegate(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Default implementation of <see cref="HandleRegularStateRequestAnswerDelegate"/>: doing nothing, returns success
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="message">Received message</param>
    /// <param name="doNotNotifyClient">Do not notify client</param>
    public static MessageHandlingResult HandleRegularStateRequestAnswerDelegate(IStateMachineState state, IInboundDataMessage message, bool doNotNotifyClient)
    {
        return MessageHandlingResultHelper.Success();
    }

    /// <summary>
    /// Default implementation of <see cref="HandleRegularStateRequestAnswerDelegate"/>: doing nothing
    /// </summary>
    /// <param name="state">Current state</param>
    public static void ExecuteActionForStateDelegate(IOrderlessActionStateMachineState state)
    {
        // doing nothing
    }

    /// <summary>
    /// Default implementation of <see cref="CancelStateDelegate"/>: doing nothing
    /// </summary>
    /// <param name="state">Current state</param>
    public static void CancelStateDelegate(IStateMachineState state)
    {
        state.CancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Default implementation of <see cref="CancelStateDelegate"/>: check JobStates
    /// </summary>
    /// <param name="state">Current state</param>
    public static void DefaultCheckJobstatesActionForStateDelegate(INoActionStateMachineState state)
    {
        while (!state.CancellationTokenSource?.IsCancellationRequested ?? false)
        {
            var context = state.CurrentContext;

            // Check if there is a job state to restore after break
            if (context.SavedJobState != null)
            {
                context.RestoreSavedJobState();
                return;
            }

            // Check if a job state is waiting. If yes, process it now

            // Get the first job state and process it
            var newState = context.GetNextJobState();

            // No state
            if (newState == null)
            {
                // Wait a bit then check again
                Thread.Sleep(DeviceCommunicationBasics.JobStateCheckTimeout);
                continue;
            }

            // State found
            context.RequestState(newState);
            state.CancellationTokenSource?.Dispose();
        }
    }

    /// <summary>
    /// Default implementation of <see cref="CancelStateDelegate"/>: doing nothing
    /// </summary>
    /// <param name="state">Current state</param>
    public static void CheckJobstatesActionForStateDelegate(INoActionStateMachineState state)
    {
        // Do nothing
    }

    /// <summary>
    /// Default implementation of <see cref="CancelStateDelegate"/>: ping the device
    /// </summary>
    /// <param name="state">Current state</param>
    public static void DefaultExecuteActionForStateDelegate(IOrderlessActionStateMachineState state)
    {
        ArgumentNullException.ThrowIfNull(state.CancellationTokenSource);
        //if (state.CancellationTokenSource == null)
        //{
        //    throw new ArgumentNullException(nameof(state.CancellationTokenSource));
        //}

        var context = state.CurrentContext;

        // Wait until the device is pingable
        while (!state.CancellationTokenSource.IsCancellationRequested)
        {
            context.SetBusinessSubState(DefaultBusinessSubStates.PingingTower);
            if (context.IsPingable)
            {
                break;
            }

            context.SetBusinessSubState(DefaultBusinessSubStates.WaitingForNextPingingTower);
            Thread.Sleep(DeviceCommunicationBasics.PingRepeatInterval);
        }

        // New state now
        var stateNew = context.CreateStateInstance(DefaultStateNames.DeviceOnlineState);
        state.NextState = stateNew;
        state.RequestNextState();
    }

    /// <summary>
    /// Default implementation of <see cref="HandleRequestAnswerOnSuccessDelegate"/>: doing nothing
    /// </summary>
    public static MessageHandlingResult HandleRequestAnswerOnSuccessDelegate(IInboundDataMessage? message, object? transportObject, IParameterSet? parameterSet)
    {
        return MessageHandlingResultHelper.Success();
    }
}