// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Faktory for monitor logger factories
/// </summary>
public class MonitorLoggerFactoryFactory : IMonitorLoggerFactoryFactory
{
    private readonly object _lock = new();
    private readonly Dictionary<string, MonitorLoggerFactory> _loggerFactories = new();
    private readonly IAppGlobals _appGlobals;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Current app globals</param>
    public MonitorLoggerFactoryFactory(IAppGlobals appGlobals)
    {
        _appGlobals = appGlobals;
    }

    /// <summary>
    /// Create a monitor logger factory
    /// </summary>
    /// <param name="deviceName">Current tower serial number</param>
    /// <returns></returns>
    public IMonitorLoggerFactory CreateInstance(string deviceName)
    {
        var fileName = Path.Combine(_appGlobals.AppStartParameter.DataPath, $"{deviceName}.log");

        var factory = new MonitorLoggerFactory(fileName);
        return factory;
    }

    /// <summary>
    /// Create a monitor logger factory
    /// </summary>
    /// <param name="clientType">Client type as string</param>
    /// <param name="ipAddress">Current IP address of the client</param>
    /// <returns>Monitor logger factory</returns>
    public IMonitorLoggerFactory CreateInstance(string clientType, string ipAddress)
    {
        var fileName = Path.Combine(_appGlobals.AppStartParameter.DataPath, $"Cients_{clientType}_{ipAddress.Replace(".", "_")}.log");
        return CreateInstanceInternally(fileName);
    }

    ///// <summary>
    ///// Create a monitor logger factory
    ///// </summary>
    ///// <param name="clientType">Client type as string</param>
    ///// <returns>Monitor logger factory</returns>
    //public IMonitorLoggerFactory CreateInstance(string clientType)
    //{
    //    var fileName = Path.Combine(Globals.DataPath, $"StSys_{clientType}.log");

    //    return CreateInstanceInternally(fileName);
    //}

    /// <summary>
    /// Internal method to create the client logger factory
    /// </summary>
    /// <param name="fileName">Full filepath for the logfile</param>
    /// <returns>Client logger factory</returns>
    private IMonitorLoggerFactory CreateInstanceInternally(string fileName)
    {
        //lock (_lock)
        //{
        if (_loggerFactories.ContainsKey(fileName))
        {
            var success = _loggerFactories.TryGetValue(fileName, out var factory);
            if (success)
            {
                return factory;
            }

            throw new ArgumentException("Client logger factory not accessible");
        }
        else
        {
            var factory = new MonitorLoggerFactory(fileName);
            var success = _loggerFactories.TryAdd(fileName, factory);

            if (success)
            {
                return factory;
            }

            throw new ArgumentException("Client logger factory could not be added to internal cache");

        }
        //}
    }
}