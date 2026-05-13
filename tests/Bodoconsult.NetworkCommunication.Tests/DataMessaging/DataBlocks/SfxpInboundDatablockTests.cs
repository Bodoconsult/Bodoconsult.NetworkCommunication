// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataBlocks;

[TestFixture]
internal class SfxpInboundDatablockTests
{

    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 

        // Act  
        var db = new SfxpInboundDatablock();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(db.Data.Length, Is.Zero);
            Assert.That(db.DataBlockType, Is.EqualTo('s'));
        }
    }
}