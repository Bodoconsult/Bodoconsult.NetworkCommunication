// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.BusinessLogicAdapters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.DataMessaging.DigitalTwins;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpDevice.Bll.Interfaces;

namespace IpDevice.Bll.BusinessLogic.Adapters;

/// <summary>
/// Current adapter for UPD data channel from backend to IP device
/// </summary>
public class SfxpBackendUdpBusinessLogicAdapter : BaseSimpleDeviceBusinessLogicAdapter, IBackendUdpBusinessLogicAdapter
{
    private CancellationTokenSource? _cts;
    private readonly ProducerConsumerQueue<IOutboundDataMessage> _outboundQueue = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public SfxpBackendUdpBusinessLogicAdapter(IIpDevice device) : base(device)
    {
        _outboundQueue.ConsumerTaskDelegate = ConsumerTaskDelegate;
        _outboundQueue.StartConsumer();
    }

    private void ConsumerTaskDelegate(IOutboundDataMessage msg)
    {
        if (_cts?.IsCancellationRequested ?? true)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);

        try
        {
            //Debug.Print($"Send message {msg.ToShortInfoString()}");
            IpDevice.CommunicationAdapter.SendDataMessage(msg);
        }
        catch (Exception e)
        {
            AppLogger.LogError($"Send message failed: {msg.ToShortInfoString()}", e);
        }
    }

    /// <summary>
    /// Default method to handle a received message from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public override void DefaultReceiveMessage(IInboundDataMessage message)
    {
        // Do nothing
    }

    /// <summary>
    /// Start the UDP channel
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    public IBusinessTransactionReply StartCommunication(IBusinessTransactionRequestData request)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);
        IpDevice.CommunicationAdapter.ComDevInit();
        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Stop the UDP channel
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    public IBusinessTransactionReply StopCommunication(IBusinessTransactionRequestData request)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);
        IpDevice.CommunicationAdapter.ComDevClose();
        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Start streaming
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    public IBusinessTransactionReply StartStreaming(IBusinessTransactionRequestData request)
    {
        _cts = new CancellationTokenSource();
        Task.Factory.StartNew(RunStreaming, TaskCreationOptions.LongRunning);
        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Stop streaming
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    public IBusinessTransactionReply StopStreaming(IBusinessTransactionRequestData request)
    {
        ArgumentNullException.ThrowIfNull(_cts);
        _cts.Cancel(false);
        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Start snapshot
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    public IBusinessTransactionReply StartSnapshot(IBusinessTransactionRequestData request)
    {
        _cts = new CancellationTokenSource();
        Task.Factory.StartNew(RunSnapshot, TaskCreationOptions.LongRunning);
        return new DefaultBusinessTransactionReply();
    }

    /// <summary>
    /// Stop snapshot
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    public IBusinessTransactionReply StopSnapshot(IBusinessTransactionRequestData request)
    {
        ArgumentNullException.ThrowIfNull(_cts);
        _cts.Cancel(false);
        return new DefaultBusinessTransactionReply();
    }


    private void RunSnapshot()
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);
        ArgumentNullException.ThrowIfNull(_cts);

        Task.Delay(2000).Wait();

        var digitalTwin = new SfxpDigitalTwinMessageFactory();

        while (!_cts.IsCancellationRequested)
        {
            var dataBlock = new BasicOutboundDatablock
            {
                Data = digitalTwin.GenerateNextMessage()
            };

            var msg = new SfxpOutboundDataMessage
            {
                DataBlock = dataBlock
            };

            _outboundQueue.Enqueue(msg);
        }
    }

    //private void SendMessage(SfxpOutboundDataMessage msg)
    //{
    //    ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);
    //    //IpDevice.CommunicationAdapter.SendDataMessage(msg);


    //    // ToDo: replace this with a produce-consumer-queue handling the _cts correctly
    //    AsyncHelper.FireAndForget(() =>
    //    {
    //        try
    //        {
    //            Debug.Print($"Send message {msg.ToShortInfoString()}");
    //            IpDevice.CommunicationAdapter.SendDataMessage(msg);
    //        }
    //        catch (Exception e)
    //        {
    //            AppLogger.LogError($"Send message failed: {msg.ToShortInfoString()}", e);
    //        }
    //    });
    //}

    private void RunStreaming()
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);
        ArgumentNullException.ThrowIfNull(_cts);

        Task.Delay(2000).Wait();

        var digitalTwin = new SfxpDigitalTwinMessageFactory();

        ulong id = 0;

        while (!_cts.IsCancellationRequested)
        {
            var dataBlock = new BasicOutboundDatablock
            {
                Data = digitalTwin.GenerateNextMessage()
            };

            var msg = new SfxpOutboundDataMessage
            {
                DataBlock = dataBlock,
                OriginalMessageId = id
            };

            //Debug.Print(id.ToString());

            _outboundQueue.Enqueue(msg);

            if (id == ulong.MaxValue)
            {
                id = 0;
            }
            else
            {
                id++;
            }
        }

        Debug.Print("Streaming stopped");
    }
}