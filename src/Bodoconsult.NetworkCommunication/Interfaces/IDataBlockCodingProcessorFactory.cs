// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for creating <see cref="IDataBlockCodingProcessor"/> instances
/// </summary>
public interface IDataBlockCodingProcessorFactory
{

    /// <summary>
    /// Get a instance implementing <see cref="IDataBlockCodingProcessor"/> 
    /// </summary>
    /// <param name="monitorLogger">Current monitor logger</param>
    public IDataBlockCodingProcessor CreateInstance(IAppLoggerProxy monitorLogger);
}