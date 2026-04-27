// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Requests;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.EnumAndStates;
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
    }

    /// <summary>
    /// Delegate fired when then state of the backend has changed
    /// </summary>
    public StateChangedNotificationDelegate? StateChangedNotificationDelegate { get; }

    /// <summary>
    /// Request a start streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStartStreamingState(IBusinessTransactionRequestData request)
    {
        var transactionId = ClientSideBusinessTransactionIds.StartStreaming;
        const string orderName = "RequestDeviceStartStreamingState";

        return CreateAndExecuteOrder(transactionId, orderName);
    }

    /// <summary>
    /// Request a start snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStartSnapshotState(IBusinessTransactionRequestData request)
    {
        var transactionId = ClientSideBusinessTransactionIds.StartSnapshot;
        const string orderName = "RequestDeviceStartSnapshotState";

        return CreateAndExecuteOrder(transactionId, orderName);
    }

    /// <summary>
    /// Request a stop streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStopStreamingState(IBusinessTransactionRequestData request)
    {
        var transactionId = ClientSideBusinessTransactionIds.StopStreaming;
        const string orderName = "RequestDeviceStopStreamingState";

        return CreateAndExecuteOrder(transactionId, orderName);
    }

    /// <summary>
    /// Request a stop snapshot state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStopSnapshotState(IBusinessTransactionRequestData request)
    {
        var transactionId = ClientSideBusinessTransactionIds.StopSnapshot;
        const string orderName = "RequestDeviceStopSnapshotState";

        return CreateAndExecuteOrder(transactionId, orderName);
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

        var transactionId = ClientSideBusinessTransactionIds.StopSnapshot;
        var orderName = "CreateFftAnalysisReport";

        return CreateAndExecuteOrder(transactionId, orderName, HandleRequestAnswerDelegate);
    }

    private MessageHandlingResult HandleRequestAnswerDelegate(IInboundDataMessage? message, object? transportObject, IParameterSet? parameterSet)
    {
        // ToDo. RL: Use the FFT data
        throw new NotImplementedException();
    }

    private IBusinessTransactionReply CreateAndExecuteOrder(int transactionId, string orderName, HandleRequestAnswerDelegate? handleRequestAnswerDelegate = null)
    {
        ArgumentNullException.ThrowIfNull(Device.OrderManager);

        var ps = new BtcpParameterSet();
        ps.BusinessTransactionId = transactionId;

        var builder = new BtcpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration(orderName, BuiltinOrders.NoAnswerBtcpOrder, builder)
        {
            HandleRequestAnswerOnSuccessDelegate = handleRequestAnswerDelegate,
            ParameterSet = ps
        };

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

        var request = _dataBlockConverter.ConvertToRequest(message.DataBlock);

        if (request is StateChangedEventFiredBusinessTransactionRequestData sr)
        {
            StateChangedNotificationDelegate?.Invoke(sr);
        }

        return MessageHandlingResultHelper.Success();
    }
}