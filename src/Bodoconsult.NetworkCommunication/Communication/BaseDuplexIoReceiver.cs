// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Base class for <see cref="IDuplexIoReceiver"/> implementations
/// </summary>
public class BaseDuplexIoReceiver : IDuplexIoReceiver
{
    /// <summary>
    /// Logger ID
    /// </summary>
    protected string LoggerId;

    /// <summary>
    /// Current polling timeout in seconds
    /// </summary>
    protected int PollingTimeOut;

    /// <summary>
    /// Current cancellation token source
    /// </summary>
    protected CancellationTokenSource? CancellationSource;

    /// <summary>
    /// Current logger
    /// </summary>
    protected IAppLoggerProxy MonitorLogger;

    /// <summary>
    /// Check and wait until the socket is connected
    /// </summary>
    /// <returns>False if the socket is not connected else true</returns>
    protected async Task<bool> WaitForSocketIsConnected()
    {
        var socketProxy = DataMessagingConfig.SocketProxy;

        // Check if the socket is available and connected
        while (true)
        {
            if (CancellationSource?.Token.IsCancellationRequested ?? true)
            {
                //Trace.TraceInformation("FillMessagePipeline cancelled");
                return false;
            }

            if (socketProxy?.Connected ?? false)
            {
                return true;
            }

            socketProxy = DataMessagingConfig.SocketProxy;

            if (CancellationSource != null)
            {
                await Task.Delay(50, CancellationSource.Token);
            }
            else
            {
                await Task.Delay(50);
            }
        }
    }

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current data messaging config</param>
    public BaseDuplexIoReceiver(IDataMessagingConfig dataMessagingConfig)
    {
        ArgumentNullException.ThrowIfNull(dataMessagingConfig.DataMessageProcessingPackage);

        LoggerId = $"{dataMessagingConfig.LoggerId}{(dataMessagingConfig.LoggerId.EndsWith(": ") ? string.Empty : ": ")}{GetType().Name}: ";

        DataMessagingConfig = dataMessagingConfig;
        DataMessageCodingProcessor = DataMessagingConfig.DataMessageProcessingPackage.DataMessageCodingProcessor;
        DataMessageProcessor = DataMessagingConfig.DataMessageProcessingPackage.DataMessageProcessor;
        DataMessageSplitter = DataMessagingConfig.DataMessageProcessingPackage.DataMessageSplitter;
        MonitorLogger = DataMessagingConfig.MonitorLogger;
    }

    /// <summary>
    /// Current device comm settings
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Current data message splitter
    /// </summary>
    public IDataMessageSplitter DataMessageSplitter { get; private set; }

    /// <summary>
    /// Current data message coding processor
    /// </summary>
    public IDataMessageCodingProcessor DataMessageCodingProcessor { get; private set; }

    /// <summary>
    /// Current data message processor for internal forwarding of the received messages
    /// </summary>
    public IDataMessageProcessor DataMessageProcessor { get; private set; }

    /// <summary>
    /// Start the internal receiver
    /// </summary>
    public virtual async Task StartReceiver()
    {
        if (CancellationSource != null)
        {
            try
            {
                await CancellationSource.CancelAsync();
                CancellationSource?.Dispose();
            }
            catch (Exception e)
            {
                var msg = $"CancellationToken cancelling failed: {e}";
                MonitorLogger.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{LoggerId}{msg}");
            }
        }

        CancellationSource = new();
    }

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    public virtual Task StopReceiver()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    public void UpdateDataMessageProcessingPackage()
    {
        ArgumentNullException.ThrowIfNull(DataMessagingConfig.DataMessageProcessingPackage);
        DataMessageCodingProcessor = DataMessagingConfig.DataMessageProcessingPackage.DataMessageCodingProcessor;
        DataMessageProcessor = DataMessagingConfig.DataMessageProcessingPackage.DataMessageProcessor;
        DataMessageSplitter = DataMessagingConfig.DataMessageProcessingPackage.DataMessageSplitter;
    }

    /// <summary>
    /// Current implementation of disposing
    /// </summary>
    /// <param name="disposing">True if diposing should run</param>
    protected virtual async Task Dispose(bool disposing)
    {
        if (!disposing)
        {
        }

        await Task.Run(() => { });
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public async ValueTask DisposeAsync()
    {
        await Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true).Wait(1000);
        GC.SuppressFinalize(this);
    }
}