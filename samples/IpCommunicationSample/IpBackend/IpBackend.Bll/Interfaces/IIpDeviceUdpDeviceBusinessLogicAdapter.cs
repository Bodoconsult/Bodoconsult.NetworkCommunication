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
    /// Send the required client hello to the server
    /// </summary>
    /// <param name="requestData">Current request parameter</param>
    /// <returns>Reply</returns>
    IBusinessTransactionReply SendClientHello(IBusinessTransactionRequestData requestData);

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