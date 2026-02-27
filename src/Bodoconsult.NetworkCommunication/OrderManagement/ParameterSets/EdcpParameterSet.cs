// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

/// <summary>
/// Parameter set for EDCP requests 
/// </summary>
public class EdcpParameterSet : BasicOutboundDatablock, IParameterSet
{
    /// <summary>
    /// The order the parameter set is bound to
    /// </summary>
    [JsonIgnore]
    public IOrder CurrentOrder { get; private set; }

    /// <summary>
    /// Load the order the parameter set is bound to. May only run once!
    /// </summary>
    /// <param name="order">Order to inject</param>
    public void LoadOrder(IOrder order)
    {
        if (CurrentOrder != null)
        {
            throw new ArgumentException("The order for a parameter set may be set only once!");
        }
        CurrentOrder = order;
    }

    /// <summary>
    /// Payload to send with the emssage
    /// </summary>
    public Memory<byte> Payload { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The name of the user requesting the order run
    /// </summary>
    public string UserRequestingTheOrderRun { get; set; }

    /// <summary>
    /// Send a CANCEL command to the device if request step was unsuccessful. Default: false
    /// </summary>
    public bool SendCancelTodeviceIfUnsuccessful { get; set; }

    /// <summary>
    /// Do not notify the client after order running
    /// </summary>
    public bool DoNotNotifyClient { get; set; }

    /// <summary>
    /// A result object filled by the order execution process
    /// </summary>
    public object OrderResult { get; set; }

    /// <summary>
    /// Is the parameter set valid?
    /// </summary>
    public ICollection<ValidationResult> IsValid => new List<ValidationResult>();

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        CurrentOrder = null;
        OrderResult = null;
    }
}