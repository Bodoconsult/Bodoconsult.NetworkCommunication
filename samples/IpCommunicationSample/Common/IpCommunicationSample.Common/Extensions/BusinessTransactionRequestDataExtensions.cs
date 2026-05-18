// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using IpCommunicationSample.Common.BusinessTransactions.Requests;

namespace IpCommunicationSample.Common.Extensions
{
    /// <summary>
    /// Extension methods for 
    /// </summary>
    public static class BusinessTransactionRequestDataExtensions
    {
        private const byte Zero = 0x0;
        private const byte One = 0x1;

        public static byte[] GetBytes(this StartMessagingReportBusinessTransactionRequestData data)
        {
            return
            [
                data.Snapshot ? One : Zero,
                data.Channel1 ? One : Zero, 
                data.Channel2 ? One : Zero, 
                data.Channel3 ? One : Zero,
                data.Channel1 ? One : Zero
            ];
        }

        public static StartMessagingReportBusinessTransactionRequestData ToStartMessagingReportBusinessTransactionRequestData(this Memory<byte> data)
        {
            if (data.Length < 5)
            {
                throw new ArgumentException($"data to short for conversion to {nameof(StartMessagingReportBusinessTransactionRequestData)}");
            }
            var result = new StartMessagingReportBusinessTransactionRequestData
            {
                Snapshot = data[..1].Span[0] == 0x1,
                Channel1 = data.Slice(1,1).Span[0] == 0x1,
                Channel2 = data.Slice(2, 1).Span[0] == 0x1,
                Channel3 = data.Slice(3, 1).Span[0] == 0x1,
                Channel4 = data.Slice(4, 1).Span[0] == 0x1
            };

            return result;
        }
    }
}
