// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.Processors;

[TestFixture]
internal class DeviceRequestSpecTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorectly()
    {
        // Arrange 
        var order = TestDataHelper.CreateSdcpOrder();

        var ps = new EmptyParameterSet();
        ps.LoadOrder(order);

        // Act  
        var rs = new DeviceRequestSpec("Test", ps);

        // Assert
        Assert.That(rs.ParameterSet, Is.EqualTo(ps));
        Assert.That(rs.SentMessage.Count, Is.EqualTo(0));
        Assert.That(string.IsNullOrEmpty(rs.Name), Is.False);

    }
}