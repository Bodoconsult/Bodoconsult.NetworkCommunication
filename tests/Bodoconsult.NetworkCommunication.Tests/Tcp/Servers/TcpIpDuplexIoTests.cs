// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Tcp.Servers;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
public class TcpIpDuplexIoTests : TcpIpDuplexIoBaseTests
{
    [SetUp]
    protected void TestSetup()
    {
        TcpIpServerTestHelper.InitClient(this);

        Debug.Print("Start TestSetup");
        
        BaseReset();

        TcpIpServerTestHelper.InitSocket(this);

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
        BindDelegates();

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
        BindDelegates();

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