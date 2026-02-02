// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication.Sending;

/// <summary>
/// Implementing <see cref="IWaitStateManager"/> for firmware versions starting with 100
/// </summary>
public class DefaultWaitStateManager : BaseWaitStateManager
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="config">Current config to use</param>
    public DefaultWaitStateManager(IDataMessagingConfig config) : base(config)
    {
        Timeout = DeviceCommunicationBasics.WaitForAckTimeout;
    }
}