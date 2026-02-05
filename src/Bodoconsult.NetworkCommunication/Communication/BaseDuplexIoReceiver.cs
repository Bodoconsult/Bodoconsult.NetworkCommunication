// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Base class for <see cref="IDuplexIoReceiver"/> implementations
/// </summary>
public class BaseDuplexIoReceiver : IDuplexIoReceiver
{

    /// <summary>
    /// Current polling timeout in seconds
    /// </summary>
    protected int PollingTimeOut;

    /// <summary>
    /// Work is in progress delegate for DuplexIO
    /// </summary>
    protected DuplexIoIsWorkInProgressDelegate DuplexIoIsWorkInProgressDelegate;

    /// <summary>
    /// No data available delegate for DuplexIO
    /// </summary>
    protected DuplexIoNoDataDelegate DuplexIoNoDataDelegate;

    /// <summary>
    /// Current cancellation token source
    /// </summary>
    protected CancellationTokenSource CancellationSource;

    /// <summary>
    /// Current logger
    /// </summary>
    protected IAppLoggerProxy Logger;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current data messaging config</param>
    public BaseDuplexIoReceiver(IDataMessagingConfig dataMessagingConfig)
    {
        DataMessagingConfig = dataMessagingConfig;
        UpdateDataMessageProcessingPackage();
        Logger = DataMessagingConfig.MonitorLogger;
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
    /// Thread filling receiver pipeline
    /// </summary>
    public Thread FillPipelineTask { get; protected set; }

    /// <summary>
    /// Thread sending messages from receiver pipeline to app internal consumers
    /// </summary>
    public Thread SendPipelineTask { get; protected set; }

    /// <summary>
    /// Start the internal receiver
    /// </summary>
    public virtual async Task StartReceiver()
    {
        if (CancellationSource != null)
        {
            try
            {
                CancellationSource.Cancel();
                CancellationSource?.Dispose();
            }
            catch (Exception e)
            {
                Logger?.LogError("CancellationToken cancelling failed", e);
            }
        }

        CancellationSource = new();

        await Task.Run(() =>
        {
            FillPipelineTask = new Thread(StartSendMessagePipeline)
            {
                Priority = ThreadPriority.Normal,
                IsBackground = true
            };
            FillPipelineTask.Start();
        });

        await Task.Run(() =>
        {
            FillPipelineTask = new Thread(StartFillMessagePipeline)
            {
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true
            };
            FillPipelineTask.Start();
        });
    }

    /// <summary>
    /// Stop the internal receiver
    /// </summary>
    public virtual Task StopReceiver()
    {
        throw new NotSupportedException();
    }

    public void StartFillMessagePipeline()
    {
        Debug.Print("StartFillMessagePipeline in progress");

        try
        {

            //while (!_cancellationSource.Token.IsCancellationRequested)
            //{

            //    if (!DataMessagingConfig.SocketProxy.Connected)
            //    {
            //        AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(new SocketException()));
            //        break;
            //    }

            var task = Task.Run(FillMessagePipeline);
            task.Wait();
            task.Dispose();

            //AsyncHelper.Delay(FillPipelineTimeout);

            //}

        }
        catch (Exception exception)
        {
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
        }
    }

    public void StartSendMessagePipeline()
    {
        Debug.Print("StartSendMessagePipeline in progress");

        try
        {

            //while (!_cancellationSource.Token.IsCancellationRequested)
            //{

            //    if (!DataMessagingConfig.SocketProxy.Connected)
            //    {
            //        AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(new SocketException()));
            //        break;
            //    }

            var task = Task.Run(SendMessagePipeline);
            task.Wait();
            task.Dispose();

            //AsyncHelper.Delay(FillPipelineTimeout);

            //}

        }
        catch (Exception exception)
        {
            AsyncHelper.FireAndForget(() => DataMessagingConfig.DuplexIoErrorHandlerDelegate?.Invoke(exception));
        }
    }

    /// <summary>
    /// Receive messages from the device.
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    /// <returns>Received device message or null in case of any error</returns>
    public virtual Task FillMessagePipeline()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Process the messages received from device internally
    /// This method is not intended to be called directly from production code.
    /// It is a unit test method.
    /// </summary>
    public virtual Task SendMessagePipeline()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Update the data message processing package
    /// </summary>
    public void UpdateDataMessageProcessingPackage()
    {
        if (DataMessagingConfig.DataMessageProcessingPackage == null)
        {
            throw new ArgumentNullException(nameof(DataMessagingConfig.DataMessageProcessingPackage));
        }
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