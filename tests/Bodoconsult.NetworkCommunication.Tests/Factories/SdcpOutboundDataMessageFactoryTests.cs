// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;

namespace Bodoconsult.NetworkCommunication.Tests.Factories;

[TestFixture]
internal class SdcpOutboundDataMessageFactoryTests
{
    [Test]
    public void CreateInstance_ValidOrder_MessageCreated()
    {
        // Arrange 
        var ps = new SdcpParameterSet();

        var factory = new SdcpOutboundDataMessageFactory();

        // Act  
        var message = (SdcpOutboundDataMessage)factory.CreateInstance(ps);

        // Assert
        Assert.That(message, Is.Not.Null);
    }
}