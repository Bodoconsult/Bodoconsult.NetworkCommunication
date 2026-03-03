// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement;

/// <summary>
/// Request spec doing an action on the device
/// </summary>
public class DeviceRequestSpec : BaseRequestSpec, IDeviceRequestSpec
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="name">Request name</param>
    /// <param name="parameterSet">Current parameter set</param>
    public DeviceRequestSpec(string name, IParameterSet parameterSet) : base(name, parameterSet)
    { }

    /// <summary>
    /// The messages to send. These messages are processed all in the same way
    /// defined by the request
    /// </summary>
    public List<IOutboundDataMessage> SentMessage { get; } = new();

    /// <summary>
    /// Send a data message to the device
    /// </summary>
    public SendDataMessageDelegate SendDataMessageDelegate { get; set; }

    /// <summary>
    /// Current sent message
    /// </summary>
    public IOutboundDataMessage CurrentSentMessage { get; set; }

    /// <summary>
    /// The next step in the chain
    /// </summary>
    public IRequestAnswerStep NextChainElement { get; set; }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public override void Dispose()
    {
        ResultTransportObject = null;
        TransportObject = null;
        SentMessage.Clear();
        base.Dispose();

        //foreach (var requestAnswerStep in RequestAnswerSteps)
        //{
        //    requestAnswerStep.Dispose();
        //}

        //RequestAnswerSteps.Clear();

        
    }
}