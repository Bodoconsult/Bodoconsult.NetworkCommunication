// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// EDCP server side implementation of an <see cref="IOutboundDataMessageFactory"/>. Creates a block code from 0 to 127
/// </summary>
public class EdcpServerOutboundDataMessageFactory : IOutboundDataMessageFactory
{
    private byte _blockCode;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="minimumBlockCode">The minimum blockcode sent by the client. Default: 0</param>
    /// <param name="maximumBlockCode">The maximum blockcode sent by the client. Default: 127</param>
    public EdcpServerOutboundDataMessageFactory(byte minimumBlockCode = 0, byte maximumBlockCode = 127)
    {
        MinimumBlockCode = minimumBlockCode;
        MaximumBlockCode = maximumBlockCode;
        _blockCode = MinimumBlockCode;
    }

    /// <summary>
    /// The minimum blockcode sent by the client. Default: 0
    /// </summary>
    public byte MinimumBlockCode { get; }

    /// <summary>
    /// The maximum blockcode sent by the client. Default: 127
    /// </summary>
    public byte MaximumBlockCode { get; }

    /// <summary>
    /// Create am outbound data message from an order management parameter set
    /// </summary>
    /// <param name="parameterSet">Order management parameterset</param>
    /// <returns>Outbound data message</returns>
    public IOutboundDataMessage CreateInstance(IParameterSet parameterSet)
    {
        if (parameterSet is not EdcpParameterSet ps)
        {
            throw new ArgumentException($"ParameterSet is not of type {nameof(EdcpParameterSet)}");
        }

        var msg = new EdcpOutboundDataMessage
        {
            DataBlock = ps,
            BlockCode = _blockCode
        };

        if (_blockCode == 127)
        {
            _blockCode = 0;
        }
        else
        {
            _blockCode++;
        }

        return msg;
    }

    /// <summary>
    /// Resets the blockcode to start blockcode of 128
    /// </summary>
    public void Reset()
    {
        _blockCode = 0;
    }
}