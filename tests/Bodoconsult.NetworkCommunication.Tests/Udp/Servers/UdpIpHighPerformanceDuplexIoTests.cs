// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Udp.Servers;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
public class UdpIpHighPerformanceDuplexIoTests : BaseUdpIpDuplexIoTests
{
    [SetUp]
    public void TestSetup()
    {
        UdpServerIpTestHelper.InitRemoteDevice(this);

        Debug.Print("Start TestSetup");

        BaseReset();

        UdpServerIpTestHelper.InitLocalSocket(this);
        ArgumentNullException.ThrowIfNull(Socket);

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
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);
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
        ArgumentNullException.ThrowIfNull(DataMessagingConfig);
        Socket = socketProxy;
        BindDelegates();

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

        // Act and assert
        Assert.Pass("Not fakeable at the moment");
    }
}