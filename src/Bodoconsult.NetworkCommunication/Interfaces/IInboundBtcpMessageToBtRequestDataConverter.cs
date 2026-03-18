// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for converters from <see cref="IInboundDataMessage"/> instances to <see cref="IBusinessTransactionRequestData"/> instances
/// </summary>
public interface IInboundBtcpMessageToBtRequestDataConverter
{
    /// <summary>
    /// Current app logger
    /// </summary>
    IAppLoggerProxy AppLogger { get; }

    /// <summary>
    /// Map a data messaging request to an internal business transaction request
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Internal business transaction request</returns>
    IBusinessTransactionRequestData? MapToBusinessTransactionRequestData(IInboundBusinessTransactionDataMessage request);
}