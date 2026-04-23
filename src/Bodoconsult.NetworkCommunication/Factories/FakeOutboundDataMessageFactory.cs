// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Fake implementation of an <see cref="IOutboundDataMessageFactory"/>
/// </summary>
public class FakeOutboundDataMessageFactory : IOutboundDataMessageFactory
{
    /// <summary>
    /// Create am outbound data message from an order management parameter set
    /// </summary>
    /// <param name="parameterSet">Order management parameterset</param>
    /// <returns>Outbound data message</returns>
    public IOutboundDataMessage CreateInstance(IParameterSet parameterSet)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Reset the factory. This might be used to reset block code generation in case of a ComDevClose event.
    /// </summary>
    public void Reset()
    {
       // Do nothing
    }
}