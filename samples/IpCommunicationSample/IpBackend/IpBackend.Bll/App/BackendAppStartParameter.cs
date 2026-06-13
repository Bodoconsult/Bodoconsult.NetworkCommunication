// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App;

namespace IpBackend.Bll.App;

/// <summary>
/// App start parameter used for IpBackend service
/// </summary>
public class BackendAppStartParameter: ThreeNetworkDevicesAppStartParameter
{
    /// <summary>
    /// The maximum filesize of a data logging file in bytes
    /// </summary>
    public long MaxDataLoggingFileSize { get; set; } = 1000000000;
}