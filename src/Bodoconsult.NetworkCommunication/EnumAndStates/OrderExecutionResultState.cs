// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.EnumAndStates;

/// <summary>
/// Default order execution result states
/// </summary>
public class OrderExecutionResultState: IOrderExecutionResultState
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public OrderExecutionResultState(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// The ID of the state
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The cleartext name of the state
    /// </summary>
    public string Name { get; set; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{Id} {Name}";

    /// <summary>
    /// Order was processed successfully
    /// </summary>
    public static OrderExecutionResultState Successful { get; } = new OrderExecutionResultState(0, string.Intern("Successful"));

    /// <summary>
    /// Order was processed unsuccessfully
    /// </summary>
    public static OrderExecutionResultState Unsuccessful { get; } = new OrderExecutionResultState(1, string.Intern("Unsuccessful"));

    /// <summary>
    /// Order has timed out
    /// </summary>
    public static OrderExecutionResultState Timeout { get; } = new OrderExecutionResultState(2, string.Intern("Timeout"));

    /// <summary>
    /// Order was processed with an error
    /// </summary>
    public static OrderExecutionResultState Error { get; } = new OrderExecutionResultState(3, string.Intern("Error"));

    /// <summary>
    /// Order was NOT processed at all
    /// </summary>
    public static OrderExecutionResultState NotProcessed { get; } = new OrderExecutionResultState(4, string.Intern("NotProcessed"));

    /// <summary>
    /// Order execution not possible due to device was NOT answering
    /// </summary>
    public static OrderExecutionResultState NoResponseFromDevice { get; } = new OrderExecutionResultState(5, string.Intern("NoResponseFromDevice"));

    /// <summary>
    /// Device returned a CAN
    /// </summary>
    public static OrderExecutionResultState Can { get; } = new OrderExecutionResultState(6, string.Intern("Can"));

    /// <summary>
    /// Device returned a NACK
    /// </summary>
    public static OrderExecutionResultState Nack { get; } = new OrderExecutionResultState(7, string.Intern("Nack"));

    /// <summary>
    /// device hardware error
    /// </summary>
    public static OrderExecutionResultState HardwareError { get; } = new OrderExecutionResultState(8, string.Intern("HardwareError"));

    /// <summary>
    /// Device is in firmware update mode
    /// </summary>
    public static OrderExecutionResultState UpdateMode { get; } = new OrderExecutionResultState(9, string.Intern("UpdateMode"));

}