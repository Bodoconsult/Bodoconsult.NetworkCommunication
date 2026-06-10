// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Duplex implementation for IP based networks (UDP only) sending only messages. No receiving.
/// Receiving data with this implementation requires NO recognizable end (STX, ETX, ...) of a datagram
/// </summary>
public class UdpDatagramSendOnlyIpDuplexIo : BaseDuplexIo
{
    private const int NumberOfRetriesSetWorkinProgress = 100;

    /// <summary>
    /// Is currently a send process or a receive process in progress
    /// </summary>
    public bool IsWorkInProgress { get; private set; }

    /// <summary>
    /// Lock object for work in progress management
    /// </summary>
    private readonly Lock _lockObject = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    public UdpDatagramSendOnlyIpDuplexIo(IDataMessagingConfig dataMessaging, ISendPacketProcessFactory sendPacketProcessFactory) : base(dataMessaging, sendPacketProcessFactory)
    { }

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
                Receiver ??= new DoNothingDuplexIoReceiver();

                await Receiver.StartReceiver();

                Sender ??= new UdpDatagramIpDuplexIoSender(DataMessagingConfig);

                await Sender.StartSender();
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