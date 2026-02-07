// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Net;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

public static class TcpIpServerTestHelper
{
    private static readonly IAppLoggerProxy Logger = TestDataHelper.GetFakeAppLoggerProxy();

    /// <summary>
    /// Initialize the IP communication
    /// </summary>
    public static void InitClient(ITcpTests testSetup)
    {
        testSetup.DataMessagingConfig = new DefaultDataMessagingConfig();
        testSetup.DataMessagingConfig.Port = GetRandomPort();
        testSetup.DataMessagingConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(testSetup.DataMessagingConfig);
        testSetup.DataMessagingConfig.AppLogger = Logger;
        testSetup.DataMessagingConfig.MonitorLogger = Logger;

        testSetup.IpAddress = IPAddress.Parse(testSetup.DataMessagingConfig.IpAddress);

        testSetup.RemoteTcpIpDevice?.Dispose();
        testSetup.RemoteTcpIpDevice = new TcpTestClient(testSetup.IpAddress, testSetup.DataMessagingConfig.Port+ 1, testSetup.DataMessagingConfig.Port);
        
    }

    private static int GetRandomPort()
    {
        return new Random(60000).Next(50000, 65000);
    }
    public static void InitSocket(ITcpTests testSetup)
    {
        //// Soft reset server
        //testSetup.Server.ResetClientSocket();
        ////testSetup.SmdTower.AppLogger = testSetup.Logger;
        ////testSetup.SmdTower.MonitorLogger = testSetup.Logger;

        //testSetup.Server.WaitForConnections().GetAwaiter().GetResult();


        // Close socket if necessary
        try
        {
            testSetup.Socket?.Close();
        }
        catch
        {
            // Do nothing
        }

        // Load socket
        ITcpIpListenerManager tcpIpListenerManager = new TcpIpListenerManager();
        var socket = new TcpIpServerSocketProxy(tcpIpListenerManager);
        socket.IpAddress = testSetup.IpAddress;
        socket.Port = testSetup.DataMessagingConfig.Port;
        testSetup.Socket = socket;

        testSetup.Socket.Connect().Wait();

        testSetup.Logger = Logger;
        testSetup.RemoteTcpIpDevice.Start();
    }

    /// <summary>
    /// Reset only the server
    /// </summary>
    /// <param name="testSetup"></param>
    public static void ResetServer(IUdpTests testSetup)
    {
        //// Soft reset server
        //testSetup.Server.ResetClientSocket();
        //testSetup.Server.WaitForConnections().GetAwaiter().GetResult();

    }

    //public static void InitFakeSocket(ITcpTowerTests testSetup)
    //{
    //    testSetup.Socket = new FakeTcpIpSocketProxy();
    //}
}