// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Benchmarking;
using Bodoconsult.App.CentralServices;
using Bodoconsult.App.Factories;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Communication;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;
using Bodoconsult.NetworkCommunication.Devices;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Configurations;
using Bodoconsult.NetworkCommunication.OrderManagement.OrderBuilders;
using Bodoconsult.NetworkCommunication.OrderManagement.ParameterSets;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.StateManagement.StateCheckManagers;


namespace Bodoconsult.NetworkCommunication.Tests.Helpers;

public static class TestDataHelper
{
    private static readonly Random PortGenerator = new();

    static TestDataHelper()
    {
        LogDataFactory = new LogDataFactory();
        LoggingConfig = new LoggingConfig();
        AppEventSourceFactory = new FakeAppEventSourceFactory();
        // ToDo: change to fake later
        AppDateService = new AppDateService();
        FakeIpCommunicationAdapter = new FakeIpCommunicationAdapter();
        DefaultOrderIdGenerator = new DefaultOrderIdGenerator(AppDateService);
    }

    public static DefaultOrderIdGenerator DefaultOrderIdGenerator { get; }

    public static LogDataFactory LogDataFactory { get; }

    public static LoggingConfig LoggingConfig { get; }

    /// <summary>
    /// Get a full set up fake logger
    /// </summary>
    /// <returns>Logger instance</returns>
    public static AppLoggerProxy GetFakeAppLoggerProxy()
    {
        if (_logger != null)
        {
            return _logger;
        }
        _logger = new AppLoggerProxy(new FakeLoggerFactory(), LogDataFactory);
        return _logger;
    }

    private static AppLoggerProxy? _logger;

    /// <summary>
    /// Get a full set up fake bench logger
    /// </summary>
    /// <returns>Bench logger instance</returns>
    public static AppBenchProxy GetFakeAppBenchProxy()
    {
        if (_bench != null)
        {
            return _bench;
        }
        _bench = new AppBenchProxy(new FakeLoggerFactory(), LogDataFactory);
        return _bench;
    }

    private static AppBenchProxy? _bench;

    /// <summary>
    ///  Get a random port (range should be configured in Wiondws Firewall for TCP and UDP). Remember: for UDP is a second port number calculated as port number 2 = port number 1 plus 1 required
    /// </summary>
    /// <returns></returns>
    public static int GetRandomPort()
    {
        var i = 0;

        while (true)
        {
            var port = PortGenerator.Next(33000, 33049);
            if (IpHelper.IsPortAvailable(port) || i >= 5)
            {
                return port;
            }

            i++;
        }
    }

    /// <summary>
    /// Get a messaging config
    /// </summary>
    /// <returns>Data messaging config</returns>
    public static IIpDataMessagingConfig GetDataMessagingConfig()
    {
        var config = new DefaultDataMessagingConfig();

        config.DataMessageProcessingPackage = new SdcpDataMessageProcessingPackage(config);
        config.AppLogger = GetFakeAppLoggerProxy();
        config.MonitorLogger = config.AppLogger;

        return config;
    }

    /// <summary>
    /// Get a messaging config
    /// </summary>
    /// <returns>Data messaging config</returns>
    public static IIpDataMessagingConfig GetSfxpSortableLoggerDataMessagingConfig()
    {
        var config = new DefaultDataMessagingConfig();

        config.DataMessageProcessingPackage = new SfxpLoggedSortableDataMessageProcessingPackage(config);
        config.AppLogger = GetFakeAppLoggerProxy();
        config.MonitorLogger = config.AppLogger;

        return config;
    }

    /// <summary>
    /// Get a EDCP messaging config
    /// </summary>
    /// <returns>Data messaging config</returns>
    /// <param name="isServer">Server config. Default: false</param>
    public static IIpDataMessagingConfig GetEdcpDataMessagingConfig(bool isServer = false)
    {
        var config = new EdcpDataMessagingConfig();

        if (isServer)
        {
            config.DataMessageProcessingPackage = new EdcpServerDataMessageProcessingPackage(config);
        }
        else
        {
            config.DataMessageProcessingPackage = new EdcpClientDataMessageProcessingPackage(config);
        }

        config.AppLogger = GetFakeAppLoggerProxy();
        config.MonitorLogger = config.AppLogger;

        return config;
    }

    /// <summary>
    /// Get a fake <see cref="IAppDateService"/> instance
    /// </summary>
    public static IAppDateService AppDateService { get; }

    /// <summary>
    /// Get a fake <see cref="IAppEventSourceFactory"/> instance
    /// </summary>
    public static IAppEventSourceFactory AppEventSourceFactory { get; }

    /// <summary>
    /// Get a <see cref="FakeIpCommunicationAdapter"/> instance
    /// </summary>
    public static FakeIpCommunicationAdapter FakeIpCommunicationAdapter { get; }

    /// <summary>
    /// Create a SDCP order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateSdcpOrder()
    {
        var ps = new SdcpParameterSet();
        ps.Payload = new byte[] { 0x42, 0x6c, 0x75, 0x62, 0x62 };

        var builder = new SdcpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = builder.CreateOrder(config, 1);
        return order;
    }

    /// <summary>
    /// Create a SDCP order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateSdcpOrder(SdcpParameterSet ps)
    {
        var builder = new SdcpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = builder.CreateOrder(config, 1);
        return order;
    }

    /// <summary>
    /// Create a no answer SDCP order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateNoAnswerSdcpOrder(SdcpParameterSet ps)
    {
        var builder = new NoAnswerSdcpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = builder.CreateOrder(config, 1);
        return order;
    }

    /// <summary>
    /// Create a no handshake no answer SDCP order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateNoHandshakeNoAnswerSdcpOrder(SdcpParameterSet ps)
    {
        var builder = new NoHandshakeNoAnswerSdcpOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = builder.CreateOrder(config, 1);
        return order;
    }

    private static MessageHandlingResult HandleRequestAnswerOnSuccessDelegate(IInboundDataMessage? message, object? transportObject, IParameterSet? parameterSet)
    {
        return new MessageHandlingResult
        {
            ExecutionResult = OrderExecutionResultState.Successful
        };
    }

    /// <summary>
    /// Create a test order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateTestOrder()
    {
        var builder = new TestOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate
        };

        var order = builder.CreateOrder(config, 1);
        return order;
    }

    /// <summary>
    /// Create a test order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateTestOrder(IParameterSet ps)
    {
        var builder = new TestOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.SdcpOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = builder.CreateOrder(config, 1);
        return order;
    }

    /// <summary>
    /// Create a Tncp client order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateEdcpClientOrder(IParameterSet ps)
    {
        var builder = new EdcpClientOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.EdcpClientOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = builder.CreateOrder(config, 1);
        return order;
    }

    /// <summary>
    /// Create a Tncp server order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateEdcpServerOrder(IParameterSet ps)
    {
        var builder = new EdcpServerOrderBuilder();

        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.EdcpServerOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = builder.CreateOrder(config, 1);
        return order;
    }

    /// <summary>
    /// Create a BTCP order for testing
    /// </summary>
    /// <returns></returns>
    public static IOrder CreateBtcpOrder(IParameterSet ps)
    {
        var builder = new BtcpOrderBuilder();


        var config = new OneRequestSpecNoOrOneStepOneAnswerConfiguration("TestConfig", BuiltinOrders.EdcpServerOrder, builder)
        {
            //Device = TestDataHelper.CreateStateMachineDevice(),
            HandleRequestAnswerOnSuccessDelegate = HandleRequestAnswerOnSuccessDelegate,
            ParameterSet = ps
        };

        var order = builder.CreateOrder(config, 1);
        return order;
    }

    /// <summary>
    /// Create a simple order management device without state machine
    /// </summary>
    /// <returns></returns>
    public static FakeSimpleDevice CreateSimpleDevice()
    {
        var device = new FakeSimpleDevice(GetDataMessagingConfig(), new DoNothingOrderManagementClientNotificationManager());
        return device;
    }

    /// <summary>
    /// Create an order management device without state machine
    /// </summary>
    /// <returns></returns>
    public static FakeOrderManagementDevice CreateOrderManagementDevice()
    {
        var device = new FakeOrderManagementDevice(GetDataMessagingConfig(), new DoNothingOrderManagementClientNotificationManager());
        return device;
    }

    /// <summary>
    /// Create a simple order management device with state machine
    /// </summary>
    /// <returns></returns>
    public static FakeStateMachineDevice CreateStateMachineDevice()
    {
        var orderFactory = new OrderFactory(DefaultOrderIdGenerator);

        var config = new TncpOrderConfiguration
        {
            CreateParameterSetDelegate = () => new TncpParameterSet()
        };

        orderFactory.RegisterConfiguration(config);

        var fakeOrderPipeline = new FakeOrderPipeline();
        var syncOrderManager = new SyncOrderManager();
        var clientNotificationManager = new DoNothingOrderManagementClientNotificationManager();
        var fakeOrderReiever = new FakeOrderReceiver();
        var stateCheckManager = new DoNothingStateCheckManager();

        var commAdapter = FakeIpCommunicationAdapter;
        var device = new FakeStateMachineDevice(GetDataMessagingConfig(), clientNotificationManager, stateCheckManager);
        var om = new FakeOrderManager(GetDataMessagingConfig(), new FakeOrderProcessor(device, fakeOrderPipeline, syncOrderManager, clientNotificationManager), fakeOrderReiever, orderFactory)
        {
            OrderProcessor = new FakeOrderProcessor(device, fakeOrderPipeline, syncOrderManager, clientNotificationManager)
        };

        device.LoadCommAdapter(commAdapter);
        device.LoadDeviceOrderManager(om);
        return device;
    }
}