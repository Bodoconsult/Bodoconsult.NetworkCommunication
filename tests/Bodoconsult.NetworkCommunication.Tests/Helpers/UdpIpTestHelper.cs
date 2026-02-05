// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.TestData;
using System.Net;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Protocols.Udp;

namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

public static class UdpIpTestHelper
{
    /// <summary>
    /// Initialize the IP communication
    /// </summary>
    public static void InitServer(IUdpTests testSetup)
    {
        testSetup.DataMessagingConfig = new DefaultDataMessagingConfig();
        testSetup.DataMessagingConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(testSetup.DataMessagingConfig);

        testSetup.IpAddress = IPAddress.Parse(testSetup.DataMessagingConfig.IpAddress);

        testSetup.Server?.Dispose();

        testSetup.Server = new UdpTestUniCastServer(testSetup.IpAddress, testSetup.DataMessagingConfig.Port, testSetup.DataMessagingConfig.Port + 1);
        testSetup.Server.Start();
    }

    public static void InitSocket(IUdpTests testSetup)
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
        var socket = new AsyncUdpSocketProxy(true);
        socket.IpAddress = testSetup.IpAddress;
        socket.Port = testSetup.DataMessagingConfig.Port + 1;
        socket.ClientPort = testSetup.DataMessagingConfig.Port;
        testSetup.Socket = socket;

        testSetup.Socket.Connect().Wait();

        testSetup.Logger = TestDataHelper.GetFakeAppLoggerProxy();
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