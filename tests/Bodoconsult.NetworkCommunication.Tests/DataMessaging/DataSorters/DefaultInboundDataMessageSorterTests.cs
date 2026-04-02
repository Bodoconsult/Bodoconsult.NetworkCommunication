// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataSorter;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataSorters;

[TestFixture]
internal class DefaultInboundDataMessageSorterTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var sorter = new DefaultInboundDataMessageSorter();

        // Assert
        Assert.That(sorter.LastMessageId, Is.EqualTo(long.MinValue));
    }

    [Test]
    public void AddMessage_MessageWithId0_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter();

        var msg = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 0
        };

        // Act  
        var result = sorter.AddMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg.OriginalMessageId));
            Assert.That(result.Contains(msg));
        }
    }

    [Test]
    public void AddMessage_MessageWithId1_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter();

        var msg = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 1
        };

        // Act  
        var result = sorter.AddMessage(msg);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg.OriginalMessageId));
            Assert.That(result.Contains(msg));
        }
    }

    [Test]
    public void AddMessage_Message1Id0Message2Id1_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter();

        var msg1 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 0
        };

        var result1 = sorter.AddMessage(msg1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg1.OriginalMessageId));
            Assert.That(result1.Contains(msg1));
        }

        var msg2 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 1
        };

        // Act  
        var result2 = sorter.AddMessage(msg2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg2.OriginalMessageId));
            Assert.That(result2.Contains(msg2));
        }
    }


    [Test]
    public void AddMessage_Message1Id2Message2Id1_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter();

        var msg0 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 0
        };

        var result0 = sorter.AddMessage(msg0);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg0.OriginalMessageId));
            Assert.That(result0.Contains(msg0), Is.True);
        }

        var msg1 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 2
        };

        var result1 = sorter.AddMessage(msg1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.Zero);
            Assert.That(result1.Contains(msg1), Is.False);
        }

        var msg2 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 1
        };

        // Act  
        var result2 = sorter.AddMessage(msg2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg1.OriginalMessageId));
            Assert.That(result2.Contains(msg1));
            Assert.That(result2.Contains(msg2));
        }
    }

    [Test]
    public void AddMessage_Message1Id3Message2Id2Message3Id1_ReturnsCorrectMessages()
    {
        // Arrange 
        var sorter = new DefaultInboundDataMessageSorter();

        var msg0 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 0
        };

        var result0 = sorter.AddMessage(msg0);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg0.OriginalMessageId));
            Assert.That(result0.Contains(msg0), Is.True);
        }

        var msg1 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 3
        };

        var result1 = sorter.AddMessage(msg1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.Zero);
            Assert.That(result1.Contains(msg1), Is.False);
        }

        var msg2 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 2
        };

        // Act  
        var result2 = sorter.AddMessage(msg2);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.Zero);
            Assert.That(result2.Contains(msg2), Is.False);
        }

        var msg3 = new SdcpSortableInboundDataMessage
        {
            OriginalMessageId = 1
        };

        // Act  
        var result3 = sorter.AddMessage(msg3);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(sorter.LastMessageId, Is.EqualTo(msg1.OriginalMessageId));
            Assert.That(result3.Contains(msg1));
            Assert.That(result3.Contains(msg2));
            Assert.That(result3.Contains(msg3));
        }
    }
}