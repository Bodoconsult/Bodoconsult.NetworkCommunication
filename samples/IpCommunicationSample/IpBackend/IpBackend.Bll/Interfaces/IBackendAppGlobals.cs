// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

namespace IpBackend.Bll.Interfaces;

/// <summary>
/// Enhancements of <see cref="IAppGlobals"/> for backend
/// </summary>
public interface IBackendAppGlobals : IAppGlobals
{
    /// <summary>
    /// The maximum filesize of a data logging file in bytes
    /// </summary>
    public long MaxDataLoggingFileSize { get; set; } 
}