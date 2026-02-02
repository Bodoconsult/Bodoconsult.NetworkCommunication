// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication.Sending;

/// <summary>
/// Current implementation of <see cref="ISendPacketProcess"/> for sending a message to the device directly without waiting for a response
/// </summary>
public class DirectSendPacketProcess : BaseSendPacketProcess
{
    /// <summary>
    /// Execute the entry state
    /// </summary>
    /// <returns>True if a handshake was received else false</returns>
    protected override bool ExecuteSendAndWait()
    {
        var result = SendMessage();

        if (!result)
        {
            ProcessExecutionResult = OrderExecutionResultState.Timeout;
            return false;
        }
        ProcessExecutionResult = OrderExecutionResultState.Successful;
        ProcessDone();
        return HasFinishedWithoutTimeout;
    }
}