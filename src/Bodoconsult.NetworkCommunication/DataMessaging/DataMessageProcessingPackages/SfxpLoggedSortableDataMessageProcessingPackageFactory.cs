// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.DataExportServices;
using Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;

/// <summary>
/// Factory for <see cref="SfxpLoggedSortableDataMessageProcessingPackage"/> instances
/// </summary>
public class SfxpLoggedSortableDataMessageProcessingPackageFactory : IDataMessageProcessingPackageFactory
{
    /// <summary>
    /// Create an instance of <see cref="SfxpLoggedSortableDataMessageProcessingPackage"/> impl <see cref="IDataMessageProcessingPackage"/>
    /// </summary>
    /// <param name="config">Current config to use</param>
    /// <returns>New instance of <see cref="IDataMessageProcessingPackage"/></returns>
    public IDataMessageProcessingPackage CreateInstance(IDataMessagingConfig config)
    {
        var package = new SfxpLoggedSortableDataMessageProcessingPackage(config);

        var es = new ByteArrayDataExportService
        {
            FileName = string.IsNullOrEmpty(config.DataLoggingFileName) ? "DataLogging": config.DataLoggingFileName,
            TargetPath = string.IsNullOrEmpty(config.DataLoggingPath) ? Path.GetTempPath(): config.DataLoggingPath,
            CacheSize = 50,
            FileExtension = "bin"
        };

        var logger = new OnlyDataBlockInboundDataLogger(es);
        logger.Start();

        package.DataLoggers.Add(logger);

        return package;
    }
}