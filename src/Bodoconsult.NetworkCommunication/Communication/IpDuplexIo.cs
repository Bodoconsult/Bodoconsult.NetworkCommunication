// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Duplex implementation for IP based networks (TCP or UDP) based on own pipeline implementation receiving data from TCP or UDP stream.
/// Receiving data with this implementation requires a recognizable end (STX, ETX, ...) of a message / datagram
/// </summary> 
public class IpDuplexIo : BaseDuplexIo
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public IpDuplexIo(IDataMessagingConfig dataMessaging, ISendPacketProcessFactory sendPacketProcessFactory) : base(dataMessaging, sendPacketProcessFactory)
    { }

    /// <summary>
    /// Start the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    public override async Task StartCommunication()
    {
        try
        {
            Receiver ??= new IpDuplexIoReceiver(DataMessagingConfig);

            await Receiver.StartReceiver();

            Sender ??= new IpDuplexIoSender(DataMessagingConfig);

            IsCommunicationStarted = true;
        }
        catch (Exception e)
        {
            var msg = $"StartCommunication: {e}";
            DataMessagingConfig.AppLogger.LogError(msg);
            DataMessagingConfig.MonitorLogger.LogError($"{LoggerId}{msg}");

            IsCommunicationStarted = false;
            throw;
        }
    }

    /// <summary>
    /// Stop the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    public override async Task StopCommunication()
    {
        if (Sender != null)
        {
            await Sender.StopSender();
        }

        if (Receiver != null)
        {
            await Receiver.StopReceiver();
        }

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