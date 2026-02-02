// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.IO.Pipelines;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// High performance implementation based on MS pipeline System.Io.Pipelines for IP based networks (UDP/TCP) 
/// </summary>
public class IpHighPerformanceDuplexIo : BaseDuplexIo, IDuplexPipe
{
    private readonly Pipe _readPipe;
    private readonly Pipe _writePipe;

    /// <summary>
    /// Timeout for polling in milliseconds
    /// </summary>
    public int PollingTimeout { get; set; } = 1000;

    /// <summary>
    /// Default ctor
    /// </summary>
    public IpHighPerformanceDuplexIo(IDataMessagingConfig config, ISendPacketProcessFactory sendPacketProcessFactory) : base(config, sendPacketProcessFactory)
    {
        _readPipe = new Pipe();
        _writePipe = new Pipe();
    }

    /// <summary>
    /// Start the duplex communication
    /// </summary>
    /// <returns>Task</returns>
    public override async Task StartCommunication()
    {
        await Task.Run(async () =>
        {
            if (Receiver == null)
            {
                Receiver = new IpHighPerformanceDuplexIoReceiver(_readPipe, DataMessagingConfig, PollingTimeout);

                await Receiver.StartReceiver();

            }

            if (Sender == null)
            {
                Sender = new IpHighPerformanceDuplexIoSender(_writePipe, DataMessagingConfig, PollingTimeout);

                await Sender.StartSender();
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
    /// Current implemenation of Dispose()
    /// </summary>
    /// <param name="disposing">Dispong required?</param>
    protected override async Task Dispose(bool disposing)
    {
        await StopCommunication();
    }

    /// <summary>Gets the <see cref="T:System.IO.Pipelines.PipeReader" /> half of the duplex pipe.</summary>
    public PipeReader Input => _readPipe.Reader;

    /// <summary>Gets the <see cref="T:System.IO.Pipelines.PipeWriter" /> half of the duplex pipe.</summary>
    public PipeWriter Output => _writePipe.Writer;
}