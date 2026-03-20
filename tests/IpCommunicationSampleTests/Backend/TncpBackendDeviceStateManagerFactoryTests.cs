// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.BusinessLogic;

namespace IpCommunicationSampleTests.Backend;

[TestFixture]
internal class TncpBackendDeviceBusinessLogicAdapterFactoryTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();
        var dsm = new TncpBackendTcpIpDeviceBusinessLogicAdapterFactory();

        // Act  
        var result = dsm.CreateInstance(device);

        // Assert
        Assert.That(result.IpDevice, Is.EqualTo(device));

        Assert.That(result.GetType(), Is.EqualTo(typeof(IStateMachineDeviceBusinessLogicAdapter)));

        var adapter = result as IStateMachineDeviceBusinessLogicAdapter;

        Assert.That(adapter, Is.Not.Null);
        Assert.That(adapter.Device, Is.EqualTo(device));
        Assert.That(adapter.StateFactory, Is.Null);
    }
}