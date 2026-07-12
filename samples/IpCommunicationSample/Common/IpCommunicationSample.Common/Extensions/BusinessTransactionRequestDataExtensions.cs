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
            Snapshot = data[..1].Span[0] == 0x1,
            Channel1 = data.Slice(1,1).Span[0] == 0x1,
            Channel2 = data.Slice(2, 1).Span[0] == 0x1,
            Channel3 = data.Slice(3, 1).Span[0] == 0x1,
            Channel4 = data.Slice(4, 1).Span[0] == 0x1,
            IsDataLoggingActivated = data.Slice(5, 1).Span[0] == 0x1,
            IsChartActivated = data.Slice(6, 1).Span[0] == 0x1,
            UseSoftwareSnapshot = data.Slice(7, 1).Span[0] == 0x1,
            CollectionInterval = BitConverter.ToInt32(data.Slice(8, 4).Span),
            CollectionTime = BitConverter.ToInt32(data.Slice(12, 4).Span)
        };

        return result;
    }
}