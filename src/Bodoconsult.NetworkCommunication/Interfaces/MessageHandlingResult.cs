// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Message handling result
/// </summary>
public class MessageHandlingResult
{
    ///// <summary>
    ///// Default ctor
    ///// </summary>
    //public MessageHandlingResult()
    //{
    //    ErrorDescription = "";
    //    //DataBlock = "";
    //    //Result = 0;
    //}

    /// <summary>
    /// The result of the message execution
    /// </summary>
    public IOrderExecutionResultState ExecutionResult { get; set; } = OrderExecutionResultState.Successful;

    /// <summary>
    /// An object to transferred from one request spec to the next
    /// </summary>
    public object? TransportObject { get; set; }

    /// <summary>
    /// In case of an error the clear text description of the error
    /// </summary>
    public string ErrorDescription { get; set; } = string.Empty;

    /// <summary>
    /// Error code
    /// </summary>
    public byte Error { get; set; }
}