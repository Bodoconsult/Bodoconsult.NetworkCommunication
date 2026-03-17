// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Configurations;

/// <summary>
/// One request spec with no or only one answer steps and a maximum one answer
/// </summary>
public class OneRequestSpecNoOrOneStepOneAnswerConfiguration : BaseOrderConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="configurationName">Name of the configuration (for state management etc.)</param>
    /// <param name="orderTypeName">Name of the order type</param>
    /// <param name="orderBuilder">Order builder instance to use for order creation</param>
    public OneRequestSpecNoOrOneStepOneAnswerConfiguration(string configurationName, string orderTypeName, IOrderBuilder orderBuilder) : base(configurationName, orderTypeName, orderBuilder)
    { }

    /// <summary>
    /// Delegate for handling request answer messages
    /// </summary>
    public HandleRequestAnswerDelegate? HandleRequestAnswerOnSuccessDelegate { get; set; }
}