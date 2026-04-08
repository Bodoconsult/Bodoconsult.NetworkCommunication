// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;

/// <summary>
/// Factory for <see cref="SfxpDataMessageProcessingPackage"/> instances
/// </summary>
public class SfxpDataMessageProcessingPackageFactory : IDataMessageProcessingPackageFactory
{
    /// <summary>
    /// Create an instance of <see cref="SfxpDataMessageProcessingPackage"/> impl <see cref="IDataMessageProcessingPackage"/>
    /// </summary>
    /// <param name="config">Current config to use</param>
    /// <returns>New instance of <see cref="IDataMessageProcessingPackage"/></returns>
    public IDataMessageProcessingPackage CreateInstance(IDataMessagingConfig config)
    {
        return new SfxpDataMessageProcessingPackage(config);
    }
}