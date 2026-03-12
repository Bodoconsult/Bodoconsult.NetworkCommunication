// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Builder for the default state DeviceOfflineState
/// </summary>
public class DeviceOfflineStateBuilder : BaseOrderlessStateMachineStateBuilder
{
    /// <summary>
    /// Allowed next states internal
    /// </summary>
    public static readonly List<string> AllowedNextStatesInternal = [DefaultStateNames.DeviceOnlineState];

    /// <summary>
    /// Default ctor
    /// </summary>
    public DeviceOfflineStateBuilder() : base(DefaultStateIds.DeviceOfflineState, DefaultStateNames.DeviceOfflineState)
    { }

    /// <summary>
    /// Configure an orderless action state
    /// </summary>
    /// <param name="state">Current <see cref="IOrderlessActionStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public override void ConfigureOrderlessActionState(IOrderlessActionStateMachineState state, IOrderlessActionStateConfiguration config)
    {
        state.InitialDeviceState = DefaultDeviceStates.DeviceStateOffline;
        state.InitialBusinessSubState = DefaultBusinessSubStates.NotSet;

        state.HandleAsyncMessageDelegate = config.HandleAsyncMessageDelegate;
        state.HandleComDevCloseDelegate = config.HandleComDevCloseDelegate;
        state.HandleErrorMessageDelegate = config.HandleErrorMessageDelegate;
        state.HandleRegularStateRequestAnswerDelegate = config.HandleRegularStateRequestAnswerDelegate;
        state.PrepareRegularStateRequestDelegate = config.PrepareRegularStateRequestDelegate;

        state.ExecuteActionForStateDelegate = config.ExecuteActionForStateDelegate ?? DelegateHelper.DefaultExecuteActionForStateDelegate;

        state.IsRunningOrdersCancellationRequired = true;
        state.IsTurningOffStateRequestsRequired = true;
        state.CancellationTokenSource = new CancellationTokenSource();
        state.CancelStateDelegate = DelegateHelper.CancelStateDelegate;

        state.AllowedNextStates.AddRange(AllowedNextStatesInternal);
    }


   
}