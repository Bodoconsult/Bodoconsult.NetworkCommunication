// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.NetworkCommands;
using IpCommunicationSample.Common;
using IpDevice.Bll.BusinessTransactions;
using IpDevice.Bll.Interfaces;

namespace IpDevice.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for TCP/IP control channel from backend to IP device
/// </summary>
public class TncpBackendTcpIpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IBackendTcpIpBusinessLogicAdapter
{
    private readonly TncpCommandParser _parser = new();
    private readonly IBusinessTransactionManager _businessTransactionManager;
    private delegate void HandleTncpMessage(NetworkCommand command);
    private readonly Dictionary<int, HandleTncpMessage> _commands = new();

    private UdpStarter? _udpStarter;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public TncpBackendTcpIpBusinessLogicAdapter(IIpDevice device, IBusinessTransactionManager businessTransactionManager) : base(device)
    {
        _businessTransactionManager = businessTransactionManager;

        _commands.Add(5, HandleGetConfigRequest);
        _commands.Add(1, HandleStartStreamingRequest);
        _commands.Add(2, HandleStopRequest);
        _commands.Add(3, HandleStartSnapshotRequest);
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

        if (tncp.TelnetCommand.StartsWith("set,stream,number", StringComparison.InvariantCultureIgnoreCase))
        {
            _udpStarter = new UdpStarter();
            return;
        }

        ArgumentNullException.ThrowIfNull(_udpStarter);

        _udpStarter.ParseCommand(tncp.TelnetCommand);

        var command = _parser.Parse(tncp.TelnetCommand);


        var del = _commands.GetValueOrDefault(_udpStarter.BusinessTransactionId);

        if (del == null)
        {
            return;
        }

        _udpStarter = null;

        del.Invoke(command);
    }

    private void HandleStopSnapshotRequest(NetworkCommand command)
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = IpDeviceBusinessTransactionCodes.StopSnapshot
        };

        _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
    }

    private void HandleStartSnapshotRequest(NetworkCommand command)
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = IpDeviceBusinessTransactionCodes.StartSnapshot
        };

        _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
    }

    private void HandleStopRequest(NetworkCommand command)
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = IpDeviceBusinessTransactionCodes.StopStreaming
        };

        _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
    }

    private void HandleStartStreamingRequest(NetworkCommand command)
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = IpDeviceBusinessTransactionCodes.StartStreaming
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