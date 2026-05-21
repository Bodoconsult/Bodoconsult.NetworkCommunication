// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpCommunicationSample.Common.BusinessTransactions.Requests;
using IpCommunicationSample.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.App.Abstractions.Extensions;

namespace IpCommunicationSampleTests.Extensions;

internal class BusinessTransactionRequestDataExtensionsTests
{

    [Test]
    public void GetBytes_Snapshot_ReturnsCorrectArray()
    {
        // Arrange 
        var request = new StartMessagingReportBusinessTransactionRequestData()
        {
            Snapshot = true,
            Channel1 = false,
            Channel2 = false,
            Channel3 = false,
            Channel4 = false,
        };

        // Act  
        var result = request.GetBytes();

        // Assert
        Assert.That(result.IsEqualTo([0x1, 0x0, 0x0, 0x0, 0x0]), Is.True);
    }

    [Test]
    public void GetBytes_Channel1_ReturnsCorrectArray()
    {
        // Arrange 
        var request = new StartMessagingReportBusinessTransactionRequestData()
        {
            Snapshot = false,
            Channel1 = true,
            Channel2 = false,
            Channel3 = false,
            Channel4 = false,
        };

        // Act  
        var result = request.GetBytes();

        // Assert
        Assert.That(result.IsEqualTo([0x0, 0x1, 0x0, 0x0, 0x0]), Is.True);
    }

    [Test]
    public void GetBytes_Channel2_ReturnsCorrectArray()
    {
        // Arrange 
        var request = new StartMessagingReportBusinessTransactionRequestData()
        {
            Snapshot = false,
            Channel1 = false,
            Channel2 = true,
            Channel3 = false,
            Channel4 = false,
        };

        // Act  
        var result = request.GetBytes();

        // Assert
        Assert.That(result.IsEqualTo([0x0, 0x0, 0x1, 0x0, 0x0]), Is.True);
    }

    [Test]
    public void GetBytes_Channel3_ReturnsCorrectArray()
    {
        // Arrange 
        var request = new StartMessagingReportBusinessTransactionRequestData()
        {
            Snapshot = false,
            Channel1 = false,
            Channel2 = false,
            Channel3 = true,
            Channel4 = false,
        };

        // Act  
        var result = request.GetBytes();

        // Assert
        Assert.That(result.IsEqualTo([0x0, 0x0, 0x0, 0x1, 0x0]), Is.True);
    }

    [Test]
    public void GetBytes_Channel4_ReturnsCorrectArray()
    {
        // Arrange 
        var request = new StartMessagingReportBusinessTransactionRequestData()
        {
            Snapshot = false,
            Channel1 = false,
            Channel2 = false,
            Channel3 = false,
            Channel4 = true,
        };

        // Act  
        var result = request.GetBytes();

        // Assert
        Assert.That(result.IsEqualTo([0x0, 0x0, 0x0, 0x0, 0x1]), Is.True);
    }

    [Test]
    public void ToStartMessagingReportBusinessTransactionRequestData_Snapshot_ReturnsCorrectRequest()
    {
        // Arrange 
        var data = new byte[] { 0x1, 0x0, 0x0, 0x0, 0x0 }.AsMemory();

        // Act  
        var result = data.ToStartMessagingReportBusinessTransactionRequestData();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Snapshot, Is.True);
            Assert.That(result.Channel1, Is.False);
            Assert.That(result.Channel2, Is.False);
            Assert.That(result.Channel3, Is.False);
            Assert.That(result.Channel4, Is.False);
        }
    }

    [Test]
    public void ToStartMessagingReportBusinessTransactionRequestData_Channel1_ReturnsCorrectRequest()
    {
        // Arrange 
        var data = new byte[] { 0x0, 0x1, 0x0, 0x0, 0x0 }.AsMemory();

        // Act  
        var result = data.ToStartMessagingReportBusinessTransactionRequestData();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Snapshot, Is.False);
            Assert.That(result.Channel1, Is.True);
            Assert.That(result.Channel2, Is.False);
            Assert.That(result.Channel3, Is.False);
            Assert.That(result.Channel4, Is.False);
        }
    }

    [Test]
    public void ToStartMessagingReportBusinessTransactionRequestData_Channel2_ReturnsCorrectRequest()
    {
        // Arrange 
        var data = new byte[] { 0x0, 0x0, 0x1, 0x0, 0x0 }.AsMemory();

        // Act  
        var result = data.ToStartMessagingReportBusinessTransactionRequestData();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Snapshot, Is.False);
            Assert.That(result.Channel1, Is.False);
            Assert.That(result.Channel2, Is.True);
            Assert.That(result.Channel3, Is.False);
            Assert.That(result.Channel4, Is.False);
        }
    }

    [Test]
    public void ToStartMessagingReportBusinessTransactionRequestData_Channel3_ReturnsCorrectRequest()
    {
        // Arrange 
        var data = new byte[] { 0x0, 0x0, 0x0, 0x1, 0x0 }.AsMemory();

        // Act  
        var result = data.ToStartMessagingReportBusinessTransactionRequestData();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Snapshot, Is.False);
            Assert.That(result.Channel1, Is.False);
            Assert.That(result.Channel2, Is.False);
            Assert.That(result.Channel3, Is.True);
            Assert.That(result.Channel4, Is.False);
        }
    }

    [Test]
    public void ToStartMessagingReportBusinessTransactionRequestData_Channel4_ReturnsCorrectRequest()
    {
        // Arrange 
        var data = new byte[] {0x0, 0x0, 0x0, 0x0, 0x1}.AsMemory();

        // Act  
        var result = data.ToStartMessagingReportBusinessTransactionRequestData();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Snapshot, Is.False);
            Assert.That(result.Channel1, Is.False);
            Assert.That(result.Channel2, Is.False);
            Assert.That(result.Channel3, Is.False);
            Assert.That(result.Channel4, Is.True);
        }
    }
}