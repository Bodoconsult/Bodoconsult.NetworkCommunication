// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpCommunicationSample.Device.Bll.Interfaces;

/// <summary>
/// Interface for the data channel from device to backend via UDP
/// </summary>
public interface IBackendUdpBusinessLogicAdapter : IDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Start the UDP channel
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply StartCommunication(IBusinessTransactionRequestData request);

    /// <summary>
    /// Stop the UDP channel
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply StopCommunication(IBusinessTransactionRequestData request);

    /// <summary>
    /// Start streaming
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply StartStreaming(IBusinessTransactionRequestData request);

    /// <summary>
    /// Start streaming 2
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply StartStreaming2(IBusinessTransactionRequestData request);

    /// <summary>
    /// Stop streaming
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply StopStreaming(IBusinessTransactionRequestData request);

    /// <summary>
    /// Start snapshot
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply StartSnapshot(IBusinessTransactionRequestData request);

    /// <summary>
    /// Stop snapshot
    /// </summary>
    /// <param name="request">Empty request</param>
    /// <returns>Empty reply</returns>
    IBusinessTransactionReply StopSnapshot(IBusinessTransactionRequestData request);
}