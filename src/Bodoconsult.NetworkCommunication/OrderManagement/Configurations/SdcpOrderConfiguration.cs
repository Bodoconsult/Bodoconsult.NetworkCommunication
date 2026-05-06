// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// SDCP order config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class SdcpOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    private static readonly IOrderBuilder Builder = new SdcpOrderBuilder();

    /// <summary>
    /// Default ctor
    /// </summary>
    public SdcpOrderConfiguration() : base($"{BuiltinOrders.SdcpOrder}Configuration", BuiltinOrders.SdcpOrder, Builder)
    { }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
        return new SdcpOrderConfiguration
        {
            CreateParameterSetDelegate = CreateParameterSetDelegate,
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = null
        };
    }
}