// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.NetworkCommands;
using IpCommunicationSample.Common;
using IpDevice.Bll.BusinessTransactions;
using IpDevice.Bll.Interfaces;
using System.Text;

namespace IpDevice.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for TCP/IP control channel from backend to IP device
/// </summary>
public class TncpBackendTcpIpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IBackendTcpIpBusinessLogicAdapter
{
    private byte[] _streamingConfig = [0x0, 0x1, 0x2, 0x3, 0xC, 0xF];
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

        if (tncp.TelnetCommand == "set,status,stop")
        {
            Debug.Print(tncp.TelnetCommand);
        }

        // Send an answer message
        SendAnswer(tncp.TelnetCommand);

        IpDevice.DataMessagingConfig.MonitorLogger.LogInformation($"TncpBackendTcpIpBusinessLogicAdapter: received command >>{tncp.TelnetCommand}<< with message {message.MessageId}");

        //NetworkCommand? command;
        //HandleTncpMessage? del;

        if (tncp.TelnetCommand.StartsWith("set,stream,order", StringComparison.InvariantCultureIgnoreCase))
        {
            var config = tncp.TelnetCommand[17..];
            _streamingConfig = GetConfig(config);
        }


        if (tncp.TelnetCommand == "set,status,start" && _udpStarter == null)
        {
            return;
        }

        if (tncp.TelnetCommand == "set,status,stop")
        {
            ExexcuteCommand(tncp.TelnetCommand, 2);
            return;
        }

        // Get the current streaming config
        if (tncp.TelnetCommand.StartsWith("set,stream,order", StringComparison.InvariantCultureIgnoreCase))
        {
            lock (_udpStarterLock)
            {
                _udpStarter ??= new UdpStarter();
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

    private void SendAnswer(string telnetCommand)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);


        Debug.Print($"Reply for {telnetCommand}");
        var cmd = CreateTncpReply(telnetCommand);

        var msg = new TncpOutboundDataMessage
        {
            TelnetCommand = cmd.ToString(),
            WaitForAcknowledgement = false
        };

        IpDevice.CommunicationAdapter.SendDataMessage(msg).GetAwaiter().GetResult();
    }

    public StringBuilder CreateTncpReply(string telnetCommand)
    {
        var cmd = new StringBuilder();
        cmd.Append($"<BEGIN>{telnetCommand}\n");

        if (telnetCommand.StartsWith("show,streamconfig", StringComparison.InvariantCultureIgnoreCase))
        {
            cmd.Append("<CONFIG>");

            foreach (var b in _streamingConfig)
            {
                cmd.Append($"0x{b:X}");
            }

            cmd.Append('\n');
        }

        cmd.Append("<END>");
        return cmd;
    }

    /// <summary>
    /// Get the streaming config from a string
    /// </summary>
    /// <param name="config">Config string with comma separated</param>
    /// <returns>Config byte array</returns>
    public static byte[] GetConfig(string config)
    {
        var s = config.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var data = new List<byte>();

        foreach (var s2 in s)
        {
            switch (s2)
            {
                case "1":
                    data.Add(0);
                    break;
                case "2":
                    data.Add(1);
                    break;
                case "3":
                    data.Add(2);
                    break;
                case "4":
                    data.Add(3);
                    break;
            }
        }

        data.Add(0xC);
        data.Add(0xF);

        return data.ToArray();
    }

    private bool ExexcuteCommand(string cmd, int businessTransactionId)
    {
        var del = _commands.GetValueOrDefault(businessTransactionId);

        if (del == null)
        {
            return false;
        }

        var waiter = new AutoResetEvent(false);

        AsyncHelper.FireAndForget(() =>
        {
            try
            {
                var x = del;
                var command = _parser.Parse(cmd);

                waiter.Set();

                x.Invoke(command);
            }
            catch (Exception e)
            {
                IpDevice.DataMessagingConfig.MonitorLogger.LogError($"TncpBackendTcpIpBusinessLogicAdapter: {cmd}: {e}");
            }
        });
        waiter.WaitOne(1000);
        return true;

        //var del = _commands.GetValueOrDefault(businessTransactionId);

        //if (del == null)
        //{
        //    return false;
        //}

        //var command = _parser.Parse(cmd);

        ////del.BeginInvoke(command, null, null);

        //del.Invoke(command);

        ////AsyncHelper.FireAndForget(() =>
        ////{
        ////    try
        ////    {
        ////        var x = del;
        ////        var y = command;
        ////        x.Invoke(y);
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        IpDevice.DataMessagingConfig.MonitorLogger.LogError($"TncpBackendTcpIpBusinessLogicAdapter: {command}: {e}");
        ////    }
        ////});
        ////Task.Delay(50);
        //return true;
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