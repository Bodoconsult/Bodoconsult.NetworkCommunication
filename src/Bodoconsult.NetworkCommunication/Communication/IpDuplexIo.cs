// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;


namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Duplex implementation for IP based networks (UDP/TCP) based on own pipeline implementation
/// </summary>
public class IpDuplexIo : BaseDuplexIo
{
    private readonly DuplexIoIsWorkInProgressDelegate _duplexIoIsWorkInProgressDelegate;
    private readonly DuplexIoNoDataDelegate _duplexIoNoDataDelegate;
    private const int NumberOfRetriesSetWorkinProgress = 100;

    /// <summary>
    /// Is currently a send or receive process in progress
    /// </summary>
    public bool IsWorkInProgress { get; private set; }

    /// <summary>
    /// Lock object for work in progress management
    /// </summary>
    private readonly Lock _lockObject = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    public IpDuplexIo(IDataMessagingConfig dataMessaging, ISendPacketProcessFactory sendPacketProcessFactory) : base(dataMessaging, sendPacketProcessFactory)
    {
        _duplexIoIsWorkInProgressDelegate = DuplexIoIsWorkInProgress;
        _duplexIoNoDataDelegate = DuplexIoSetNotInProgress;
    }

    /// <summary>
    /// Set <see cref="IsWorkInProgress"/> to false
    /// </summary>
    private void DuplexIoSetNotInProgress()
    {
        SetInProgress(false);
    }

    /// <summary>
    /// Check if <see cref="IsWorkInProgress"/> is false or true
    /// </summary>
    /// <returns>False if no work is in progress currently else true</returns>
    private bool DuplexIoIsWorkInProgress()
    {
        if (!IsWorkInProgress)
        {
            SetInProgress(true);
            return false;
        }

        var i = 0;
        while (i < NumberOfRetriesSetWorkinProgress)
        {
            if (!IsWorkInProgress)
            {
                SetInProgress(true);
                return false;
            }

            AsyncHelper.Delay(5);
            i++;
        }

        return true;
    }

    private void SetInProgress(bool value)
    {
        try
        {
            //Debug.Print($"IsWorkInProgress: {value}");
            lock (_lockObject)
            {
                IsWorkInProgress = value;
            }

            return;
        }
        catch
        {
            AsyncHelper.Delay(5);
        }

        lock (_lockObject)
        {
            IsWorkInProgress = value;
        }
    }

    /// <summary>
    /// Start the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    public override async Task StartCommunication()
    {
        await Task.Run(async () =>
        {
            try
            {
                Receiver ??= new IpDuplexIoReceiver(DataMessagingConfig,
                    _duplexIoIsWorkInProgressDelegate,
                    _duplexIoNoDataDelegate);

                await Receiver.StartReceiver();

                Sender ??= new IpDuplexIoSender(DataMessagingConfig,
                    _duplexIoIsWorkInProgressDelegate,
                    _duplexIoNoDataDelegate);
            }
            catch (Exception e)
            {
                DataMessagingConfig.AppLogger.LogError(e, "starting communication failed");
                throw;
            }
        });

        IsCommunicationStarted = true;
    }

    /// <summary>
    /// Stop the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    public override async Task StopCommunication()
    {
        await Task.Run(() =>
        {
            Receiver?.StopReceiver();
        });

        IsCommunicationStarted = false;
    }


    /// <summary>
    /// Current implementation of Dispose()
    /// </summary>
    /// <param name="disposing">Dispose required?</param>
    protected override async Task Dispose(bool disposing)
    {
        if (Receiver == null)
        {
            return;
        }

        try
        {
            await Receiver.StopReceiver();
            await Receiver.DisposeAsync();
        }
        catch
        {
            // Do nothing
        }
    }
}