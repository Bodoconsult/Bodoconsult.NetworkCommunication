// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.EnumAndStates;

/// <summary>
/// Default order execution result states
/// </summary>
public class OrderExecutionResultState: IOrderExecutionResultState
{
    private readonly string _msg;

    /// <summary>
    /// Default ctor
    /// </summary>
    public OrderExecutionResultState(int id, string name)
    {
        Id = id;
        Name = name;
        _msg = string.Intern($"{Id} {Name}");
    }

    /// <summary>
    /// The ID of the state
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The cleartext name of the state
    /// </summary>
    public string Name { get; set; }

    /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
    public bool Equals(IOrderExecutionResultState? other)
    {
        if (other == null)
        {
            return false;
        }

        return Id == other.Id;
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => _msg;

    /// <summary>
    /// Order was processed successfully
    /// </summary>
    public static OrderExecutionResultState Successful { get; } = new(0, "Successful");

    /// <summary>
    /// Order was processed unsuccessfully
    /// </summary>
    public static OrderExecutionResultState Unsuccessful { get; } = new(1, "Unsuccessful");

    /// <summary>
    /// Order has timed out
    /// </summary>
    public static OrderExecutionResultState Timeout { get; } = new(2, "Timeout");

    /// <summary>
    /// Order was processed with an error
    /// </summary>
    public static OrderExecutionResultState Error { get; } = new(3, "Error");

    /// <summary>
    /// Order was NOT processed at all
    /// </summary>
    public static OrderExecutionResultState NotProcessed { get; } = new(4, "NotProcessed");

    /// <summary>
    /// Order execution not possible due to device was NOT answering
    /// </summary>
    public static OrderExecutionResultState NoResponseFromDevice { get; } = new(5, "NoResponseFromDevice");

    /// <summary>
    /// Device returned a CAN
    /// </summary>
    public static OrderExecutionResultState Can { get; } = new(6, "Can");

    /// <summary>
    /// Device returned a NACK
    /// </summary>
    public static OrderExecutionResultState Nack { get; } = new(7, "Nack");

    /// <summary>
    /// device hardware error
    /// </summary>
    public static OrderExecutionResultState HardwareError { get; } = new(8, "HardwareError");

    /// <summary>
    /// Device is in firmware update mode
    /// </summary>
    public static OrderExecutionResultState UpdateMode { get; } = new(9, "UpdateMode");

}