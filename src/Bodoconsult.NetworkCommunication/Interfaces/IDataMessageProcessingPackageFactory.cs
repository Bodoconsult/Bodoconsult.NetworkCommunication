
// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

public interface IDataMessageProcessingPackageFactory
{

    /// <summary>
    /// Create a instance impl <see cref="IDataMessageProcessingPackage"/>
    /// </summary>
    /// <param name="config">Current config to use</param>
    /// <returns>New instance of <see cref="IDataMessageProcessingPackage"/></returns>
    IDataMessageProcessingPackage CreateInstance(IDataMessagingConfig config);

}