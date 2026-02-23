// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Order state enum implemented as class with static default enum values
/// </summary>
public class OrderState : IOrderState
{

    /// <summary>
    /// Default ctor
    /// </summary>
    public OrderState(int id, string name)
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

    /// <summary>
    /// Unknown means the order was just created
    /// </summary>
    public static OrderState Unknown { get; } = new(0, string.Intern("Unknown"));

    /// <summary>
    /// Added to waiting queue
    /// </summary>
    public static OrderState Added { get; } = new(1, string.Intern("Added"));

    /// <summary>
    /// Order execution was started
    /// </summary>
    public static OrderState Started { get; } = new(2, string.Intern("Started"));

    /// <summary>
    /// Order has finished successfully
    /// </summary>
    public static OrderState FinishedSuccessfully { get; } = new(3, string.Intern("FinishedSuccessfully"));

    /// <summary>
    /// Order has finished with failure
    /// </summary>
    public static OrderState FinishedWithFailure { get; } = new(4, string.Intern("FinishedWithFailure"));

    /// <summary>
    /// The order has been cancelled
    /// </summary>
    public static OrderState Canceled { get; } = new(5, string.Intern("Canceled"));


    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{Id} {Name}";
}