// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpCommunicationSample.Common.Extensions;

/// <summary>
/// Extension methods for 
/// </summary>
public static class BusinessTransactionRequestDataExtensions
{
    private const byte Zero = 0x0;
    private const byte One = 0x1;

    public static byte[] GetBytes(this StartMessagingBusinessTransactionRequestData data)
    {
        var result = new List<byte>
        {
            data.Snapshot ? One : Zero,
            data.Channel1 ? One : Zero,
            data.Channel2 ? One : Zero,
            data.Channel3 ? One : Zero,
            data.Channel4 ? One : Zero,
            data.IsDataLoggingActivated ? One : Zero,
            data.IsChartActivated ? One : Zero,
            data.UseSoftwareSnapshot ? One : Zero
        };

        result.AddRange(BitConverter.GetBytes(data.CollectionInterval));
        result.AddRange(BitConverter.GetBytes(data.CollectionTime));

        return result.ToArray() ;
    }

    public static StartMessagingBusinessTransactionRequestData ToStartMessagingReportBusinessTransactionRequestData(this Memory<byte> data)
    {
        if (data.Length < 8)
        {
            throw new ArgumentException($"data to short for conversion to {nameof(StartMessagingBusinessTransactionRequestData)}");
        }
        var result = new StartMessagingBusinessTransactionRequestData
        {
            Snapshot = data.Span[0] == 0x1,
            Channel1 = data.Span[1] == 0x1,
            Channel2 = data.Span[2] == 0x1,
            Channel3 = data.Span[3] == 0x1,
            Channel4 = data.Span[4] == 0x1,
            IsDataLoggingActivated = data.Span[5] == 0x1,
            IsChartActivated = data.Span[6] == 0x1,
            UseSoftwareSnapshot = data.Span[7] == 0x1,
            CollectionInterval = BitConverter.ToInt32(data.Slice(8, 4).Span),
            CollectionTime = BitConverter.ToInt32(data.Slice(12, 4).Span)
        };

        return result;
    }
}