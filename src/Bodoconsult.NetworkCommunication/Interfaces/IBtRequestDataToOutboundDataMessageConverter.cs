// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for converters from <see cref="IBusinessTransactionRequestData"/> instances to <see cref="IOutboundDataMessage"/> instances
/// </summary>
public interface IBtRequestDataToOutboundDataMessageConverter
{
    /// <summary>
    /// Current app logger
    /// </summary>
    IAppLoggerProxy AppLogger { get; }

    /// <summary>
    /// Map  an internal business transaction request to a data messaging request
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Internal business transaction request</returns>
    IOutboundDataMessage? MapToOutboundDataMessage(IBusinessTransactionRequestData request);
}

