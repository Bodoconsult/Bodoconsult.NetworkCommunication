// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Interfaces;
using System.Net;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Protocols.Udp;

namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

internal static class UdpClientIpTestHelper
{
    private static readonly IAppLoggerProxy Logger =  TestDataHelper.GetFakeAppLoggerProxy();
    
    /// <summary>
    /// Initialize the IP communication
    /// </summary>
    public static void InitServer(IUdpTests testSetup)
    {
        testSetup.DataMessagingConfig = new DefaultDataMessagingConfig();
        testSetup.DataMessagingConfig.Port = TestDataHelper.GetRandomPort();
        testSetup.DataMessagingConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(testSetup.DataMessagingConfig);
        testSetup.DataMessagingConfig.AppLogger = Logger;
        testSetup.DataMessagingConfig.MonitorLogger = Logger;

        testSetup.IpAddress = IPAddress.Parse(testSetup.DataMessagingConfig.IpAddress);

        testSetup.RemoteUdpDevice?.Dispose();

        testSetup.RemoteUdpDevice = new UdpTestUniCastServer(testSetup.IpAddress, testSetup.DataMessagingConfig.Port, testSetup.DataMessagingConfig.Port + 1);
        testSetup.RemoteUdpDevice.Start();
    }

    public static void InitSocket(IUdpTests testSetup)
    {
        //// Soft reset server
        //testSetup.Server.ResetClientSocket();
        ////testSetup.Smddevice.AppLogger = testSetup.Logger;
        ////testSetup.Smddevice.MonitorLogger = testSetup.Logger;

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
        var socket = new UdpServerSocketProxy();
        socket.IpAddress = testSetup.IpAddress;
        socket.Port = testSetup.DataMessagingConfig.Port + 1;
        socket.RemotePort = testSetup.DataMessagingConfig.Port;
        testSetup.Socket = socket;

        testSetup.Socket.Connect().Wait();

        testSetup.Logger = Logger;
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

    //public static void InitFakeSocket(ITcpdeviceTests testSetup)
    //{
    //    testSetup.Socket = new FakeTcpIpSocketProxy();
    //}
}