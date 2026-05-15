// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

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
        package.DataLoggers.AddRange(config.DataLoggers);
        return package;
    }
}