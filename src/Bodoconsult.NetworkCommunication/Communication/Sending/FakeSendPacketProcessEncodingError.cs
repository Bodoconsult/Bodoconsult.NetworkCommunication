// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication.Sending;

/// <summary>
/// Fake implementation of <see cref="ISendPacketProcess"/> to avoid real TCP/IP sending of messages. Simulates a encoding error
/// </summary>
public class FakeSendPacketProcessEncodingError : FakeSendPacketProcess
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
        DataMessagingConfig.RaiseDataMessageSentDelegate?.Invoke(new ReadOnlyMemory<byte>(Message.RawMessageData.ToArray()));
        return true;
    }

}