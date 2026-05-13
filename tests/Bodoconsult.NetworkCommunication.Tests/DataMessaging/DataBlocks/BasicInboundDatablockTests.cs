// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataBlocks;

[TestFixture]
internal class BasicInboundDatablockTests
{

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var db = new BasicInboundDatablock();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(db.Data.Length, Is.Zero);
            Assert.That(db.DataBlockType, Is.EqualTo('x'));
        }
    }
}