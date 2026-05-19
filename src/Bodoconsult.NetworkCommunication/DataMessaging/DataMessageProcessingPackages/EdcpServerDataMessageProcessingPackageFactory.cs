// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;

/// <summary>
/// Factory for <see cref="EdcpServerDataMessageProcessingPackage"/> instances
/// </summary>
public class EdcpServerDataMessageProcessingPackageFactory : IDataMessageProcessingPackageFactory
{
    /// <summary>
    /// Create an instance of <see cref="EdcpServerDataMessageProcessingPackage"/> impl <see cref="IDataMessageProcessingPackage"/>
    /// </summary>
    /// <param name="config">Current config to use</param>
    /// <returns>New instance of <see cref="IDataMessageProcessingPackage"/></returns>
    public void CreateInstance(IDataMessagingConfig config)
    {
        var package = new EdcpServerDataMessageProcessingPackage(config);
        package.DataLoggers.AddRange(config.DataLoggers);
    }
}