// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpBackend.Bll.BusinessLogic.AdapterFactories;

namespace IpCommunicationSampleTests.Backend.AdapterFactories;

[TestFixture]
internal class TncpIpDeviceDeviceBusinessLogicAdapterFactoryTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var bt = new BusinessTransactionManager(TestDataHelper.Logger,
            new FakeAppEventSourceFactory());
        var device = TestDataHelper.CreateStateMachineDevice();
        var dsm = new TncpIpDeviceTcpIpBusinessLogicAdapterFactory(bt);

        // Act  
        var result = dsm.CreateInstance(device);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IpDevice, Is.EqualTo(device));

            Assert.That(result is IStateMachineDeviceBusinessLogicAdapter, Is.True);

            var adapter = result as IStateMachineDeviceBusinessLogicAdapter;

            Assert.That(adapter, Is.Not.Null);
            ArgumentNullException.ThrowIfNull(adapter);
            Assert.That(adapter.IpDevice, Is.EqualTo(device));
            Assert.That(adapter.Device, Is.EqualTo(device));
            Assert.That(adapter.StateFactory, Is.Null);
        }
    }
}