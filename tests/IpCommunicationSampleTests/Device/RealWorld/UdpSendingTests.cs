// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Testing;
using IpDevice.Bll.Interfaces;
using IpDeviceService.DiContainerProvider;
using System.Net;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Helpers;
using IpCommunicationSampleTests.App;

namespace IpCommunicationSampleTests.Device.RealWorld
{
    [TestFixture]
    internal class UdpSendingTests
    {
        private IIpDeviceManager? _deviceManager;

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
        }

        private void StartUdpClient()
        {
            RemoteUdpDevice = new UdpTestUniCastClient(IPAddress.Parse(_startParams.IpAddress), _startParams.Port);
            RemoteUdpDevice.Start();

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
            // Arrange 
            StartUdpClient();
            ArgumentNullException.ThrowIfNull(RemoteUdpDevice);

            CreateAndStartDevice();

            ArgumentNullException.ThrowIfNull(_deviceManager?.BackendUdp?.DeviceBusinessLogicAdapter);

            var adapter = (IBackendUdpBusinessLogicAdapter)_deviceManager.BackendUdp.DeviceBusinessLogicAdapter;

            var request = new EmptyBusinessTransactionRequestData();
            var request2 = new EmptyBusinessTransactionRequestData();

            // Act  
            adapter.StartStreaming2(request);

            Wait.Until(CheckMessages);

            adapter.StopStreaming(request2);
            Thread.Sleep(5000);

            // Assert
            Assert.That(CheckMessages(), Is.True);

            Debug.Print($"Messages received: {RemoteUdpDevice?.ReceivedMessages.Count}");
        }

        private bool CheckMessages()
        {
            return RemoteUdpDevice?.ReceivedMessages.Count != 0;
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
