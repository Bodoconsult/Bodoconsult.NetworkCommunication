// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Text;
using Bodoconsult.App;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions.NetworkCommands;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;
using IpCommunicationSample.Common;
using IpCommunicationSample.Device.Bll.BusinessTransactions;
using IpCommunicationSample.Device.Bll.Interfaces;

namespace IpCommunicationSample.Device.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for TCP/IP control channel from backend to IP device
/// </summary>
public class TncpBackendTcpIpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IBackendTcpIpBusinessLogicAdapter
{
    private readonly TncpCommandParser _parser = new();
    private readonly IBusinessTransactionManager _businessTransactionManager;
    private delegate void HandleTncpMessage(NetworkCommand command);
    private readonly Dictionary<string, HandleTncpMessage> _commands = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public TncpBackendTcpIpBusinessLogicAdapter(IIpDevice device, IBusinessTransactionManager businessTransactionManager) : base(device)
    {
        _businessTransactionManager = businessTransactionManager;

        _commands.Add(TncpCommands.GetConfig.ToLowerInvariant(), HandleGetConfigRequest);
        _commands.Add(TncpCommands.StartStreaming.ToLowerInvariant(), HandleStartStreamingRequest);
        _commands.Add(TncpCommands.StopStreaming.ToLowerInvariant(), HandleStopStreamingRequest);
        _commands.Add(TncpCommands.StartSnapshot.ToLowerInvariant(), HandleStartSnapshotRequest);
        _commands.Add(TncpCommands.StopSnapshot.ToLowerInvariant(), HandleStopSnapshotRequest);
    }

    /// <summary>
    /// Default method to handle a received message from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public override void DefaultReceiveMessage(IInboundDataMessage message)
    {
        if (message is not TncpInboundDataMessage tncp)
        {
            return;
        }

        if (string.IsNullOrEmpty(tncp.TelnetCommand))
        {
            return;
        }

        var command = _parser.Parse(tncp.TelnetCommand);

        var del = _commands.GetValueOrDefault(command.Command.ToLowerInvariant());

        if (del == null)
        {
            return;
        }

        del.Invoke(command);
    }

    private void HandleStopSnapshotRequest(NetworkCommand command)
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = BusinessTransactionCodes.StopSnapshot
        };

        _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
    }

    private void HandleStartSnapshotRequest(NetworkCommand command)
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = BusinessTransactionCodes.StartSnapshot
        };

        _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
    }

    private void HandleStopStreamingRequest(NetworkCommand command)
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = BusinessTransactionCodes.StopStreaming
        };

        _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
    }

    private void HandleStartStreamingRequest(NetworkCommand command)
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = BusinessTransactionCodes.StartStreaming
        };

        _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
    }

    private void HandleGetConfigRequest(NetworkCommand command)
    {
        // ToDo: adjust config data
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);

        var message = new TncpOutboundDataMessage
        {
            TelnetCommand = $"{TncpCommands.GetConfig},configData"
        };

        IpDevice.CommunicationAdapter.SendDataMessage(message);
    }
}