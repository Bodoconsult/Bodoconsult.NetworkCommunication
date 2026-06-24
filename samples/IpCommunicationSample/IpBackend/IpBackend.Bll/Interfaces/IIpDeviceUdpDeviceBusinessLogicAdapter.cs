// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpBackend.Bll.Interfaces;

/// <summary>
/// Interface for state handling in the UDP channel from backend to IP device
/// </summary>
public interface IIpDeviceUdpDeviceBusinessLogicAdapter : ISimpleDeviceBusinessLogicAdapter
{
    #region Basics


    /// <summary>
    /// Start the binary data loggers
    /// </summary>
    /// <param name="requestData">Empty request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply StartDataLoggers(IBusinessTransactionRequestData requestData);

    /// <summary>
    /// Flush the binary data loggers
    /// </summary>
    /// <param name="requestData">Empty request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply FlushDataLoggers(IBusinessTransactionRequestData requestData);
    
    /// <summary>
    /// Stop the binary data loggers
    /// </summary>
    /// <param name="requestData">Empty request parameter</param>
    /// <returns>Reply</returns>
    public IBusinessTransactionReply StopDataLoggers(IBusinessTransactionRequestData requestData);

    /// <summary>
    /// Send the required client hello to the server
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    IBusinessTransactionReply CheckConnection(IBusinessTransactionRequestData requestData);

    /// <summary>
    /// Send the required client hello to the server
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    IBusinessTransactionReply SendClientHello(IBusinessTransactionRequestData requestData);

    /// <summary>
    /// Load the streaming config
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    IBusinessTransactionReply LoadStreamingConfig(IBusinessTransactionRequestData requestData);

    /// <summary>
    /// Start data logging
    /// </summary>
    /// <param name="requestData">Empty request</param>
    /// <returns>Reply</returns>
    IBusinessTransactionReply StartDataLogging(IBusinessTransactionRequestData requestData);

    /// <summary>
    /// Stop data logging
    /// </summary>
    /// <param name="requestData">Empty request</param>
    /// <returns>Reply</returns>
    IBusinessTransactionReply StopDataLogging(IBusinessTransactionRequestData requestData);

    #endregion


    #region Reporting

    /// <summary>
    /// Create an FFT analysis report
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    IBusinessTransactionReply CreateFftAnalysisReport(IBusinessTransactionRequestData requestData);

    #endregion
}