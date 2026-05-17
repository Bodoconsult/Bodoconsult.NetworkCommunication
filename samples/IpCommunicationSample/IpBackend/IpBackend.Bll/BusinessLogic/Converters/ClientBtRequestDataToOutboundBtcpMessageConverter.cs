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