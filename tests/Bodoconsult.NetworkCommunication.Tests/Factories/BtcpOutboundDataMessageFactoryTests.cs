// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

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

        var factory = new BtcpOutboundDataMessageFactory();

        // Act  
        var message = (BtcpOutboundDataMessage)factory.CreateInstance(ps);

        // Assert
        Assert.That(message, Is.Not.Null);
        Assert.That(message.BusinessTransactionId, Is.EqualTo(ps.BusinessTransactionId));
    }
}