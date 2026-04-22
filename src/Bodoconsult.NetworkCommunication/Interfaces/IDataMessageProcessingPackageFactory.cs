// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for creating <see cref="IDataMessageProcessingPackage"/> instances
/// </summary>
public interface IDataMessageProcessingPackageFactory
{
    /// <summary>
    /// Create an instance implementing <see cref="IDataMessageProcessingPackage"/>
    /// </summary>
    /// <param name="config">Current config to use</param>
    /// <returns>New instance of <see cref="IDataMessageProcessingPackage"/></returns>
    IDataMessageProcessingPackage CreateInstance(IDataMessagingConfig config);
}