// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

public class MessageSendingResult
{

    public MessageSendingResult(IDataMessage message, IOrderExecutionResultState processExecutionResult)
    {
        Message = message;
        ProcessExecutionResult = processExecutionResult;
    }


    public IDataMessage Message { get; }

       
    public IOrderExecutionResultState ProcessExecutionResult { get; }
}