// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpDevice.Bll.BusinessLogic.Adapters;

namespace IpCommunicationSampleTests.Device;

[TestFixture]
internal class UdpStarterTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var udpStarter = new UdpStarter();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(udpStarter.BusinessTransactionId, Is.Zero);
            Assert.That(udpStarter.Snapshot, Is.False);
        }
    }

    [Test]
    public void ParseCommand_InvalidCommand_NoChanges()
    {
        // Arrange 
        var udpStarter = new UdpStarter();

        // Act  
        udpStarter.ParseCommand("Blubb");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(udpStarter.BusinessTransactionId, Is.Zero);
            Assert.That(udpStarter.Snapshot, Is.False);
        }
    }

    [Test]
    public void ParseCommand_SetSnapshot_NoChanges()
    {
        // Arrange 
        var udpStarter = new UdpStarter();

        // Act  
        udpStarter.ParseCommand("set,stream,mode,snapshot");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(udpStarter.BusinessTransactionId, Is.Zero);
            Assert.That(udpStarter.Snapshot, Is.True);
        }
    }

    [Test]
    public void ParseCommand_StartSnapshot_NoChanges()
    {
        // Arrange 
        var udpStarter = new UdpStarter();
        udpStarter.ParseCommand("set,stream,mode,snapshot");

        // Act  
        udpStarter.ParseCommand("set,status,start");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(udpStarter.BusinessTransactionId, Is.EqualTo(3));
            Assert.That(udpStarter.Snapshot, Is.True);
        }
    }

    [Test]
    public void ParseCommand_StartStreaming_NoChanges()
    {
        // Arrange 
        var udpStarter = new UdpStarter();
        udpStarter.ParseCommand("set,stream,mode,continious");

        // Act  
        udpStarter.ParseCommand("set,status,start");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(udpStarter.BusinessTransactionId, Is.EqualTo(1));
            Assert.That(udpStarter.Snapshot, Is.False);
        }
    }

    [Test]
    public void ParseCommand_Stop_NoChanges()
    {
        // Arrange 
        var udpStarter = new UdpStarter();

        // Act  
        udpStarter.ParseCommand("set,status,stop");

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(udpStarter.BusinessTransactionId, Is.EqualTo(2));
        }
    }
}