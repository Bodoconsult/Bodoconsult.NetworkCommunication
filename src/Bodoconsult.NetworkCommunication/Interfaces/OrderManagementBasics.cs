// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Basic settings for order management
/// </summary>
public static class OrderManagementBasics
{
    static OrderManagementBasics()
    {

        // Default order states
        DefaultOrderStates.Add(OrderState.Unknown);
        DefaultOrderStates.Add(OrderState.Added);
        DefaultOrderStates.Add(OrderState.Started);
        DefaultOrderStates.Add(OrderState.FinishedSuccessfully);
        DefaultOrderStates.Add(OrderState.FinishedWithFailure);
        DefaultOrderStates.Add(OrderState.Canceled);

        // Default order execution result states
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.Successful);
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.Unsuccessful);
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.Timeout);
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.Error);
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.NotProcessed);
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.NoResponseFromDevice);
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.HardwareError);
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.Can);
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.Nack);
        DefaultOrderExecutionResultStates.Add(OrderExecutionResultState.UpdateMode);
    }

    /// <summary>
    /// All default order execution result states
    /// </summary>
    public static List<IOrderState> DefaultOrderStates { get; set; } = [];

    /// <summary>
    /// All default order execution result states
    /// </summary>
    public static List<IOrderExecutionResultState> DefaultOrderExecutionResultStates { get; set; } = [];
}