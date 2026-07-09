// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.StateManagement;
using IpBackend.Bll.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpBackend.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current implementation of <see cref="IStateMachineDeviceBusinessLogicAdapter"/> for the backend
/// </summary>
public class TncpIpDeviceTcpIpBusinessLogicAdapter : BaseStateMachineDeviceBusinessLogicAdapter, IIpDeviceTcpIpDeviceBusinessLogicAdapter
{
    private readonly IBusinessTransactionManager _businessTransactionManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public TncpIpDeviceTcpIpBusinessLogicAdapter(IStateMachineDevice device, IBusinessTransactionManager businessTransactionManager) : base(device)
    {
        _businessTransactionManager = businessTransactionManager;
    }

    /// <summary>
    /// Current UDP port to use
    /// </summary>
    public int UdpPort { get; set; }

    #region Device order handling

    /// <summary>
    /// Stopping snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopSnapshotUnsuccessfully(IStateMachineState state, IOrder order)
    {
        Device.CheckConnection();
    }

    /// <summary>
    /// Starting snapshot was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartSnapshotUnsuccessfully(IStateMachineState state, IOrder order)
    {
        Device.CheckConnection();
    }

    /// <summary>
    /// Starting streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StartMessagingUnsuccessfully(IStateMachineState state, IOrder order)
    {
        Device.CheckConnection();
    }

    /// <summary>
    /// Stopping streaming was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void StopMessagingUnsuccessfully(IStateMachineState state, IOrder order)
    {
        Device.CheckConnection();
    }

    /// <summary>
    /// Init device was successful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void DeviceInitSuccessfully(IStateMachineState state, IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Init device was unsuccessful
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="order">Current order</param>
    public void DeviceInitUnsuccessfully(IStateMachineState state, IOrder order)
    {
        Device.CheckConnection();
    }

    #endregion

    #region State management

    /// <summary>
    /// Request a start streaming state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStartMessagingState(IBusinessTransactionRequestData request)
    {
        try
        {
            if (request is not StartMessagingBusinessTransactionRequestData startRequest)
            {
                throw new ArgumentException($"request is not {nameof(StartMessagingBusinessTransactionRequestData)}");
            }

            //return new DefaultBusinessTransactionReply
            //{
            //    RequestData = request
            //};

            if ((Device.CurrentState?.Id ?? 0) != DefaultStateIds.DeviceReadyState)
            {
                return new DefaultBusinessTransactionReply
                {
                    RequestData = request,
                    ErrorCode = 1001,
                    Message = $"Start of the transaction is not allowed as the state is not {DefaultStateNames.DeviceReadyState}"
                };
            }

            if (startRequest.IsDataLoggingActivated)
            {
                StartLogging();
            }

            if (startRequest.IsChartActivated)
            {
                StartCollector(startRequest.CollectionInterval, startRequest.CollectionTime);
            }

            ArgumentNullException.ThrowIfNull(Device);
            ArgumentNullException.ThrowIfNull(StateFactory, "StateFactory is null. Call LoadStateFactory() before!");
            ArgumentNullException.ThrowIfNull(OrderFactory);

            const string stateName = DefaultStateNames.DeviceStartStreamingState;

            // Get the state config
            var jobConfig = GetConfiguration(StateFactory, stateName);

            // Get the required parametersets now
            for (var index = 0; index < jobConfig.OrderConfigurations.Count; index++)
            {
                CreateStartOrders(jobConfig, index, OrderFactory, startRequest);
            }

            // Now create the state
            CreateAndRegisterState(Device, StateFactory, jobConfig);

            return new DefaultBusinessTransactionReply
            {
                RequestData = request
            };
        }
        catch (Exception e)
        {
            return new DefaultBusinessTransactionReply
            {
                RequestData = request,
                ErrorCode = 1000,
                Message = "Starting streaming failed",
                ExceptionMessage = e.ToString()
            };
        }
    }


    private void StartCollector(int collectionInterval, int collectionTime)
    {
        var request = new StartCollectorBusinessTransactionRequestData
        {
            TransactionId = ServerSideBusinessTransactionIds.StartDataCollector,
            CollectionInterval = collectionInterval,
            CollectionTime = collectionTime
        };
        _businessTransactionManager.RunBusinessTransaction(ServerSideBusinessTransactionIds.StartDataCollector, request);
    }

    private void StopCollector()
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = ServerSideBusinessTransactionIds.StopDataCollector
        };
        _businessTransactionManager.RunBusinessTransaction(ServerSideBusinessTransactionIds.StopDataCollector, request);
    }

    private void StartLogging()
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = ServerSideBusinessTransactionIds.StartDataLogging
        };
        _businessTransactionManager.RunBusinessTransaction(ServerSideBusinessTransactionIds.StartDataLogging, request);
    }

    private void StopLogging()
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = ServerSideBusinessTransactionIds.StopDataLogging
        };
        _businessTransactionManager.RunBusinessTransaction(ServerSideBusinessTransactionIds.StopDataLogging, request);
    }

    ///// <summary>
    ///// Check the UDP connection now
    ///// </summary>
    //private void CheckUdpConnection()
    //{
    //    AsyncHelper.FireAndForget(() =>
    //    {
    //        try
    //        {
    //            var request = new EmptyBusinessTransactionRequestData();
    //            _businessTransactionManager.RunBusinessTransaction(ServerSideBusinessTransactionIds.CheckConnection, request);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e);
    //            throw;
    //        }

    //    });
    //}

    /// <summary>
    /// Request a start streamin state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStartSnapshotState(IBusinessTransactionRequestData request)
    {
        try
        {
            if (request is not StartMessagingBusinessTransactionRequestData startRequest)
            {
                throw new ArgumentException($"request is not {nameof(StartMessagingBusinessTransactionRequestData)}");
            }

            if ((Device.CurrentState?.Id ?? 0) != DefaultStateIds.DeviceReadyState)
            {
                return new DefaultBusinessTransactionReply
                {
                    RequestData = request,
                    ErrorCode = 1001,
                    Message = $"Start of the transaction is not allowed as the state is not {DefaultStateNames.DeviceReadyState}"
                };
            }

            ArgumentNullException.ThrowIfNull(Device);
            ArgumentNullException.ThrowIfNull(StateFactory, "StateFactory is null. Call LoadStateFactory() before!");
            ArgumentNullException.ThrowIfNull(OrderFactory);

            if (startRequest.IsDataLoggingActivated)
            {
                StartLogging();
            }

            if (startRequest.IsChartActivated)
            {
                StartCollector(startRequest.CollectionInterval, startRequest.CollectionTime);
            }

            const string stateName = DefaultStateNames.DeviceStartSnapshotState;

            // Get the state config
            var jobConfig = GetConfiguration(StateFactory, stateName);

            // Get the required parametersets now
            for (var index = 0; index < jobConfig.OrderConfigurations.Count; index++)
            {
                CreateStartOrders(jobConfig, index, OrderFactory, startRequest);
            }

            // Now create the state
            CreateAndRegisterState(Device, StateFactory, jobConfig);

            return new DefaultBusinessTransactionReply
            {
                RequestData = request
            };
        }
        catch (Exception e)
        {
            return new DefaultBusinessTransactionReply
            {
                RequestData = request,
                ErrorCode = 1002,
                Message = "Starting snapshot failed",
                ExceptionMessage = e.ToString()
            };
        }
    }

    private void CreateStartOrders(IJobStateConfiguration jobConfig, int index, IOrderFactory orderFactory, StartMessagingBusinessTransactionRequestData startRequest)
    {
        var orderConfigName = jobConfig.OrderConfigurations[index];
        var orderConfig = orderFactory.GetConfiguration(orderConfigName);

        ArgumentNullException.ThrowIfNull(orderConfig);

        jobConfig.OrderConfigurationInstances.Add(orderConfig);

        ArgumentNullException.ThrowIfNull(orderConfig, $"Order config for {orderConfigName} is null");
        ArgumentNullException.ThrowIfNull(orderConfig.CreateParameterSetDelegate);

        var ps = (TncpParameterSet)orderConfig.CreateParameterSetDelegate.Invoke();

        switch (index)
        {
            //case 5:
            //    ps.TelnetCommand = "set,snapshot,4,4";   // 
            //    break;
            case 4:
                ps.TelnetCommand = "set,status,start";
                break;
            case 3:
                if (orderConfig is OneRequestSpecNoOrOneStepOneAnswerConfiguration one)
                {
                    one.HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate;
                }
                ps.TelnetCommand = "show,streamconfig";   // 
                break;
            case 2:
                ps.TelnetCommand = $"set,stream,mode,{(startRequest.Snapshot ? "snapshot" : "continuous")}";  // 
                break;
            case 1:
                ps.TelnetCommand = $"set,connection,{IpHelper.GetLocalIpAddress().MapToIPv4()},{UdpPort}";    // 
                break;
            default:
                var param = GetPaths(startRequest);

                ps.TelnetCommand = $"set,stream,order,{param}";   // One channel
                break;
        }

        jobConfig.ParameterSets.Add(ps);
    }

    private MessageHandlingResult HandleRequestAnswerOnSuccessDelegate(IInboundDataMessage? message, object? transportObject, IParameterSet? parameterSet)
    {
        if (message is TncpInboundDataMessage tncp)
        {
            return HandleTncpMessage(tncp);
        }

        return MessageHandlingResultHelper.Success();
    }

    /// <summary>
    /// Get paths
    /// </summary>
    /// <param name="startRequest">Start request</param>
    /// <returns>String with paths as required for Telnet command</returns>
    public static string GetPaths(StartMessagingBusinessTransactionRequestData startRequest)
    {
        var paths = new List<string>();

        if (startRequest.Channel1)
        {
            paths.Add("1");
        }

        if (startRequest.Channel2)
        {
            paths.Add("2");
        }

        if (startRequest.Channel3)
        {
            paths.Add("3");
        }

        if (startRequest.Channel4)
        {
            paths.Add("4");
        }

        var param = string.Join(',', paths);
        return param;
    }

    /// <summary>
    /// Request a stop streamin state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStopMessagingState(IBusinessTransactionRequestData request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(Device);
            ArgumentNullException.ThrowIfNull(StateFactory, "StateFactory is null. Call LoadStateFactory() before!");
            ArgumentNullException.ThrowIfNull(OrderFactory);

            if ((Device.CurrentState?.Id ?? 0) is not (DefaultStateIds.DeviceStreamingState or DefaultStateIds.DeviceSnapshotState))
            {
                return new DefaultBusinessTransactionReply
                {
                    RequestData = request,
                    ErrorCode = 1001,
                    Message = $"Start of the transaction is not allowed as the state is not {DefaultStateNames.DeviceStreamingState} or {DefaultStateNames.DeviceSnapshotState}"
                };
            }

            const string stateName = DefaultStateNames.DeviceStopMessagingState;

            // Get the state config
            var jobConfig = GetConfiguration(StateFactory, stateName);

            // Get the required parametersets now
            foreach (var orderConfigName in jobConfig.OrderConfigurations)
            {
                var orderConfig = OrderFactory.GetConfiguration(orderConfigName);

                ArgumentNullException.ThrowIfNull(orderConfig);
                jobConfig.OrderConfigurationInstances.Add(orderConfig);

                ArgumentNullException.ThrowIfNull(orderConfig, $"Order config for {orderConfigName} is null");
                ArgumentNullException.ThrowIfNull(orderConfig.CreateParameterSetDelegate);

                var ps = (TncpParameterSet)orderConfig.CreateParameterSetDelegate.Invoke();

                ps.TelnetCommand = ps.TelnetCommand = "set,status,stop";

                // ps.TelnetCommand = "StopStreaming";

                jobConfig.ParameterSets.Add(ps);
            }

            // Now create the state
            CreateAndRegisterState(Device, StateFactory, jobConfig);

            // Stop logging now
            StopLogging();

            // Stop collector
            StopCollector();

            return new DefaultBusinessTransactionReply
            {
                RequestData = request
            };
        }
        catch (Exception e)
        {
            return new DefaultBusinessTransactionReply
            {
                RequestData = request,
                ErrorCode = 1001,
                Message = "Stopping streaming failed",
                ExceptionMessage = e.ToString()
            };
        }
    }

    /// <summary>
    /// Request a start streamin state
    /// </summary>
    /// <param name="request">Current request</param>
    public IBusinessTransactionReply RequestDeviceStopSnapshotState(IBusinessTransactionRequestData request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(Device);
            ArgumentNullException.ThrowIfNull(StateFactory, "StateFactory is null. Call LoadStateFactory() before!");

            ArgumentNullException.ThrowIfNull(OrderFactory);

            const string stateName = DefaultStateNames.DeviceStopMessagingState;

            // Get the state config
            var jobConfig = GetConfiguration(StateFactory, stateName);

            // Get the required parametersets now
            foreach (var orderConfigName in jobConfig.OrderConfigurations)
            {
                var orderConfig = OrderFactory.GetConfiguration(orderConfigName);
                ArgumentNullException.ThrowIfNull(orderConfig);
                jobConfig.OrderConfigurationInstances.Add(orderConfig);

                ArgumentNullException.ThrowIfNull(orderConfig, $"Order config for {orderConfigName} is null");
                ArgumentNullException.ThrowIfNull(orderConfig.CreateParameterSetDelegate);

                var ps = (TncpParameterSet)orderConfig.CreateParameterSetDelegate.Invoke();

                ps.TelnetCommand = "set,status,stop";

                jobConfig.ParameterSets.Add(ps);
            }

            // Now create the state
            CreateAndRegisterState(Device, StateFactory, jobConfig);

            // Stop logging now
            StopLogging();

            // Stop collector
            StopCollector();

            return new DefaultBusinessTransactionReply
            {
                RequestData = request
            };
        }
        catch (Exception e)
        {
            return new DefaultBusinessTransactionReply
            {
                RequestData = request,
                ErrorCode = 1003,
                Message = "Stopping snapshot failed",
                ExceptionMessage = e.ToString()
            };
        }

    }

    #endregion

    #region Received messages handling

    /// <summary>
    /// Default method to handle an async received message
    /// </summary>
    public override MessageHandlingResult DefaultHandleAsyncMessage(IStateMachineState state, IInboundDataMessage? message)
    {

        if (message is TncpInboundDataMessage tncp)
        {
            return HandleTncpMessage(tncp);
        }

        // Do nothing
        return MessageHandlingResultHelper.Success();
    }

    private MessageHandlingResult HandleTncpMessage(TncpInboundDataMessage tncp)
    {
        if (tncp.TelnetCommand == null)
        {
            return MessageHandlingResultHelper.Success();
        }

        // Error message received
        if (tncp.TelnetCommand.StartsWith("<BEGIN>AUTO"))
        {
            Device.DataMessagingConfig.AppLogger.LogError($"{LoggerId} Telnet: {tncp.TelnetCommand} AddInfo: {tncp.TelnetAdditionalInfo}");
            return MessageHandlingResultHelper.Success();
        }

        // Error message received
        if (!tncp.TelnetCommand.StartsWith("<BEGIN>show,streamconfig"))
        {
            return MessageHandlingResultHelper.Success();
        }

        // show,streamconfig with additional data
        if (string.IsNullOrEmpty(tncp.TelnetAdditionalInfo))
        {
            return MessageHandlingResultHelper.Error("No data provided for tncp.TelnetAdditionalInfo");
        }

        var config = GetConfig(tncp.TelnetAdditionalInfo);

        var request = new LoadStreamingConfigBusinessTransactionRequestData
        {
            TransactionId = ServerSideBusinessTransactionIds.LoadStreamConfig,
            Config = config
        };

        //var reply = _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
        _businessTransactionManager.RunBusinessTransactionFireAndForget(request.TransactionId, request);

        Device.DataMessagingConfig.AppLogger.LogInformation($"{LoggerId} Telnet: {tncp.TelnetCommand} AddInfo: {tncp.TelnetAdditionalInfo}");

        return MessageHandlingResultHelper.Success();
    }

    /// <summary>
    /// Get the config from a config string
    /// </summary>
    /// <param name="telnetCommand">Current telnet command</param>
    /// <returns>Config array</returns>
    public static byte[] GetConfig(string telnetCommand)
    {
        var configString = telnetCommand.Replace("<CONFIG>", string.Empty, StringComparison.InvariantCultureIgnoreCase);

        var config = ArrayHelper.GetBytes(configString);
        return config;
    }

    #endregion
}