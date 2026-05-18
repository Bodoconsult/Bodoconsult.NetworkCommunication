// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.App;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpBackend.Bll.BusinessLogic.AdapterFactories;

namespace IpCommunicationSampleTests.Backend.AdapterFactories;

[TestFixture]
internal class BtcpClientTcpIpBusinessLogicAdapterFactoryTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.Logger;
    private readonly FakeAppEventSourceFactory _appEventSourceFactory = new();

    [OneTimeTearDown]
    public void Cleanup()
    {
        _appLogger.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateSimpleDevice();
        device.LoadCommAdapter(TestDataHelper.FakeIpCommunicationAdapter);

        IBusinessTransactionManager businessTransactionManager = new BusinessTransactionManager(_appLogger, _appEventSourceFactory);
        var dsm = new BtcpClientTcpIpBusinessLogicAdapterFactory(businessTransactionManager, Globals.Instance);

        // Act  
        var result = dsm.CreateInstance(device);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IpDevice, Is.EqualTo(device));

            Assert.That(result, Is.InstanceOf<ISimpleDeviceBusinessLogicAdapter>());

            var adapter = result as ISimpleDeviceBusinessLogicAdapter;
            ArgumentNullException.ThrowIfNull(adapter);

            Assert.That(adapter, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(adapter.IpDevice);
            Assert.That(adapter.IpDevice, Is.EqualTo(device));
        }
    }
}