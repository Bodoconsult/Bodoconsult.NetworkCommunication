// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.OrderManagement.ParameterSets;

[TestFixture]
internal class SdcpParameterSetTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var ps = new SdcpParameterSet();

        // Assert
        Assert.That(ps, Is.Not.Null);
        Assert.That(ps.GetType().GetInterfaces().Contains(typeof(IParameterSet)));
    }

    [Test]
    public void IsValid_ValidSetup_ReturnsTrue()
    {
        // Arrange 
        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x5, 0x6, 0x7 };

        // Act  
        var result = ps.IsValid;

        // Assert
        Assert.That(result.Any(), Is.False);
    }


    [Test]
    public void IsValid_InvalidSetup_ReturnsFalse()
    {
        // Arrange 
        var ps = new SdcpParameterSet();

        // Act  
        var result = ps.IsValid;

        // Assert
        Assert.That(result.Any(), Is.True);
    }

    [Test]
    public void LoadOrder_TestOrder_OrderLoaded()
    {
        // Arrange 
        var ps = new SdcpParameterSet();

        TestDataHelper.GetFakeAppBenchProxy();

        // Act  
        var order = TestDataHelper.CreateTestOrder(ps);

        // Assert
        Assert.That(order, Is.Not.Null);
        Assert.That(ps.CurrentOrder, Is.Not.Null);
    }

    [Test]
    public void ToDataBlock_ValidSetup_DataFilled()
    {
        // Arrange 
        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x5, 0x6, 0x7 };

        // Act  
        ps.ToDataBlock();

        // Assert
        Assert.That(ps.Data.Length, Is.EqualTo(ps.Payload.Length));
    }
}