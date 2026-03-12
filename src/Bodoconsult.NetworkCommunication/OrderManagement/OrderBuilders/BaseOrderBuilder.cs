// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

/// <summary>
/// Base class for order builders
/// </summary>
public abstract class BaseOrderBuilder : IOrderBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="parameterSetType">The type of parameter set the order requires</param>
    /// <param name="orderTypeName">Order type name</param>
    protected BaseOrderBuilder(Type? parameterSetType, string orderTypeName)
    {
        ParameterSetType = parameterSetType;
        OrderTypeName = orderTypeName;
    }

    /// <summary>
    /// The type of parameter set the order requires
    /// </summary>
    public Type? ParameterSetType { get; }

    /// <summary>
    /// Order type name
    /// </summary>
    public string OrderTypeName { get; }

    /// <summary>
    /// Create the (raw) order
    /// </summary>
    /// <param name="id">ID of the order</param>
    /// <param name="parameterSet">ParameterSet to use for the order</param>
    public IOrder CreateOrder(long id, IParameterSet parameterSet)
    {
        if (ParameterSetType != null)
        {
            var type = parameterSet.GetType();
            if (type != ParameterSetType)
            {
                throw new ArgumentException($"ParameterSet should be {ParameterSetType.Name} but was {type.Name}");
            }
        }

        var order = new OmOrder(id, OrderTypeName, parameterSet);
        ConfigureOrder(order);
        return order;
    }


    /// <summary>
    /// Configure the order. Implementation of this method may require to add dependencies to your business logic layer
    /// </summary>
    public virtual void ConfigureOrder(IOrder order)
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>
    /// Create an <see cref="DeviceRequestSpec"/> instance and add it to the order
    /// </summary>
    /// <param name="order">Current order</param>
    /// <param name="name">Name of the request spec</param>
    /// <returns><see cref="DeviceRequestSpec"/> instance </returns>
    public IDeviceRequestSpec CreateDeviceRequestSpec(IOrder order, string name)
    {
        if (order.ParameterSet == null)
        {
            throw new ArgumentNullException(nameof(order.ParameterSet));
        }

        var rs = new DeviceRequestSpec(name, order.ParameterSet);
        order.RequestSpecs.Add(rs);
        return rs;
    }

    /// <summary>
    /// Create an <see cref="NoAnswerDeviceRequestSpec"/> instance and add it to the order
    /// </summary>
    /// <param name="order">Current order</param>
    /// <param name="name">Name of the request spec</param>
    /// <param name="handleRequestAnswerOnSuccessDelegate">Delegate fired if the order was eceuted successfully</param>
    /// <returns><see cref="DeviceRequestSpec"/> instance </returns>
    public INoAnswerDeviceRequestSpec CreateNoAnswerDeviceRequestSpec(IOrder order, string name, HandleRequestAnswerDelegate? handleRequestAnswerOnSuccessDelegate)
    {
        if (order.ParameterSet == null)
        {
            throw new ArgumentNullException(nameof(order.ParameterSet));
        }

        var rs = new NoAnswerDeviceRequestSpec(name, order.ParameterSet);
        rs.HandleRequestAnswerOnSuccessDelegate = handleRequestAnswerOnSuccessDelegate;
        order.RequestSpecs.Add(rs);
        return rs;
    }


    /// <summary>
    /// Create an <see cref="NoHandshakeNoAnswerDeviceRequestSpec"/> instance and add it to the order
    /// </summary>
    /// <param name="order">Current order</param>
    /// <param name="name">Name of the request spec</param>
    /// <param name="handleRequestAnswerOnSuccessDelegate">Delegate fired if the order was eceuted successfully</param>
    /// <returns><see cref="DeviceRequestSpec"/> instance </returns>
    public INoHandshakeNoAnswerDeviceRequestSpec CreateNoHandshakeNoAnswerDeviceRequestSpec(IOrder order, string name, HandleRequestAnswerDelegate? handleRequestAnswerOnSuccessDelegate)
    {
        if (order.ParameterSet == null)
        {
            throw new ArgumentNullException(nameof(order.ParameterSet));
        }

        var rs = new NoHandshakeNoAnswerDeviceRequestSpec(name, order.ParameterSet);
        rs.HandleRequestAnswerOnSuccessDelegate = handleRequestAnswerOnSuccessDelegate;
        order.RequestSpecs.Add(rs);
        return rs;
    }

    /// <summary>
    /// Create an <see cref="InternalRequestSpec"/> instance and add it to the order
    /// </summary>
    /// <param name="order">Current order</param>
    /// <param name="name">Name of the request spec</param>
    /// <returns><see cref="InternalRequestSpec"/> instance </returns>
    public IInternalRequestSpec CreateInternalRequestSpec(IOrder order, string name)
    {
        if (order.ParameterSet == null)
        {
            throw new ArgumentNullException(nameof(order.ParameterSet));
        }

        var rs = new InternalRequestSpec(name, order.ParameterSet);
        order.RequestSpecs.Add(rs);
        return rs;
    }

    /// <summary>
    /// Create an <see cref="DeviceRequestAnswerStep"/> instance and add it to a request spec in the order
    /// </summary>
    /// <param name="requestSpec">Current request spec</param>
    /// <param name="name">Name of the request spec</param>
    /// <returns><see cref="DeviceRequestAnswerStep"/> instance </returns>
    public IDeviceRequestAnswerStep CreateDeviceRequestAnswerStep(IDeviceRequestSpec requestSpec, string name)
    {
        var ras = new DeviceRequestAnswerStep(requestSpec);
        requestSpec.RequestAnswerSteps.Add(ras);
        return ras;
    }

    /// <summary>
    /// Create an <see cref="InternalRequestAnswerStep"/> instance and add it to a request spec in the order
    /// </summary>
    /// <param name="requestSpec">Current request spec</param>
    /// <param name="name">Name of the request spec</param>
    /// <returns><see cref="InternalRequestAnswerStep"/> instance </returns>
    public IInternalRequestAnswerStep CreateInternalRequestAnswerStep(IInternalRequestSpec requestSpec, string name)
    {
        var ras = new InternalRequestAnswerStep(requestSpec);
        requestSpec.RequestAnswerSteps.Add(ras);
        return ras;
    }


    /// <summary>
    /// Create an <see cref="IRequestAnswer"/> instance and it to a request answer step in the order
    /// </summary>
    /// <param name="requestAnswerStep">Current request answer step</param>
    /// <param name="name">Name of the answer</param>
    /// <param name="checkReceivedMessageDelegate">Current delegate for message checking</param>
    /// <param name="handleRequestAnswerOnSuccessDelegate">Delegate fired if the order was eceuted successfully</param>
    /// <param name="hasDatablock">Has datablock?</param>
    /// <param name="dataBlockType">Type of the datablock or null</param>
    /// <returns>Request answer</returns>
    public IRequestAnswer CreateRequestAnswer(IRequestAnswerStep requestAnswerStep, string name, CheckReceivedMessageDelegate checkReceivedMessageDelegate, HandleRequestAnswerDelegate? handleRequestAnswerOnSuccessDelegate, bool hasDatablock = false, Type? dataBlockType = null)
    {
        var ra = new RequestAnswer(hasDatablock, dataBlockType, name, checkReceivedMessageDelegate);
        ra.HandleRequestAnswerOnSuccessDelegate = handleRequestAnswerOnSuccessDelegate;
        requestAnswerStep.AllowedRequestAnswers.Add(ra);
        return ra;
    }
}