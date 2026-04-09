// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.BusinessLogic;
using IpBackend.Bll.Interfaces;
using IpCommunicationSampleTests.App;
using IpDevice.Bll.Interfaces;
using IpDeviceService.DiContainerProvider;

namespace IpCommunicationSampleTests.Backend.RealWorld
{
    [TestFixture]
    internal class UdpReceivingTests
    {
        private IIpDeviceManager? _deviceManager;

        private IBackendManager? _backendManager;

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

            var factory = new IpDeviceServiceProductionDiContainerServiceProviderPackageFactory(globals);
            var package = factory.CreateInstance();

            package.AddServices(globals.DiContainer);

            globals.DiContainer.AddSingleton<IBackendManager, BackendManager>();

            globals.DiContainer.BuildServiceProvider();
        }


        private void CreateAndStartBackend()
        {
            var clientConfig = new IpConfig { IpAddress = _startParams.IpAddress3, Port = _startParams.Port3 };
            var deviceTcpIpConfig = new IpConfig { IpAddress = _startParams.IpAddress2, Port = _startParams.Port2 };
            var deviceUdpConfig = new IpConfig { IpAddress = _startParams.IpAddress, Port = _startParams.Port };

            _backendManager = Globals.Instance.DiContainer.Get<IBackendManager>();
            _backendManager.ClientTcpIpConfig = clientConfig;
            _backendManager.IpDeviceTcpIpConfig = deviceTcpIpConfig;
            _backendManager.IpDeviceUdpConfig = deviceUdpConfig;

            _backendManager.LoadIpDeviceUdp();
            _backendManager.LoadIpDeviceTcpIp();
            _backendManager.LoadClient();

            _backendManager.LoadBusinessTransactions();

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
            // Arrange 
            CreateDiContainer();

            CreateAndStartDevice();

            CreateAndStartBackend();

            ArgumentNullException.ThrowIfNull(_deviceManager?.BackendUdp?.DeviceBusinessLogicAdapter);

            var adapter = (IBackendUdpBusinessLogicAdapter)_deviceManager.BackendUdp.DeviceBusinessLogicAdapter;

            var request = new EmptyBusinessTransactionRequestData();
            var request2 = new EmptyBusinessTransactionRequestData();

            // Act  
            adapter.StartStreaming2(request);

            //Thread.Sleep(5000);

            Wait.Until(CheckMessages);

            adapter.StopStreaming(request2);

            // Assert
            Assert.That(CheckMessages(), Is.True);
        }

        private bool CheckMessages()
        {
            return _messageCounter != 0;
        }

        //private void UdpReceiving()
        //{
        //    while (!_cts.Token.IsCancellationRequested)
        //    {
        //        RemoteUdpDevice.ReceivedMessages

        //    }
        //}

    }
}
