// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication.Sending;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Create an instance of <see cref="FakeSendPacketProcess"/> to fake implement <see cref="ISendPacketProcess"/>
/// </summary>
public class FakeSendPacketProcessFactory : ISendPacketProcessFactory
{

    /// <summary>
    /// Current type of fake send packet process result
    /// </summary>
    public FakeSendPacketProcessEnum TypeOfFakeSendPacketProcessEnum { get; set; } =
        FakeSendPacketProcessEnum.Successful;

    /// <summary>
    /// Create a instance of <see cref="ISendPacketProcess "/> to send a message to the tower
    /// </summary>
    /// <param name="duplexIo">Current Duplex-IO instance</param>
    /// <param name="message">Current message to send</param>
    /// <param name="config">Current device config</param>
    /// <returns>A send packet process instance to hande the message to send</returns>
    public ISendPacketProcess CreateInstance(IDuplexIo duplexIo, IDataMessage message,
        IDataMessagingConfig config)
    {
        switch (TypeOfFakeSendPacketProcessEnum)
        {
            case FakeSendPacketProcessEnum.Successful:
                return CreateInstanceFakeSendPacketProcess(message, config);
            case FakeSendPacketProcessEnum.EncodingError:
                return CreateInstanceFakeSendPacketProcessEncodingError(message, config);
            case FakeSendPacketProcessEnum.SocketError:
                return CreateInstanceFakeSendPacketProcessSocketError(message, config);
            default:
                return CreateInstanceFakeSendPacketProcess(message, config);
        }
    }

    /// <summary>
    /// Reset the <see cref="SendPacketProcess"/> instance and give it back to pool
    /// </summary>
    /// <param name="sendPacketProcess">Instance to return</param>
    public void EnqueueInstance(ISendPacketProcess sendPacketProcess)
    {
        // Do nothing
    }

    private static ISendPacketProcess CreateInstanceFakeSendPacketProcess(IDataMessage message, IDataMessagingConfig config)
    {
        var process = new FakeSendPacketProcess
        {
            DataMessagingConfig = config,
            Message = message,
        };
        return process;
    }

    private static ISendPacketProcess CreateInstanceFakeSendPacketProcessEncodingError(IDataMessage message, IDataMessagingConfig config)
    {
        var process = new FakeSendPacketProcessEncodingError
        {
            DataMessagingConfig = config,
            Message = message,
        };
        return process;
    }

    private static ISendPacketProcess CreateInstanceFakeSendPacketProcessSocketError(IDataMessage message, IDataMessagingConfig config)
    {
        var process = new FakeSendPacketProcessSocketError
        {
            DataMessagingConfig = config,
            Message = message,
        };
        return process;
    }
}