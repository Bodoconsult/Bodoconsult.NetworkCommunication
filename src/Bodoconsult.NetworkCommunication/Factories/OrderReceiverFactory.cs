// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Current implementation of <see cref="IOrderReceiverFactory"/>
/// </summary>
public class OrderReceiverFactory : IOrderReceiverFactory
{
    /// <summary>
    /// Create a tower order receiver instance
    /// </summary>
    /// <param name="logger">Current monitor logger</param>
    /// <returns>A tower order receiver instance</returns>
    public IOrderReceiver CreateInstance(IAppLoggerProxy logger)
    {
        return new OrderReceiver(logger);
    }
}