// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

public class MessageHandlingResult
{
    public MessageHandlingResult()
    {
        ErrorDescription = "";
        //DataBlock = "";
        //Result = 0;
    }

    /// <summary>
    /// The result of the message execution
    /// </summary>
    public IOrderExecutionResultState ExecutionResult { get; set; } = OrderExecutionResultState.Successful;

    /// <summary>
    /// A object to transferred from one request spec to the next
    /// </summary>
    public object TransportObject { get; set; }

    /// <summary>
    /// In case of an error the clear text description of the error
    /// </summary>
    public  string ErrorDescription { get; set; }


    //public string DataBlock { get; set; }
        

        


    /// <summary>
    /// Error code
    /// </summary>
    public byte Error { get; set; }
        
    //public int Result { get; set; }

    //public int DwError { get; set; }

    //public char ReceiveState { get; set; }

    //public int DataLength { get; set; }

    //public bool ExpectedMessageAnswerAndNoErrorFlag { get; set; }

    //public int AsciiCodeOfdeviceState { get; set; }

}