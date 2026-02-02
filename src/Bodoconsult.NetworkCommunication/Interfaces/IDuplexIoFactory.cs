// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories for <see cref="IDuplexIo"/> implementations
/// </summary>
public interface IDuplexIoFactory
{
    /// <summary>
    /// Creates an instance of <see cref="IDuplexIo"/>
    /// </summary>
    /// <param name="config">Current data messaging config</param>
    /// <returns>Instance of <see cref="IDuplexIo"/></returns>
    IDuplexIo CreateInstance(IDataMessagingConfig config);

}