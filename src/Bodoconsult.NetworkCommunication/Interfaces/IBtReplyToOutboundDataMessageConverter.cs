// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for converters from <see cref="IBusinessTransactionReply"/> instances to <see cref="IOutboundDataMessage"/> instances
/// </summary>
public interface IBtReplyToOutboundDataMessageConverter
{
    /// <summary>
    /// Current app logger
    /// </summary>
    IAppLoggerProxy AppLogger { get; }

    /// <summary>
    /// Map an internal business transaction reply to a data messaging request
    /// </summary>
    /// <param name="request">Current request</param>
    /// <returns>Internal business transaction request</returns>
    IOutboundDataMessage? MapToOutboundDataMessage(IBusinessTransactionReply request);
}