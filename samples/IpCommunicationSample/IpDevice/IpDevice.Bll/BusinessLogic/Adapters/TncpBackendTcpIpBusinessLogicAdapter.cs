// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
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
    private readonly Lock _udpStarterLock = new();

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

        Trace.TraceInformation($"TncpBackendTcpIpBusinessLogicAdapter: received command >>{tncp.TelnetCommand}<< with message {message.MessageId}");

        NetworkCommand? command;
        HandleTncpMessage? del;

        if (tncp.TelnetCommand == "set,status,stop")
        {
            ExexcuteCommand(tncp.TelnetCommand, 2);
            return;
        }

        if (tncp.TelnetCommand.StartsWith("set,stream,number", StringComparison.InvariantCultureIgnoreCase))
        {
            lock (_udpStarterLock)
            {
                _udpStarter = new UdpStarter();
            }
            return;
        }

        ArgumentNullException.ThrowIfNull(_udpStarter);

        _udpStarter.ParseCommand(tncp.TelnetCommand);

        if (!ExexcuteCommand(tncp.TelnetCommand, _udpStarter.BusinessTransactionId))
        {
            return;
        }

        lock (_udpStarterLock)
        {
            _udpStarter = null;
        }
    }

    private bool ExexcuteCommand(string cmd, int businessTransactionId)
    {
        var del = _commands.GetValueOrDefault(businessTransactionId);

        if (del == null)
        {
            return false;
        }

        var command = _parser.Parse(cmd);
        del.Invoke(command);

        return true;
    }

    //private void HandleStopSnapshotRequest(NetworkCommand command)
    //{
    //    var request = new EmptyBusinessTransactionRequestData
    //    {
    //        TransactionId = IpDeviceBusinessTransactionCodes.StopSnapshot
    //    };

    //    _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
    //}

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