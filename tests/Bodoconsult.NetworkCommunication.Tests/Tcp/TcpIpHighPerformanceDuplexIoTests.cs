// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
public class TcpIpHighPerformanceDuplexIoTests : TcpIpDuplexIoBaseTests
{

    public TcpIpHighPerformanceDuplexIoTests()
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

        DataMessagingConfig.SocketProxy = socketProxy;
        DataMessagingConfig.DataMessageProcessingPackage.WaitStateManager.RaiseHandshakeReceivedDelegate = OnHandshakeReceivedDelegate;
        DataMessagingConfig.RaiseCommLayerDataMessageReceivedDelegate = OnRaiseDataMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigMessageNotReceivedDelegate = OnRaiseDataMessagingConfigMessageNotReceivedEvent;
        DataMessagingConfig.RaiseComDevCloseRequestDelegate = OnRaiseRequestComDevCloseEvent;
        DataMessagingConfig.RaiseUnexpectedDataMessageReceivedDelegate = OnNotExpectedMessageReceivedEvent;
        //DataMessagingConfig.RaiseDataMessagingConfigCorruptedMessageDelegate = OnCorruptedMessage;
        DataMessagingConfig.RaiseDataMessageNotSentDelegate = OnRaiseDataMessageNotSentEvent;
        DataMessagingConfig.DuplexIoErrorHandlerDelegate = CentralErrorHandling;
        DataMessagingConfig.RaiseDataMessageSentDelegate = OnRaiseDataMessageSentEvent;

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
}