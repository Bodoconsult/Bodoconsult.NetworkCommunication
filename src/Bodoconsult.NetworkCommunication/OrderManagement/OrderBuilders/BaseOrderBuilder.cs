// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement;
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
    protected BaseOrderBuilder(Type parameterSetType, string orderTypeName)
    {
        ParameterSetType = parameterSetType;
        OrderTypeName = orderTypeName;
    }

    /// <summary>
    /// The type of parameter set the order requires
    /// </summary>
    public Type ParameterSetType { get; }

    /// <summary>
    /// Order type name
    /// </summary>
    public string OrderTypeName { get; }

    /// <summary>
    /// ParameterSet to use for the order
    /// </summary>
    public IParameterSet ParameterSet { get; private set; }

    /// <summary>
    /// Create the (raw) order
    /// </summary>
    /// <param name="id">ID of the order</param>
    /// <param name="parameterSet">ParameterSet to use for the order</param>
    public IOrder CreateOrder(long id, IParameterSet parameterSet)
    {
        var type = parameterSet.GetType();
        if (type != ParameterSetType)
        {
            throw new ArgumentException($"ParameterSet should be {ParameterSetType.Name} but was {type.Name}");
        }

        ParameterSet = parameterSet;
        var order = new OmOrder(id, OrderTypeName, ParameterSet);
        ConfigureOrder(order);
        return order;
    }

    /// <summary>
    /// Configure the order
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
        var rs = new DeviceRequestSpec(name, ParameterSet);
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
        var rs = new InternalRequestSpec(name, ParameterSet);
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
    /// <param name="hasDatablock">Has datablock?</param>
    /// <param name="dataBlockType">Type of the datablock or null</param>
    /// <returns></returns>
    public IRequestAnswer CreateRequestAnswer(IRequestAnswerStep requestAnswerStep, string name, bool hasDatablock = false, Type dataBlockType = null)
    {
        var ra = new RequestAnswer(hasDatablock, dataBlockType, name);
        requestAnswerStep.AllowedRequestAnswers.Add(ra);
        return ra;
    }
}