// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

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
        var device = TestDataHelper.CreateStateMachineDevice();
        var dsm = new TncpIpDeviceTcpIpBusinessLogicAdapterFactory();

        // Act  
        var result = dsm.CreateInstance(device);

        // Assert
        Assert.That(result.IpDevice, Is.EqualTo(device));

        Assert.That(result is IStateMachineDeviceBusinessLogicAdapter, Is.True);

        var adapter = result as IStateMachineDeviceBusinessLogicAdapter;

        Assert.That(adapter, Is.Not.Null);
        Assert.That(adapter.IpDevice, Is.EqualTo(device));
        Assert.That(adapter.Device, Is.EqualTo(device));
        Assert.That(adapter.StateFactory, Is.Null);
    }
}