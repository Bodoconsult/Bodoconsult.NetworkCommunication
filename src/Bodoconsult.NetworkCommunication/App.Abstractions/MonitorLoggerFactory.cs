// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Microsoft.Extensions.Logging;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Server logger factory for tower bound monitor logging
/// </summary>
public class MonitorLoggerFactory : IMonitorLoggerFactory
{

    private ILogger _logger;

    private readonly Type type = typeof(Log4NetLogger);

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="fileName">Current full file path to log in</param>
    public MonitorLoggerFactory(string fileName)
    {
        FileName = fileName;
    }


    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _logger = null;
    }

    /// <summary>
    /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger" /> instance.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>The <see cref="T:Microsoft.Extensions.Logging.ILogger" />.</returns>
    public ILogger CreateLogger(string categoryName)
    {

        // Use caching
        if (_logger != null)
        {
            return _logger;
        }

        //var configFilePath = Path.Combine(new FileInfo(type.Assembly.Location).DirectoryName, "log4net.config");

        //_logger = File.Exists(configFilePath) ? 
        //    new Log4NetLogger(FileName, "log4net.config") : 
        //    new Log4NetLogger(FileName);

        _logger = new Log4NetLogger(FileName);

        //switch (categoryName)
        //{
        //    case "Default":
        //        _logger = new Log4NetLogger(FileName);
        //        break;
        //    case "Display":
        //        _logger = new Log4NetLogger(FileName,3, 1 * 1024 * 1024);
        //        break;
        //    default:
        //        _logger = new Log4NetLogger(FileName);
        //        break;
        //}

        return _logger;
    }

    /// <summary>
    /// Adds an <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" /> to the logging system.
    /// </summary>
    /// <param name="provider">The <see cref="T:Microsoft.Extensions.Logging.ILoggerProvider" />.</param>
    public void AddProvider(ILoggerProvider provider)
    {
        // Do nothing
    }

    /// <summary>
    /// Full file path of the log file
    /// </summary>
    public string FileName { get; }
}