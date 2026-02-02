// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace Bodoconsult.NetworkCommunication.EnumAndStates;

/// <summary>
/// Default order source for orders normally sent to the device
/// </summary>
public class OrderSource
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public OrderSource(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// The ID of the order source
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// The cleartext name of the order source
    /// </summary>
    public string Name { get; set; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"{Id} {Name}";


    /// <summary>
    /// Order created by the application and sent to device
    /// </summary>
    public static OrderSource AppOrderSource = new OrderSource(0, "Application order");

    /// <summary>
    /// Ann app internal order not send to the device and sent to device
    /// </summary>
    public static OrderSource AppInternalOrderSource = new OrderSource(1, "Application internal order");

    /// <summary>
    /// An order ccreated by a webservice interface and sent to device
    /// </summary>
    public static OrderSource WebserviceOrderSource = new OrderSource(2, "Webservice order");

    /// <summary>
    /// An order created by a remote order call via plain text, JSON or XML based file and sent to device
    /// </summary>
    public static OrderSource RemoteOrderSource = new OrderSource(0, "Order created by remote order");

}