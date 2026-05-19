// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.Tests.Helpers;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataMessageProcessingPackages;

[TestFixture]
internal class SfxpLoggedSortableDataMessageProcessingPackageFactoryTests
{
    [Test]
    public void CreateInstance_ValidSetup_ReturnsPackage()
    {
        // Arrange 
        var factory = new SfxpLoggedSortableDataMessageProcessingPackageFactory();

        var config = TestDataHelper.GetSfxpSortableLoggerDataMessagingConfig();
        
        // Act  
        factory.CreateInstance(config);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(config.DataMessageProcessingPackage?.GetType(), Is.EqualTo(typeof(SfxpLoggedSortableDataMessageProcessingPackage)));
            Assert.That(config.DataMessageProcessingPackage?.DataLoggers.Count, Is.EqualTo(1));
        }
    }
}