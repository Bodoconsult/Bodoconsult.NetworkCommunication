//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//using Bodoconsult.App.Interfaces;
//using Bodoconsult.NetworkCommunication.EnumAndStates;
//using Bodoconsult.NetworkCommunication.Interfaces;
//using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

//namespace Bodoconsult.NetworkCommunication.OrderManagement.Orders;

///// <summary>
///// Order running a TNCP transaction
///// </summary>
//public class TestOrder : BaseOrder
//{
//    /// <summary>
//    /// Default ctor
//    /// </summary>
//    /// <param name="parameterSet">Parameter set</param>
//    /// <param name="dateTimeService">Datetime service</param>
//    /// <param name="benchLogger">Bench logger instance for benchmarking</param>
//    public TestOrder(IParameterSet parameterSet, IAppDateService dateTimeService, IAppBenchProxy benchLogger) : base(parameterSet, dateTimeService, benchLogger)
//    {
//        TraceCodeSuccess = TraceCodes.IdsMsgTestOrderOk;
//        TraceCodeError = TraceCodes.IdsMsgTestOrderFails;

//        TraceMessage = "Test order";
//    }
//}