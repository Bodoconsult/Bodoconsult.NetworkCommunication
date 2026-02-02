// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for creating <see cref="IWaitStateManager"/> based implementations
/// </summary>
public interface IWaitStateManagerFactory
{

    /// <summary>
    /// Create a instance implementing <see cref="IWaitStateManager"/>
    /// </summary>
    /// <param name="config">Current config to use</param>
    /// <returns>New instance of <see cref="IWaitStateManager"/></returns>
    IWaitStateManager CreateInstance(IDataMessagingConfig config);

}