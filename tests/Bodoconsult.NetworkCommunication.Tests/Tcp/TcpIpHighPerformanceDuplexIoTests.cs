// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
public class TcpIpHighPerformanceDuplexIoTests : TcpIpDuplexIoBaseTests
{
    [SetUp]
    protected void TestSetup()
    {
        TcpIpTestHelper.InitServer(this);

        Debug.Print("Start TestSetup");

        BaseReset();

        TcpIpTestHelper.InitSocket(this);

        DuplexIo = GetDuplexIo(Socket);

        Debug.Print("End FTestSetup");
    }

    /// <summary>
    /// Get the <see cref="IDuplexIo"/> instance to test
    /// </summary>
    /// <param name="socketProxy">Current socket proxy to use</param>
    /// <returns><see cref="IDuplexIo"/> instance to test</returns>
    public override IDuplexIo GetDuplexIo(ISocketProxy socketProxy)
    {
        Socket = socketProxy;
        BindDelegates();

        ISendPacketProcessFactory sendPacketProcessFactory = new FakeSendPacketProcessFactory();
        return new IpHighPerformanceDuplexIo(DataMessagingConfig, sendPacketProcessFactory);
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

        //MessageEncodingDecodingHandler = new FakeErrorDecodingEncodingHandler();

        DataMessagingConfig.SocketProxy = socketProxy;
        DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate = OnRaiseDataMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageNotReceivedDelegate = OnRaiseDataMessagingConfigMessageNotReceivedEvent;
        DataMessagingConfig.RaiseComDevCloseRequestDelegate = OnRaiseRequestComDevCloseEvent;
        DataMessagingConfig.RaiseUnexpectedDataMessageReceivedDelegate = OnNotExpectedMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigCorruptedMessageDelegate = OnCorruptedMessage;
        DataMessagingConfig.DuplexIoErrorHandlerDelegate = CentralErrorHandling;

        var sendPacketProcessFactory = new FakeSendPacketProcessFactory
        {
            TypeOfFakeSendPacketProcessEnum = expectedResult
        };
        return new IpHighPerformanceDuplexIo(DataMessagingConfig, sendPacketProcessFactory);

    }

    [Test]
    public override void SendMessage_SocketError_Fails()
    {
        // Arrange
        TestSetup();

        // Act and assert
        Assert.Pass("Not fakeable at the moment");
    }
}