// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using IpBackend.Bll.BusinessLogic.AdapterFactories;

namespace IpCommunicationSampleTests.Backend.AdapterFactories;

[TestFixture]
internal class SfxpIpDeviceUdpBusinessLogicAdapterFactoryTests
{
    private readonly IBusinessTransactionManager _businessTransactionManager = new FakeBusinessTransactionManager();

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var device = TestDataHelper.CreateSimpleDevice();
        var dsm = new SfxpIpDeviceUdpBusinessLogicAdapterFactory(_businessTransactionManager);

        // Act  
        var result = dsm.CreateInstance(device);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.IpDevice, Is.EqualTo(device));

            Assert.That(result, Is.InstanceOf<ISimpleDeviceBusinessLogicAdapter>());

            var adapter = result as ISimpleDeviceBusinessLogicAdapter;

            Assert.That(adapter, Is.Not.Null);

            ArgumentNullException.ThrowIfNull(adapter);
            Assert.That(adapter.IpDevice, Is.EqualTo(device));
        }
    }
}