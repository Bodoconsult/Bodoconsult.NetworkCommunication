// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
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
    private Thread? _workerTask;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public SfxpBackendUdpBusinessLogicAdapter(IIpDevice device) : base(device)
    { }

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
        // Do nothing
        return new DefaultBusinessTransactionReply();
    }

    public IBusinessTransactionReply StartStreaming2(IBusinessTransactionRequestData request)
    {
        _cts = new CancellationTokenSource();

        //AsyncHelper.FireAndForget(() =>
        //{
        _workerTask = new Thread(RunStreaming)
        {
            Priority = ThreadPriority.AboveNormal,
            IsBackground = true
        };
        _workerTask.Start();
        //});
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
        _cts.Cancel();
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

        //AsyncHelper.FireAndForget(() =>
        //{
        _workerTask = new Thread(RunSnapshot)
        {
            Priority = ThreadPriority.AboveNormal,
            IsBackground = true
        };
        _workerTask.Start();
        //});
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
        _cts.Cancel();
        return new DefaultBusinessTransactionReply();
    }


    private void RunSnapshot()
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);
        ArgumentNullException.ThrowIfNull(_cts);


        var digitalTwin = new SfxpDigitalTwinMessageFactory();

        while (_cts.IsCancellationRequested)
        {
            var dataBlock = new BasicOutboundDatablock
            {
                Data = digitalTwin.GenerateNextMessage()
            };

            var msg = new SfxpOutboundDataMessage
            {
                DataBlock = dataBlock
            };

            IpDevice.CommunicationAdapter.SendDataMessage(msg);

            Task.Delay(200);
        }
    }

    private void RunStreaming()
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);
        ArgumentNullException.ThrowIfNull(_cts);

        var digitalTwin = new SfxpDigitalTwinMessageFactory();

        while (_cts.IsCancellationRequested)
        {
            var dataBlock = new BasicOutboundDatablock
            {
                Data = digitalTwin.GenerateNextMessage()
            };

            var msg = new SfxpOutboundDataMessage
            {
                DataBlock = dataBlock
            };

            IpDevice.CommunicationAdapter.SendDataMessage(msg);

            Task.Delay(200);
        }
    }
}