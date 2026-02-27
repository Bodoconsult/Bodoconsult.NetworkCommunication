// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.OrderManagement.Orders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class EdcpClientOutboundDataMessageFactoryTests
{
    [Test]
    public void CreateInstance_ValidOrder_MessageCreated()
    {
        // Arrange 
        var ps = new EdcpParameterSet();

        var order = new EdcpOrder(ps, TestDataHelper.AppDateService, TestDataHelper.GetFakeAppBenchProxy());

        var factory = new EdcpClientOutboundDataMessageFactory();

        // Act  
        var message = (EdcpOutboundDataMessage)factory.CreateInstance(order);

        // Assert
        Assert.That(message, Is.Not.Null);
    }

    [Test]
    public void CreateInstance_MultipleValidOrders_BlockCodeCorrect()
    {
        // Arrange 
        var ps = new EdcpParameterSet();

        var order = new EdcpOrder(ps, TestDataHelper.AppDateService, TestDataHelper.GetFakeAppBenchProxy());

        var factory = new EdcpClientOutboundDataMessageFactory();

        // Act  
        var shift = 0;
        for (var i = 128; i < 259; i++)
        {
            // Act
            var message = (EdcpOutboundDataMessage)factory.CreateInstance(order);

            // Assert
            Assert.That(message, Is.Not.Null);

            if (i == 256)
            {
                shift = 128;
            }

            Assert.That(message.BlockCode, Is.EqualTo(i - shift));
        }
    }
}