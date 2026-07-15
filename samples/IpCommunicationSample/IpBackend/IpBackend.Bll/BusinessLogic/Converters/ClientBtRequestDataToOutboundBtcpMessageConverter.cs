// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.BusinessTransactions.Converters;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using System.Text;

namespace IpBackend.Bll.BusinessLogic.Converters;

public class ClientBtRequestDataToOutboundBtcpMessageConverter : BaseBtRequestDataToOutboundBtcpMessageConverter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appLogger">Current app logger</param>
    /// <param name="appGlobals">Current app globals</param>
    public ClientBtRequestDataToOutboundBtcpMessageConverter(IAppLoggerProxy appLogger, IAppGlobals appGlobals) : base(appLogger, appGlobals)
    {
        AllBusinessTransactionRequestDataDelegates.Add(nameof(ErrorBusinessTransactionRequestData), CreateError);
        AllBusinessTransactionRequestDataDelegates.Add(nameof(FftReportBusinessTransactionRequestData), CreateFft);
    }

    private static IOutboundBusinessTransactionDataMessage CreateFft(IBusinessTransactionRequestData request)
    {
        if (request is not FftReportBusinessTransactionRequestData fft)
        {
            throw new ArgumentException($"Request must be {nameof(FftReportBusinessTransactionRequestData)}");
        }

        var db = new BasicOutboundDatablock
        {
            Data = GetBytes(fft)
        };

        var message = new BtcpRequestOutboundDataMessage(fft.TransactionId, fft.TransactionGuid)
        {
            DataBlock = db
        };

        return message;
    }

    private static Memory<byte> GetBytes(FftReportBusinessTransactionRequestData fft)
    {
        var list = new List<byte>(fft.JpegImageData.Length + 1) { 0x66 };
        list.AddRange(fft.JpegImageData);
        return list.ToArray();
    }

    private static IOutboundBusinessTransactionDataMessage CreateError(IBusinessTransactionRequestData request)
    {
        if (request is not ErrorBusinessTransactionRequestData err)
        {
            throw new ArgumentException($"Request must be {nameof(ErrorBusinessTransactionRequestData)}");
        }

        var bytes = Encoding.UTF8.GetBytes($"e{err.TelnetCommand}|{err.TelnetAdditionalInfo}");

        var db = new BasicOutboundDatablock
        {
            Data = bytes
        };

        var message = new BtcpRequestOutboundDataMessage(err.TransactionId, err.TransactionGuid)
        {
            DataBlock = db
        };

        return message;
    }

    // No more requests to handle here

    // Notfications are not handled here. See
}