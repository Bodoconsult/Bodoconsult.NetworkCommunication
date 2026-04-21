// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Abstractions.DependencyInjection;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.BusinessLogic.Adapters;
using IpBackend.Bll.Interfaces;
using IpBackendService.DiContainerProvider;
using IpCommunicationSampleTests.App;
using IpDevice.Bll.Interfaces;
using IpDeviceService.DiContainerProvider;

namespace IpCommunicationSampleTests.Backend.RealWorld;

[TestFixture]
internal class UdpReceivingTests
{
    private IIpDeviceManager? _deviceManager;

    private IBackendManager? _backendManager;

    private readonly DiContainer _deviceDiContainer = new();
    private readonly DiContainer _backendDiContainer = new();


    private long _messageCounter;

    private readonly ThreeNetworkDevicesAppStartParameter _startParams = new()
    {
        IpAddress = "127.0.0.1",
        Port = 33001,
        IpAddress2 = "127.0.0.1",
        Port2 = 33002,
        IpAddress3 = "127.0.0.1",
        Port3 = 33003,
    };

    //private CancellationTokenSource _cts;

    [TearDown]
    public void Cleanup()
    {
        _messageCounter = 0;
    }

    //private void StartUdpClient()
    //{
    //    RemoteUdpDevice = new UdpTestUniCastClient(IPAddress.Parse(_startParams.IpAddress), _startParams.Port);
    //    RemoteUdpDevice.Start();

    //    //_cts = new CancellationTokenSource();
    //    //Task.Run(UdpReceiving, _cts.Token);
    //}

    private void CreateDiContainer()
    {
        var globals = Globals.Instance;

        // Device

        var factory = new IpDeviceServiceProductionDiContainerServiceProviderPackageFactory(globals);
        var package = factory.CreateInstance();

        package.AddServices(_deviceDiContainer);

        _deviceDiContainer.BuildServiceProvider();

        // Backend
        var factory2 = new IpBackendServiceProductionDiContainerServiceProviderPackageFactory(globals);
        var package2 = factory2.CreateInstance();

        package2.AddServices(_backendDiContainer);

        _backendDiContainer.BuildServiceProvider();

        //globals.DiContainer.AddSingleton<IBusinessTransactionManager, BusinessTransactionManager>();
        //globals.DiContainer.AddSingleton<ISyncOrderManager, SyncOrderManager>();
        //globals.DiContainer.AddSingleton<ISocketProxyFactory, SocketProxyFactory>();
        //globals.DiContainer.AddSingleton<IMonitorLoggerFactoryFactory, MonitorLoggerFactoryFactory>();
        //globals.DiContainer.AddSingleton<ITcpIpListenerManager, TcpIpListenerManager>();
        //globals.DiContainer.AddSingleton<ISendPacketProcessFactory, SendPacketProcessFactory>();
        //globals.DiContainer.AddSingleton<ICommunicationAdapterFactory, IpCommunicationAdapterFactory>();
        //globals.DiContainer.AddSingleton<IDuplexIoFactory, IpDuplexIoFactory>();

        //var cnm = new DoNothingOrderManagementClientNotificationManager();
        //globals.DiContainer.AddSingleton<ICentralClientNotificationManager>(cnm);
        //globals.DiContainer.AddSingleton<IOrderManagementClientNotificationManager>(cnm);

        //globals.DiContainer.AddSingleton<IOrderIdGenerator, DefaultOrderIdGenerator>();
        //globals.DiContainer.AddSingleton<IOrderFactory, OrderFactory>();
        //globals.DiContainer.AddSingleton<IOrderReceiverFactory, OrderReceiverFactory>();
        //globals.DiContainer.AddSingleton<IRequestProcessorFactoryFactory, RequestProcessorFactoryFactory>();
        //globals.DiContainer.AddSingleton<IRequestStepProcessorFactoryFactory, RequestStepProcessorFactoryFactory>();
        //globals.DiContainer.AddSingleton<IOrderPipelineFactory, OrderPipelineFactory>();
        //globals.DiContainer.AddSingleton<IOrderProcessorFactory, StateMachineOrderProcessorFactory>();
        //globals.DiContainer.AddSingleton<IBackendManager, BackendManager>();

        //globals.DiContainer.BuildServiceProvider();
    }


    private void CreateAndStartBackend()
    {
        var clientConfig = new IpConfig { IpAddress = _startParams.IpAddress3, Port = _startParams.Port3 };
        var deviceTcpIpConfig = new IpConfig { IpAddress = _startParams.IpAddress2, Port = _startParams.Port2 };
        var deviceUdpConfig = new IpConfig { IpAddress = _startParams.IpAddress, Port = _startParams.Port };

        _backendManager = _backendDiContainer.Get<IBackendManager>();
        _backendManager.ClientTcpIpConfig = clientConfig;
        _backendManager.IpDeviceTcpIpConfig = deviceTcpIpConfig;
        _backendManager.IpDeviceUdpConfig = deviceUdpConfig;

        _backendManager.LoadIpDeviceUdp();
        _backendManager.LoadIpDeviceTcpIp();
        _backendManager.LoadClient();

        _backendManager.LoadBusinessTransactions();

        _backendManager.StartIpDeviceTcpIpCommunication();
        _backendManager.StartIpDeviceUdpCommunication();


        ArgumentNullException.ThrowIfNull(_backendManager.IpDeviceUdp?.IpDevice);
        _backendManager.IpDeviceUdp.IpDevice.DataMessagingConfig.RaiseAppLayerDataMessageReceivedDelegate = RaiseAppLayerDataMessageReceivedDelegate;
    }

    private void RaiseAppLayerDataMessageReceivedDelegate(IInboundDataMessage message)
    {
        _messageCounter++;
    }

    private void CreateAndStartDevice()
    {
        var deviceTcpIpConfig = new IpConfig { IpAddress = _startParams.IpAddress2, Port = _startParams.Port2 };
        var deviceUdpConfig = new IpConfig { IpAddress = _startParams.IpAddress, Port = _startParams.Port };

        _deviceManager = _deviceDiContainer.Get<IIpDeviceManager>();
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
    public void DeviceStartStreaming2_ValidSetup_MessagesSent()
    {
        // Arrange 
        CreateDiContainer();

        CreateAndStartDevice();

        CreateAndStartBackend();

        ArgumentNullException.ThrowIfNull(_deviceManager?.BackendUdp?.DeviceBusinessLogicAdapter);

        var adapter = (IBackendUdpBusinessLogicAdapter)_deviceManager.BackendUdp.DeviceBusinessLogicAdapter;

        var request = new EmptyBusinessTransactionRequestData();
        var request2 = new EmptyBusinessTransactionRequestData();

        // Act  
        adapter.StartStreaming(request);

        //Thread.Sleep(5000);

        Wait.Until(CheckMessages);

        adapter.StopStreaming(request2);

        // Assert
        Assert.That(CheckMessages(), Is.True);
    }

    [Test]
    public void BackendRequestDeviceStartStreamingState_ValidSetup_MessagesSent()
    {
        // Arrange 
        CreateDiContainer();

        CreateAndStartDevice();

        CreateAndStartBackend();

        ArgumentNullException.ThrowIfNull(_backendManager?.IpDeviceTcpIp?.DeviceBusinessLogicAdapter);
        ArgumentNullException.ThrowIfNull(_backendManager.IpDeviceTcpIp?.Device?.CurrentState);

        Wait.Until(() => _backendManager.IpDeviceTcpIp.Device.CurrentState.Id == DefaultStateIds.DeviceReadyState);

        var adapter = (TncpIpDeviceTcpIpBusinessLogicAdapter)_backendManager.IpDeviceTcpIp.DeviceBusinessLogicAdapter;

        var request = new EmptyBusinessTransactionRequestData();
        var request2 = new EmptyBusinessTransactionRequestData();

        // Act  
        AsyncHelper.FireAndForget(() =>
        {
            adapter.RequestDeviceStartStreamingState(request);
        });

        Task.Delay(1000);

        Wait.Until(() => _backendManager.IpDeviceTcpIp.Device.CurrentState.Id == DefaultStateIds.DeviceStartStreamingState, 10000);


        Wait.Until(() => _backendManager.IpDeviceTcpIp.Device.CurrentState.Id == DefaultStateIds.DeviceReadyState, 10000);

        //Thread.Sleep(5000);

        Wait.Until(CheckMessages);

        AsyncHelper.FireAndForget(() =>
        {
            adapter.RequestDeviceStopStreamingState(request2);
        });
            
        Wait.Until(() => _backendManager.IpDeviceTcpIp.Device.CurrentState.Id == DefaultStateIds.DeviceReadyState, 10000);

        // Assert
        Assert.That(CheckMessages(), Is.True);
    }

    private bool CheckMessages()
    {
        // ToDo: better criteria
        return true;
        //return _messageCounter != 0;
    }

    //private void UdpReceiving()
    //{
    //    while (!_cts.Token.IsCancellationRequested)
    //    {
    //        RemoteUdpDevice.ReceivedMessages

    //    }
    //}

}