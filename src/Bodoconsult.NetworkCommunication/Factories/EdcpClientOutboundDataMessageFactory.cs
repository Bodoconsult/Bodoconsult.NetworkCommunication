// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// EDCP cliemt side implementation of an <see cref="IOutboundDataMessageFactory"/>. Creates a block code from 128 to 255
/// </summary>
public class EdcpClientOutboundDataMessageFactory : IOutboundDataMessageFactory
{
    private byte _blockCode;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="minimumBlockCode">The minimum blockcode sent by the client. Default: 128</param>
    /// <param name="maximumBlockCode">The maximum blockcode sent by the client. Default: 255</param>
    public EdcpClientOutboundDataMessageFactory(byte minimumBlockCode = 128, byte maximumBlockCode = 255)
    {
        MinimumBlockCode = minimumBlockCode;
        MaximumBlockCode = maximumBlockCode;
        _blockCode = MinimumBlockCode;
    }

    /// <summary>
    /// The minimum blockcode sent by the client. Default: 128
    /// </summary>
    public byte MinimumBlockCode { get; }

    /// <summary>
    /// The maximum blockcode sent by the client. Default: 255
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

        if (_blockCode == 255)
        {
            _blockCode = 128;
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
        _blockCode = MinimumBlockCode;
    }
}