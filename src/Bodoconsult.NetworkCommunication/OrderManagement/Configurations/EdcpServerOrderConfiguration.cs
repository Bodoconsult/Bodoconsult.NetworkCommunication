// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// EDCP server order config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class EdcpServerOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    private static readonly IOrderBuilder Builder = new EdcpServerOrderBuilder();

    /// <summary>
    /// Default ctor
    /// </summary>
    public EdcpServerOrderConfiguration() : base($"{BuiltinOrders.EdcpServerOrder}Configuration", BuiltinOrders.EdcpServerOrder, Builder)
    { }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
        return new EdcpServerOrderConfiguration
        {
            CreateParameterSetDelegate = CreateParameterSetDelegate,
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = null
        };
    }
}