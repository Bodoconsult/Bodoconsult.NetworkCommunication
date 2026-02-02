// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Net;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

public static class UdpIpTestHelper
{
    /// <summary>
    /// Initialize the IP communication
    /// </summary>
    public static void InitServer(IUdpTests testSetup)
    {
        testSetup.DataMessagingConfig = new DefaultDataMessagingConfig();

        testSetup.IpAddress = IPAddress.Parse(testSetup.DataMessagingConfig.IpAddress);

        testSetup.Server?.Dispose();

        testSetup.Server = new UdpTestUniCastServer(testSetup.IpAddress, testSetup.DataMessagingConfig.Port);
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
        var socket = new AsyncTcpIpSocketProxy();
        testSetup.Socket = socket;

        var ipLocalEndPoint = new IPEndPoint(testSetup.IpAddress, testSetup.DataMessagingConfig.Port);
        testSetup.Socket.Connect(ipLocalEndPoint).Wait();

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