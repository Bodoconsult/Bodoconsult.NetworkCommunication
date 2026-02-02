// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication.Sending;

/// <summary>
/// Fake implementation of <see cref="ISendPacketProcess"/> to avoid real TCP/IP sending of messages. Simulates a socket error
/// </summary>
public class FakeSendPacketProcessSocketError : FakeSendPacketProcess
{
    /// <summary>
    /// Excute the step
    /// </summary>
    public override void Execute()
    {
        SendMessage();
        ProcessExecutionResult = OrderExecutionResultState.Unsuccessful;
    }

    /// <summary>
    /// Send the message
    /// </summary>
    public override bool SendMessage()
    {
        DataMessagingConfig.RaiseDataMessageNotSentDelegate?.Invoke(new ReadOnlyMemory<byte>(Message.RawMessageData.ToArray()), "Blubb");
        DataMessagingConfig.RaiseComDevCloseRequestDelegate?.Invoke("Blubb");
        return true;
    }

}