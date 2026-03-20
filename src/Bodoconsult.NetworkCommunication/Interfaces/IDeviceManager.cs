// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

public interface IDeviceManager
{
    /// <summary>
    /// Current device
    /// </summary>
    IIpDevice? IpDevice { get; }
}