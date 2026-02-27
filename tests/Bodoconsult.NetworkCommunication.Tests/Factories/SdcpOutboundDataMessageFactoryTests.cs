// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class SdcpOutboundDataMessageFactoryTests
{
    [Test]
    public void CreateInstance_ValidOrder_MessageCreated()
    {
        // Arrange 
        var ps = new SdcpParameterSet();

        var order = new SdcpOrder(ps, TestDataHelper.AppDateService, TestDataHelper.GetFakeAppBenchProxy());

        var factory = new SdcpOutboundDataMessageFactory();

        // Act  
        var message = (SdcpOutboundDataMessage)factory.CreateInstance(order);

        // Assert
        Assert.That(message, Is.Not.Null);
    }
}