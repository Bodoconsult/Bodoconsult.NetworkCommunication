// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class BtcpOutboundDataMessageFactoryTests
{
    [Test]
    public void CreateInstance_ValidOrder_MessageCreated()
    {
        // Arrange 
        var ps = new BtcpParameterSet();
        ps.BusinessTransactionId = 57;

        var order = new BtcpOrder(ps, TestDataHelper.AppDateService, TestDataHelper.GetFakeAppBenchProxy());

        var factory = new BtcpOutboundDataMessageFactory();

        // Act  
        var message = (BtcpOutboundDataMessage)factory.CreateInstance(order);

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.BusinessTransactionId, Is.EqualTo(ps.BusinessTransactionId));
    }
}