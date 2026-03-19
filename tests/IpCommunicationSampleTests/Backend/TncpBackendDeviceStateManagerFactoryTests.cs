// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpCommunicationSample.Backend.Bll.BusinessLogic;

namespace IpCommunicationSampleTests.Backend;

[TestFixture]
internal class TncpBackendDeviceStateManagerFactoryTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateStateMachineDevice();
        var dsm = new TncpBackendDeviceStateManagerFactory();

        // Act  
        var result = dsm.CreateInstance(device);

        // Assert
        Assert.That(result.Device, Is.EqualTo(device));
        Assert.That(result.StateFactory, Is.Null);
    }
}