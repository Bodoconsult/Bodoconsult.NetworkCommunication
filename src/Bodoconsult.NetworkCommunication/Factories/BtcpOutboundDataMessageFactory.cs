// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// BTCP implementation of an <see cref="IOutboundDataMessageFactory"/>
/// </summary>
public class BtcpOutboundDataMessageFactory : IOutboundDataMessageFactory
{
    /// <summary>
    /// Create am outbound data message from an order management parameter set
    /// </summary>
    /// <param name="parameterSet">Order management parameterset</param>
    /// <returns>Outbound data message</returns>
    public IOutboundDataMessage CreateInstance(IParameterSet parameterSet)
    {
        if (parameterSet is not BtcpParameterSet ps)
        {
            throw new ArgumentException($"ParameterSet is not of type {nameof(BtcpParameterSet)}");
        }

        var msg = new BtcpOutboundDataMessage(ps.BusinessTransactionId)
        {
            DataBlock = ps
        };

        return msg;
    }

    /// <summary>
    /// Reset the factory. This might be used to reset block code generation in case of a ComDevClose event. In the current implementation this method does nothing
    /// </summary>
    public void Reset()
    {
        // Do nothing
    }
}
