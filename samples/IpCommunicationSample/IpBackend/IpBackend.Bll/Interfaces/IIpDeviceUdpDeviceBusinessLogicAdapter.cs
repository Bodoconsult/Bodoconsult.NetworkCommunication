// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace IpCommunicationSample.Backend.Bll.Interfaces;

/// <summary>
/// Interface for state handling in the UDP channel from backend to IP device
/// </summary>
public interface IIpDeviceUdpDeviceBusinessLogicAdapter : ISimpleDeviceBusinessLogicAdapter
{
    #region Reporting

    /// <summary>
    /// Create an FFT analysis report
    /// </summary>
    /// <param name="requestData"></param>
    /// <returns></returns>
    IBusinessTransactionReply CreateFftAnalysisReport(IBusinessTransactionRequestData requestData);

    #endregion
}