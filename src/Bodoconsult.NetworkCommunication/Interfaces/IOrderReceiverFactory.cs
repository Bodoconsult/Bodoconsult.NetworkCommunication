// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for order receiver implementations
/// </summary>
public interface IOrderReceiverFactory
{

    /// <summary>
    /// Create a device order receiver instance
    /// </summary>
    /// <param name="logger">Current monitor logger</param>
    /// <returns>A device order receiver instance</returns>
    IOrderReceiver CreateInstance(IAppLoggerProxy logger);

}