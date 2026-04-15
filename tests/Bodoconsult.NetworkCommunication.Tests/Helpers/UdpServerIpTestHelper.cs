// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Net;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Protocols.Udp;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

internal static class UdpServerIpTestHelper
{
    private static readonly IAppLoggerProxy Logger = TestDataHelper.GetFakeAppLoggerProxy();


    /// <summary>
    /// Create the messaging config
    /// </summary>
    /// <param name="testSetup"></param>
    public static void CreateMessagingConfig(IUdpTests testSetup)
    {
        Debug.Assert(testSetup.DataMessagingConfig==null);

        testSetup.DataMessagingConfig = new DefaultDataMessagingConfig();
        testSetup.DataMessagingConfig.Port = TestDataHelper.GetRandomPort();
        testSetup.DataMessagingConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(testSetup.DataMessagingConfig);
        testSetup.DataMessagingConfig.AppLogger = Logger;
        testSetup.DataMessagingConfig.MonitorLogger = Logger;

        testSetup.IpAddress = IPAddress.Parse(testSetup.DataMessagingConfig.IpAddress);
    }


    /// <summary>
    /// Initialize the IP communication
    /// </summary>
    public static void InitRemoteDevice(IUdpTests testSetup)
    {
        ArgumentNullException.ThrowIfNull(testSetup.DataMessagingConfig);
        ArgumentNullException.ThrowIfNull(testSetup.IpAddress);

        testSetup.RemoteUdpDevice?.Dispose();
        testSetup.RemoteUdpDevice = new UdpTestUniCastClient(testSetup.IpAddress, testSetup.DataMessagingConfig.Port);
        testSetup.RemoteUdpDevice.Start();
    }

    public static void InitLocalSocket(IUdpTests testSetup)
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
        ArgumentNullException.ThrowIfNull(testSetup.DataMessagingConfig);
        socket.Port = testSetup.DataMessagingConfig.Port;
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