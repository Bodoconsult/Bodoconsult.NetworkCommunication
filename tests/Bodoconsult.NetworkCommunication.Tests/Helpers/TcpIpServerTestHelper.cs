// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;
using Bodoconsult.NetworkCommunication.Testing;
using Bodoconsult.NetworkCommunication.Tests.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

public static class TcpIpServerTestHelper
{
    private static readonly IAppLoggerProxy Logger = TestDataHelper.GetFakeAppLoggerProxy();

    /// <summary>
    /// Default ctor
    /// </summary>
    static TcpIpServerTestHelper()
    {
        TcpIpListenerManager = new();
    }

    /// <summary>
    /// Current <see cref="TcpIpListenerManager"/> instance
    /// </summary>
    public static TcpIpListenerManager TcpIpListenerManager { get; }

    /// <summary>
    /// Initialize the IP communication
    /// </summary>
    public static void InitClient(ITcpTests testSetup)
    {
        testSetup.DataMessagingConfig = new DefaultDataMessagingConfig();
        testSetup.DataMessagingConfig.Port = TestDataHelper.GetRandomPort();
        testSetup.DataMessagingConfig.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(testSetup.DataMessagingConfig);
        testSetup.DataMessagingConfig.AppLogger = Logger;
        testSetup.DataMessagingConfig.MonitorLogger = Logger;

        testSetup.IpAddress = IPAddress.Parse(testSetup.DataMessagingConfig.IpAddress);

        testSetup.RemoteTcpIpDevice?.Dispose();
        testSetup.RemoteTcpIpDevice = new TcpTestClient(testSetup.IpAddress, testSetup.DataMessagingConfig.Port);
        
    }

    public static void InitSocket(ITcpTests testSetup)
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

        TcpIpListenerManager.ClearAll();

        // Load socket
        var socket = new TcpIpServerSocketProxy(TcpIpListenerManager);
        socket.IpAddress = testSetup.IpAddress;
        socket.Port = testSetup.DataMessagingConfig.Port;
        socket.RemotePort = testSetup.DataMessagingConfig.Port + 1;
        socket.Connect().Wait(1000);
        testSetup.Socket = socket;
        

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

    //public static void InitFakeSocket(ITcpdeviceTests testSetup)
    //{
    //    testSetup.Socket = new FakeTcpIpSocketProxy();
    //}
}