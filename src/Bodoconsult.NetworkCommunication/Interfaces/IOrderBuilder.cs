// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for order builders
/// </summary>
public interface IOrderBuilder
{
    /// <summary>
    /// The type of parameter set the order requires
    /// </summary>
    Type ParameterSetType { get; }

    /// <summary>
    /// Order type name
    /// </summary>
    string OrderTypeName { get; }

    /// <summary>
    /// Create the (raw) order
    /// </summary>
    /// <param name="id">ID of the order</param>
    /// <param name="parameterSet">ParameterSet to use for the order</param>
    IOrder CreateOrder(long id, IParameterSet parameterSet);

    /// <summary>
    /// Configure the order. Implementation of this method may require to add dependencies to your business logic layer
    /// </summary>
    void ConfigureOrder(IOrder order);

    /// <summary>
    /// Create an <see cref="DeviceRequestSpec"/> instance and add it to the order
    /// </summary>
    /// <param name="order">Current order</param>
    /// <param name="name">Name of the request spec</param>
    /// <returns><see cref="DeviceRequestSpec"/> instance </returns>
    IDeviceRequestSpec CreateDeviceRequestSpec(IOrder order, string name);

    /// <summary>
    /// Create an <see cref="InternalRequestSpec"/> instance and add it to the order
    /// </summary>
    /// <param name="order">Current order</param>
    /// <param name="name">Name of the request spec</param>
    /// <returns><see cref="InternalRequestSpec"/> instance </returns>
    IInternalRequestSpec CreateInternalRequestSpec(IOrder order, string name);

    /// <summary>
    /// Create an <see cref="DeviceRequestAnswerStep"/> instance and add it to a request spec in the order
    /// </summary>
    /// <param name="requestSpec">Current request spec</param>
    /// <param name="name">Name of the request spec</param>
    /// <returns><see cref="DeviceRequestAnswerStep"/> instance </returns>
    IDeviceRequestAnswerStep CreateDeviceRequestAnswerStep(IDeviceRequestSpec requestSpec, string name);

    /// <summary>
    /// Create an <see cref="InternalRequestAnswerStep"/> instance and add it to a request spec in the order
    /// </summary>
    /// <param name="requestSpec">Current request spec</param>
    /// <param name="name">Name of the request spec</param>
    /// <returns><see cref="InternalRequestAnswerStep"/> instance </returns>
    IInternalRequestAnswerStep CreateInternalRequestAnswerStep(IInternalRequestSpec requestSpec, string name);

    /// <summary>
    /// Create an <see cref="IRequestAnswer"/> instance and it to a request answer step in the order
    /// </summary>
    /// <param name="requestAnswerStep">Current request answer step</param>
    /// <param name="name">Name of the answer</param>
    /// <param name="hasDatablock">Has datablock?</param>
    /// <param name="dataBlockType">Type of the datablock or null</param>
    /// <returns></returns>
    IRequestAnswer CreateRequestAnswer(IRequestAnswerStep requestAnswerStep, string name, bool hasDatablock = false, Type dataBlockType = null);
}