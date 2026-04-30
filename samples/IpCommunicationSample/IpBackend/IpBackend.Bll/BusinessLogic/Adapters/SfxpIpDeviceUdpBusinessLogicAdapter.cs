// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions.Replies;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using System.Diagnostics;

namespace IpBackend.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for UPD channel from backend to IP device
/// </summary>
public class SfxpIpDeviceUdpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IIpDeviceUdpDeviceBusinessLogicAdapter
{
    private long _messageCounter;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public SfxpIpDeviceUdpBusinessLogicAdapter(IIpDevice device) : base(device)
    { }

    /// <summary>
    /// Default method to handle a received message from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public override void DefaultReceiveMessage(IInboundDataMessage message)
    {
        _messageCounter++;

        if (Math.Abs(_messageCounter % 1000.0) < 0.1)
        {
            var msg = $"Received message {_messageCounter} with {message.RawMessageData.Length} bytes";
            //Debug.Print(msg);
            IpDevice.DataMessagingConfig.AppLogger.LogInformation(msg);
            Trace.TraceInformation($"SfxpIpDeviceUdpBusinessLogicAdapter: {msg}");
        }

        if (_messageCounter == long.MaxValue)
        {
            _messageCounter = 0;
        }

        // ToDo: add your business logic
    }

    /// <summary>
    /// Send the required client hello to the server
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply CheckConnection(IBusinessTransactionRequestData requestData)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);

        if (IpDevice.CommunicationAdapter.IsConnected)
        {
            if (IpDevice.CommunicationAdapter.CommunicationHandler == null)
            {
                IpDevice.CommunicationAdapter.ComDevInit();
            }
            else
            {
                IpDevice.CommunicationAdapter.CommunicationHandler.Disconnect();
                IpDevice.CommunicationAdapter.CommunicationHandler.Connect();

                Trace.TraceInformation("SfxpIpDeviceUdpBusinessLogicAdapter: Connection reset");
            }

            return new DefaultBusinessTransactionReply();
        }

        if (IpDevice.CommunicationAdapter.ComDevInit())
        {
            return new DefaultBusinessTransactionReply();
        }

        return new DefaultBusinessTransactionReply
        {
            ErrorCode = 1,
            Message = "No connection"
        };
    }

    /// <summary>
    /// Send the required client hello to the server
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply SendClientHello(IBusinessTransactionRequestData requestData)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);

        var msg = new RawOutboundDataMessage
        {
            RawMessageData = new Memory<byte>([0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x66, 0x72, 0x6f, 0x6d, 0x20, 0x63, 0x6c, 0x69, 0x65, 0x6e, 0x74])
        };

        var result = IpDevice.CommunicationAdapter.SendDataMessage(msg);

        if (result.ProcessExecutionResult.Id == OrderExecutionResultState.Successful.Id)
        {
            return new DefaultBusinessTransactionReply();
        }

        return new DefaultBusinessTransactionListReply
        {
            ErrorCode = result.ProcessExecutionResult.Id,
            Message = "Sending HELLO failed"
        };
    }

    public IBusinessTransactionReply CreateFftAnalysisReport(IBusinessTransactionRequestData requestData)
    {
        if (requestData is not FftReportBusinessTransactionRequestData fft)
        {
            throw new ArgumentException($"requestData is not {nameof(FftReportBusinessTransactionRequestData)}");
        }

        // ToDo: collect data here

        return new FftReportBusinessTransactionReply();
    }
}