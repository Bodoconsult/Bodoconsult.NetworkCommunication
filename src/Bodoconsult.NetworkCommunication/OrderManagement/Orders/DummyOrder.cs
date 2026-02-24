// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Orders;

/// <summary>
/// Dummy order doing nothing
/// </summary>
public class DummyOrder: BaseOrder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="parameterSet">Parameter set</param>
    /// <param name="dateTimeService">Datetime service</param>
    /// <param name="benchLogger">Bench logger instance for benchmarking</param>
    public DummyOrder(IParameterSet parameterSet, IDateTimeService dateTimeService, IAppBenchProxy benchLogger) : base(parameterSet, dateTimeService, benchLogger)
    { }
}