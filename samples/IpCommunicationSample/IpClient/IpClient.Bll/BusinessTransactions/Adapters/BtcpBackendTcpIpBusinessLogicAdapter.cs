// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.StateManagement;
using IpClient.Bll.BusinessTransactions.Converters;
using IpClient.Bll.Delegates;
using IpClient.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using System.Diagnostics;
using Bodoconsult.App.Helpers;
using IpCommunicationSample.Common.Extensions;

namespace IpClient.Bll.BusinessTransactions.Adapters;

/// <summary>
/// Current adapter for TCP/IP channel from client to backend
/// </summary>
public class BtcpBackendTcpIpBusinessLogicAdapter : BaseOrderManagementDeviceBusinessLogicAdapter, IBackendTcpIpBusinessLogicAdapter
{
    private readonly IOrderIdGenerator _orderIdGenerator;
    //private readonly IUiStateHandler _uiStateHandler;
    private readonly DataBlockConverter _dataBlockConverter = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device supporting order management</param>
    /// <param name="orderIdGenerator">Current order ID generator</param>
    /// <param name="uiStateHandler">Current UI state handler</param>
    public BtcpBackendTcpIpBusinessLogicAdapter(IOrderManagementDevice device, IOrderIdGenerator orderIdGenerator, IUiStateHandler uiStateHandler) :
        base(device)
    {
        _orderIdGenerator = orderIdGenerator;
        //_uiStateHandler = uiStateHandler;
        StateChangedNotificationDelegate = uiStateHandler.StateChangedNotificationReceived;
        ReportDeviceErrorDelegate = uiStateHandler.ReportDeviceErrorReceived;
    }

    /// <summary>
    /// Delegate fired when then state of the backend has changed
    /// </summary>
    public StateChangedNotificationDelegate? StateChangedNotificationDelegate { get; }

    /// <summary>
    /// Delegate fired when the device reported an error to the backend
    /// </summary>
    public ReportDeviceErrorDelegate? ReportDeviceErrorDelegate { get; }

    /// <summary>
    /// Request a start messaging state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStartMessagingState(IBusinessTransactionRequestData request)
    {
        if (request is not StartMessagingBusinessTransactionRequestData startRequest)
        {
            throw new ArgumentException($"request is not {nameof(StartMessagingBusinessTransactionRequestData)}");
        }

        var transactionId = ClientSideBusinessTransactionIds.StartMessaging;
        const string orderName = "RequestDeviceStartMessaging";

        var data = startRequest.GetBytes();

        return CreateAndExecuteOrder(transactionId, orderName, data);
    }

    /// <summary>
    /// Request a stop streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStopMessagingState(IBusinessTransactionRequestData request)
    {
        var transactionId = ClientSideBusinessTransactionIds.StopMessaging;
        const string orderName = "RequestDeviceStopMessaging";

        return CreateAndExecuteOrder(transactionId, orderName, []);
    }

    /// <summary>
    /// Notification fired
    /// </summary>
    /// <param name="requestData">Current request data</param>
    /// <returns>Returns <see cref="DoNotSendBusinessTransactionReply"/></returns>
    public IBusinessTransactionReply NotificationFired(IBusinessTransactionRequestData requestData)
    {
        if (requestData is StateChangedEventFiredBusinessTransactionRequestData srd)
        {
            StateChangedNotificationDelegate?.Invoke(srd);
        }

        return new DoNotSendBusinessTransactionReply
        {
            RequestData = requestData
        };
    }

    /// <summary>
    /// Create an FFT analysis report
    /// </summary>
    /// <param name="requestData"></param>
    /// <returns></returns>
    public IBusinessTransactionReply CreateFftAnalysisReport(IBusinessTransactionRequestData requestData)
    {
        if (requestData is not FftReportBusinessTransactionRequestData fft)
        {
            throw new ArgumentException($"requestData is not not {nameof(FftReportBusinessTransactionRequestData)}");
        }

        ArgumentNullException.ThrowIfNull(Device.OrderManager);

        var transactionId = ClientSideBusinessTransactionIds.CreateFftAnalysisReport;
        var orderName = "CreateFftAnalysisReport";

        return CreateAndExecuteOrder(transactionId, orderName, [], HandleRequestAnswerDelegate);
    }

    private MessageHandlingResult HandleRequestAnswerDelegate(IInboundDataMessage? message, object? transportObject, IParameterSet? parameterSet)
    {
        // ToDo. RL: Use the FFT data
        throw new NotImplementedException();
    }

    private IBusinessTransactionReply CreateAndExecuteOrder(int transactionId, string orderName, byte[] data, HandleRequestAnswerDelegate? handleRequestAnswerDelegate = null)
    {
        ArgumentNullException.ThrowIfNull(Device.OrderManager);

        var ps = new BtcpParameterSet();
        ps.BusinessTransactionId = transactionId;
        
        var builder = new BtcpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration(orderName, BuiltinOrders.BtcpOrder, builder)
        {
            HandleRequestAnswerOnSuccessDelegate = handleRequestAnswerDelegate,
            ParameterSet = ps
        };

        // Add payload
        if (data.Length > 0)
        {
            ps.Payload = data;
        }

        var order = builder.CreateOrder(config, _orderIdGenerator.NextId());

        var result = Device.OrderManager.OrderProcessor.TryToExecuteOrderSync(order);

        if (result == OrderExecutionResultState.Successful)
        {
            return new DefaultBusinessTransactionReply();
        }

        return new DefaultBusinessTransactionReply
        {
            ErrorCode = 2000,
            Message = $"{orderName} was not successful",
            ExceptionMessage = $"Order exec result: {result.Id} {result.Name}"
        };
    }

    /// <summary>
    /// Default method to handle an async received message
    /// </summary>
    /// <param name="message">Received message</param>
    public override MessageHandlingResult DefaultHandleAsyncMessage(IInboundDataMessage? message)
    {
        if (message?.DataBlock == null)
        {
            return MessageHandlingResultHelper.Error("No message or no datablock in message received");
        }

        var msg = $"{message.ToShortInfoString()}: {ArrayHelper.GetStringFromArrayCsharpStyle(message.DataBlock.Data, false)}";
        //Debug.Print(msg);
        Device.DataMessagingConfig.MonitorLogger.LogInformation(msg);

        var request = _dataBlockConverter.ConvertToRequest(message.DataBlock);

        if (request is FftReportBusinessTransactionRequestData fft)
        {
            //ReportDeviceErrorDelegate?.Invoke(err);
            return MessageHandlingResultHelper.Success();
        }

        if (request is StateChangedEventFiredBusinessTransactionRequestData sr)
        {
            StateChangedNotificationDelegate?.Invoke(sr);
            return MessageHandlingResultHelper.Success();
        }

        if (request is ErrorBusinessTransactionRequestData err)
        {
            ReportDeviceErrorDelegate?.Invoke(err);
        }
        
        return MessageHandlingResultHelper.Success();
    }
}