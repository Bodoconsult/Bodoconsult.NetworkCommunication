// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.BusinessLogic.AdapterFactories;

namespace IpCommunicationSampleTests.Backend.AdapterFactories;

[TestFixture]
internal class SdcpIpDeviceUdpBusinessLogicAdapterFactoryTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateSimpleDevice();
        var dsm = new SdcpIpDeviceUdpBusinessLogicAdapterFactory();

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