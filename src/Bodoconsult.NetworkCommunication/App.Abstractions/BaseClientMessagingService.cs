// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.App.Abstractions.Interfaces;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Base class for<see cref="IClientMessagingService"/> instances converting internal notifications to transport layer messages
/// </summary>
public abstract class BaseClientMessagingService : IClientMessagingService
{
    /// <summary>
    /// All conversion rules for notifications event args to client transport level target object
    /// </summary>
    public Dictionary<string, NotificationToTargetTransferObjectDelegate> ConversionRules { get; } = new();

    /// <summary>
    /// Convert a notification into a client transport level target object
    /// </summary>
    /// <param name="notification">Current notification to send</param>
    /// <returns>Object to transfer to the client on transport level</returns>
    public object? Convert(IClientNotification notification)
    {
        var notiType = notification.GetType().Name;

        var success = ConversionRules.TryGetValue(notiType, out var del);

        if (success && del != null)
        {
            return del(notification);
        }

        return null;
    }
}