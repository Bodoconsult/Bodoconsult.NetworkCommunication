// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Base interface for comm data pipelines
/// </summary>
public interface IPipeline : IDisposable
{
    /// <summary>
    /// Buffer size to use
    /// </summary>
    int BufferSize { get; set; }
}