// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// EDCP server order waiting ONLY for answer config with one request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class OnlyAnswerEdcpServerOrderConfiguration : OneRequestSpecNoOrOneStepOneAnswerConfiguration
{
    private static readonly IOrderBuilder Builder = new OnlyAnswerEdcpServerOrderBuilder();

    /// <summary>
    /// Default ctor
    /// </summary>
    public OnlyAnswerEdcpServerOrderConfiguration() : base($"{BuiltinOrders.OnlyAnswerEdcpServerOrder}Configuration", BuiltinOrders.OnlyAnswerEdcpServerOrder, Builder)
    { }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone()
    {
        return new OnlyAnswerEdcpServerOrderConfiguration
        {
            CreateParameterSetDelegate = CreateParameterSetDelegate,
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = null
        };
    }
}