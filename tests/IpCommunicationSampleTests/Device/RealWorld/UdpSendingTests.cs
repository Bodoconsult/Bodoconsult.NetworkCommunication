// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Testing;
using IpDevice.Bll.Interfaces;
using IpDeviceService.DiContainerProvider;
using System.Net;
using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Tests.App;
using DynamicData;
using IpCommunicationSampleTests.App;

namespace IpCommunicationSampleTests.Device.RealWorld;

[TestFixture]
[NonParallelizable]
[SingleThreaded]
internal class UdpSendingTests
{
    private IIpDeviceManager? _deviceManager;

    // { 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6e, 0x75, 0x6d, 0x62, 0x65, 0x72, 0x2c, 0x31, 0xd }

    /// <summary>
    /// Current UDP remote device to send data to the socket
    /// </summary>
    ITcpIpDevice? RemoteTcpIpDevice { get; set; }

    /// <summary>
    /// Current UDP remote device to send data to the socket
    /// </summary>
    IUdpDevice? RemoteUdpDevice { get; set; }

    private readonly TwoNetworkDevicesAppStartParameter _startParams = new()
    {
        IpAddress = "127.0.0.1",
        Port = 33001,
        IpAddress2 = "127.0.0.1",
        Port2 = 33002
    };

    //private CancellationTokenSource _cts;

    [TearDown]
    public void Cleanup()
    {
        RemoteUdpDevice?.Dispose();
        RemoteTcpIpDevice?.Dispose();
    }

    private void StartUdpClient()
    {
        RemoteUdpDevice = new UdpTestUniCastClient(IPAddress.Parse(_startParams.IpAddress), _startParams.Port);
        RemoteUdpDevice.Start();
        //// Send cleint hello
        //RemoteUdpDevice.Send([0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x66, 0x72, 0x6f, 0x6d, 0x20, 0x63, 0x6c, 0x69, 0x65, 0x6e, 0x74]);

        //_cts = new CancellationTokenSource();
        //Task.Run(UdpReceiving, _cts.Token);
    }

    private void StartTcpIpClient()
    {
        RemoteTcpIpDevice = new TcpTestClient(IPAddress.Parse(_startParams.IpAddress2), _startParams.Port2);
        RemoteTcpIpDevice.Start();

        //_cts = new CancellationTokenSource();
        //Task.Run(UdpReceiving, _cts.Token);
    }

    private void CreateAndStartDevice()
    {
        var globals = Globals.Instance;

        var factory = new IpDeviceServiceProductionDiContainerServiceProviderPackageFactory(globals);
        var package = factory.CreateInstance();

        package.AddServices(globals.DiContainer);

        globals.DiContainer.BuildServiceProvider();

        var deviceTcpIpConfig = new IpConfig { IpAddress = _startParams.IpAddress2, Port = _startParams.Port2 };
        var deviceUdpConfig = new IpConfig { IpAddress = _startParams.IpAddress, Port = _startParams.Port };

        _deviceManager = Globals.Instance.DiContainer.Get<IIpDeviceManager>();
        _deviceManager.BackendTcpIpConfig = deviceTcpIpConfig;
        _deviceManager.BackendUdpConfig = deviceUdpConfig;

        _deviceManager.LoadBackendUdp();
        _deviceManager.LoadBackendTcpIp();
        _deviceManager.LoadBusinessTransactions();

        ArgumentNullException.ThrowIfNull(_deviceManager.BackendUdp);
        ArgumentNullException.ThrowIfNull(_deviceManager.BackendTcpIp);
        ArgumentNullException.ThrowIfNull(_deviceManager.BackendUdp.IpDevice);
        ArgumentNullException.ThrowIfNull(_deviceManager.BackendTcpIp.IpDevice);

        _deviceManager.BackendUdp.IpDevice.StartComm();
        _deviceManager.BackendTcpIp.IpDevice.StartComm();
    }

    [Test]
    public void StartStreaming2_ValidSetup_MessagesSent()
    {
        //const string hello = "Hello from client";
        //var bytes = Encoding.ASCII.GetBytes(hello);
        //Debug.Print($"{ArrayHelper.GetStringFromArrayCsharpStyle(bytes)}");

        // Arrange 
        CreateAndStartDevice();
        ArgumentNullException.ThrowIfNull(_deviceManager?.BackendUdp?.DeviceBusinessLogicAdapter);

        StartUdpClient();
        ArgumentNullException.ThrowIfNull(RemoteUdpDevice);

        var adapter = (IBackendUdpBusinessLogicAdapter)_deviceManager.BackendUdp.DeviceBusinessLogicAdapter;

        var request = new EmptyBusinessTransactionRequestData();
        var request2 = new EmptyBusinessTransactionRequestData();

        // Act  
        adapter.StartStreaming(request);

        Wait.Until(CheckMessages);

        adapter.StopStreaming(request2);

        // Assert
        Assert.That(CheckMessages(), Is.True);

        Debug.Print($"Messages received: {RemoteUdpDevice?.ReceivedMessages.Count}");
    }

    [Test]
    public void TcpClientStartStreaming2_ValidSetup_MessagesSent()
    {
        //const string hello = "Hello from client";
        //var bytes = Encoding.ASCII.GetBytes(hello);
        //Debug.Print($"{ArrayHelper.GetStringFromArrayCsharpStyle(bytes)}");

        //var blubb = new byte[]
        //    {
        //        0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6d, 0x6f, 0x64, 0x65, 0x2c, 0x73,
        //        0x6e, 0x61, 0x70, 0x73, 0x68, 0x6f, 0x74, 0x2c, 0x63, 0x6f, 0x6e, 0x74, 0x69, 0x6e, 0x69, 0x6f, 0x75,
        //        0x73, 0xd, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x72, 0x65, 0x61, 0x6d, 0x2c, 0x6e, 0x75, 0x6d, 0x62,
        //        0x65, 0x72, 0x2c, 0x34, 0xd, 0x73, 0x65, 0x74, 0x2c, 0x73, 0x74, 0x61, 0x74, 0x75, 0x73, 0x2c, 0x73,
        //        0x74, 0x61, 0x72, 0x74, 0xd
        //    };
        //Debug.Print(Encoding.UTF8.GetString(blubb));

        // Arrange 
        CreateAndStartDevice();
        ArgumentNullException.ThrowIfNull(_deviceManager?.BackendUdp?.DeviceBusinessLogicAdapter);

        StartTcpIpClient();
        ArgumentNullException.ThrowIfNull(RemoteTcpIpDevice);

        StartUdpClient();
        ArgumentNullException.ThrowIfNull(RemoteUdpDevice);

        // Act  
        var data = new List<byte>();
        data.AddRange(Encoding.UTF8.GetBytes("set,stream,number,4"));
        data.Add([DeviceCommunicationBasics.Lf]);
        AsyncHelper.FireAndForget(() =>
        {
            RemoteTcpIpDevice.Send(data.ToArray());
        });

        Task.Delay(100);

        var data2 = new List<byte>();
        data2.AddRange(Encoding.UTF8.GetBytes("set,stream,mode,continious"));
        data2.Add([DeviceCommunicationBasics.Lf]);

        AsyncHelper.FireAndForget(() =>
        {
            RemoteTcpIpDevice.Send(data2.ToArray());
        });

        Task.Delay(100);

        var data3 = new List<byte>();
        data3.AddRange(Encoding.UTF8.GetBytes("set,status,start"));
        data3.Add([DeviceCommunicationBasics.Lf]);
        AsyncHelper.FireAndForget(() =>
        {
            RemoteTcpIpDevice.Send(data3.ToArray());
        });

        Task.Delay(1000);

        Wait.Until(CheckMessages);

        //adapter.StopStreaming(request2);

        // Assert
        Assert.That(CheckMessages(), Is.True);

        Debug.Print($"Messages received: {RemoteUdpDevice?.ReceivedMessages.Count}");
    }

    private bool CheckMessages()
    {
        //Debug.Print($"RemoteUdpDevice?.ReceivedMessages.Count {RemoteUdpDevice?.ReceivedMessages.Count}");
        return RemoteUdpDevice?.ReceivedMessages.Count > 5;
    }

}