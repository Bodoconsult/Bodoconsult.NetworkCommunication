// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpBackend.Bll.BusinessLogic.AdapterFactories;

namespace IpCommunicationSampleTests.Backend.AdapterFactories;

[TestFixture]
internal class BtcpClientTcpIpBusinessLogicAdapterFactoryTests
{
    private readonly IAppLoggerProxy _appLogger = TestDataHelper.GetFakeAppLoggerProxy();
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
        var dsm = new BtcpClientTcpIpBusinessLogicAdapterFactory(businessTransactionManager);

        // Act  
        var result = dsm.CreateInstance(device);

        // Assert
        Assert.That(result.IpDevice, Is.EqualTo(device));

        Assert.That(result is ISimpleDeviceBusinessLogicAdapter, Is.True);

        var adapter = result as ISimpleDeviceBusinessLogicAdapter;

        Assert.That(adapter, Is.Not.Null);
        Assert.That(adapter.IpDevice, Is.EqualTo(device));
    }
}