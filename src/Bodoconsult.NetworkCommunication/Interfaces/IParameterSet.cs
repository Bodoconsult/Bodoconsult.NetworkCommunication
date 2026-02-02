// Copyright (c) Mycronic. All rights reserved.

using System.ComponentModel.DataAnnotations;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Base interface for TOM parameter sets
/// </summary>
public interface IParameterSet: IDisposable
{
    /// <summary>
    /// User ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The order the parameter set is bound to
    /// </summary>
    IOrder CurrentOrder { get; }

    /// <summary>
    /// Load the order the parameter set is bound to. May only run once!
    /// </summary>
    /// <param name="order">Order to inject</param>
    void LoadOrder(IOrder order);

    /// <summary>
    /// The name of the user requesting the order run
    /// </summary>
    string UserRequestingTheOrderRun { get; set; }

    /// <summary>
    /// Send a CANCEL command to the tower if request step was unsuccessful. Default: false
    /// </summary>
    bool SendCancelToTowerIfUnsuccessful { get; set; }

    /// <summary>
    /// Do not notify the client after order running
    /// </summary>
    bool DoNotNotifyClient { get; set; }

    /// <summary>
    /// A result object filled by the the order execution process
    /// </summary>
    object OrderResult { get; set; }


    /// <summary>
    /// Is the parameter set valid?
    /// </summary>
    ICollection<ValidationResult> IsValid { get; }

}