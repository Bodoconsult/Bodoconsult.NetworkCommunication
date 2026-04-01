// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.App.Abstractions.DataExportServices;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.Tests.DataMessaging.DataLoggers;

[TestFixture]
internal class RawInboundDataLoggerTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        // Act  
        var logger = new RawInboundDataLogger(dataExportService);

        // Assert
        Assert.That(logger.DataExportService, Is.EqualTo(dataExportService));
    }

    [Test]
    public void CheckIfMessageIsToLog_ValidMessage_ReturnsTrue()
    {
        // Arrange 
        IDataExportService<byte[]> dataExportService = new FakeDataExportService();

        var logger = new RawInboundDataLogger(dataExportService);

        var msg = new SdcpSortableInboundDataMessage();

        // Act  
        var result = logger.CheckIfMessageIsToLog(msg);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void LogTheMessage_ValidMessage_DataLogged()
    {
        // Arrange 
        var dataExportService = new FakeDataExportService();

        var logger = new RawInboundDataLogger(dataExportService);

        var msg = new SdcpSortableInboundDataMessage();

        var result = logger.CheckIfMessageIsToLog(msg);

        Assert.That(result, Is.True);

        // Act  
        logger.LogTheMessage(msg);

        // Assert
        Wait.Until(() => dataExportService.WasLogged);
        Assert.That(dataExportService.WasLogged, Is.True);
    }
}