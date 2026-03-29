// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

namespace Bodoconsult.NetworkCommunication.ClientNotifications;

/// <summary>
/// Base class for BTCP based notifications services to clients
/// </summary>
public abstract class BaseBtcpNetworkingClientMessagingService: BaseClientMessagingService
{
    /// <summary>
    /// Transaction ID to use for BTCP transaction message. Default 100
    /// </summary>
    public int TransactionId { get; set; } = 100;

    /// <summary>
    /// Default ctor loading <see cref="StateMachineStateNotification"/> notifications
    /// </summary>
    protected BaseBtcpNetworkingClientMessagingService()
    {
        ConversionRules.Add(nameof(StateMachineStateNotification), CreateMessageForStateMachineStateNotification);
    }

    private object CreateMessageForStateMachineStateNotification(IClientNotification notification)
    {
        if (notification is not StateMachineStateNotification rd)
        {
            throw new ArgumentException($"Request must be {nameof(StateMachineStateNotification)}");
        }

        var db = new BasicOutboundDatablock
        {
            Data = Encoding.UTF8.GetBytes($"{rd.DeviceStateId}\u0005{rd.DeviceStateName}\u0005{rd.BusinessStateId}\u0005{rd.BusinessStateName}\u0005{rd.BusinessSubstateId}\u0005{rd.BusinessSubstateName}")
        };

        var message = new BtcpRequestOutboundDataMessage(TransactionId, Guid.NewGuid())
        {
            DataBlock = db
        };

        return message;
    }
}