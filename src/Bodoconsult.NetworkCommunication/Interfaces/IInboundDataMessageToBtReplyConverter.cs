// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for converters from <see cref="IInboundDataMessage"/> instances to <see cref="IBusinessTransactionReply"/> instances
/// </summary>
public interface IInboundDataMessageToBtReplyConverter
{
    /// <summary>
    /// Current app logger
    /// </summary>
    IAppLoggerProxy AppLogger { get; }

    /// <summary>
    /// Map a data messaging reply to an internal business transaction reply
    /// </summary>
    /// <param name="reply">Current request</param>
    /// <returns>Internal business transaction request</returns>
    IBusinessTransactionReply? MapToBusinessTransactionReply(IInboundDataMessage reply);
}