// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for creating outbound data messages, Implementations should be used per device. Implementations can handle things like block code generation etc.
/// </summary>
public interface IOutboundDataMessageFactory
{
    /// <summary>
    /// Create am outbound data message from an order management parameter set
    /// </summary>
    /// <param name="parameterSet">Order management parameterset</param>
    /// <returns>Outbound data message</returns>
    IOutboundDataMessage CreateInstance(IParameterSet parameterSet);

    /// <summary>
    /// Reset the factory. This might be used to reset block code generation in case of a ComDevClose event.
    /// </summary>
    void Reset();
}