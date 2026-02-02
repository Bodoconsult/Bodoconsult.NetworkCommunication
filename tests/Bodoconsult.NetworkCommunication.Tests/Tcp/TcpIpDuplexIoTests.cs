// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
public class TcpIpDuplexIoTests : TcpIpDuplexIoBaseTests
{

    public TcpIpDuplexIoTests()
    {
        TcpIpTestHelper.InitServer(this);
    }

    [SetUp]
    public void TestSetup()
    {
        Debug.Print("Start TestSetup");

        BaseReset();

        TcpIpTestHelper.InitSocket(this);

        DuplexIo = GetDuplexIo(Socket);

        Debug.Print("End TestSetup");
    }

    /// <summary>
    /// Get the <see cref="IDuplexIo"/> instance to test
    /// </summary>
    /// <param name="socketProxy">Current socket proxy to use</param>
    /// <returns><see cref="IDuplexIo"/> instance to test</returns>
    public override IDuplexIo GetDuplexIo(ISocketProxy socketProxy)
    {
        Socket = socketProxy;

        DataMessagingConfig.SocketProxy = Socket;
        //DataMessagingConfig.DataMessageProcessingPackage.WaitStateManager.RaiseHandshakeReceivedDelegate = OnHandshakeReceivedDelegate;
        DataMessagingConfig.RaiseCommLayerDataMessageReceivedDelegate = OnRaiseDataMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageNotReceivedDelegate = OnRaiseDataMessagingConfigMessageNotReceivedEvent;
        DataMessagingConfig.RaiseComDevCloseRequestDelegate = OnRaiseRequestComDevCloseEvent;
        DataMessagingConfig.RaiseUnexpectedDataMessageReceivedDelegate = OnNotExpectedMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigCorruptedMessageDelegate = OnCorruptedMessage;
        DataMessagingConfig.RaiseDataMessageNotSentDelegate = OnRaiseDataMessageNotSentEvent;
        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageSentDelegate = OnRaiseDataMessagingConfigMessageSentEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageNotSentDelegate = OnRaiseDataMessagingConfigMessageNotSentEvent;


        ISendPacketProcessFactory sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        return new IpDuplexIo(DataMessagingConfig, sendPacketProcessFactory);

    }

    /// <summary>
    /// Get the <see cref="IDuplexIo"/> instance to test
    /// </summary>
    /// <param name="socketProxy">Current socket proxy to use</param>
    /// <param name="expectedResult">Current expected result from send process</param>
    /// <returns></returns>
    public override IDuplexIo GetDuplexIoWithFakeEncodeDecoder(ISocketProxy socketProxy, FakeSendPacketProcessEnum expectedResult)
    {

        Socket = socketProxy;

        DataMessagingConfig.SocketProxy = Socket;
        DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate = OnRaiseDataMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageNotReceivedDelegate = OnRaiseDataMessagingConfigMessageNotReceivedEvent;
        DataMessagingConfig.RaiseComDevCloseRequestDelegate = OnRaiseRequestComDevCloseEvent;
        DataMessagingConfig.RaiseUnexpectedDataMessageReceivedDelegate = OnNotExpectedMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigCorruptedMessageDelegate = OnCorruptedMessage;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageSentDelegate = OnRaiseDataMessagingConfigMessageSentEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageNotSentDelegate = OnRaiseDataMessagingConfigMessageNotSentEvent;

        var sendPacketProcessFactory = new FakeSendPacketProcessFactory
        {
            TypeOfFakeSendPacketProcessEnum = expectedResult
        };
        return new IpDuplexIo(DataMessagingConfig, sendPacketProcessFactory);
    }

    //public override void SendDataAndReceive(byte[] data, byte[] data2 = null)
    //{
    //    // Arrange
    //    DuplexIo.StartCommunication().Wait();


    //    Server.Send(data);

    //    if (data2 != null)
    //    {
    //        Server.Send(data2);
    //    }

    //    var task = Task.Run(() =>
    //    {
    //        var i = 0;
    //        while (i < 200)
    //        {
    //            AsyncHelper.Delay(5).Wait();
    //            i++;
    //        }

    //    });
    //    task.Wait();

    //    // Act
    //    DuplexIo.StopCommunication().Wait();

    //    Debug.Print("Process done");

    //}

}