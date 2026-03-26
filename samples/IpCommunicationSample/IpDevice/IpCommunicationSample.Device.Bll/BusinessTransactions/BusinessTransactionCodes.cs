// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpCommunicationSample.Device.Bll.BusinessTransactions
{
    public class BusinessTransactionCodes
    {
        public const int StartCommunication = 1;
        public const int StopCommunication = 2;
        public const int StartStreaming = 3;
        public const int StopStreaming = 4;
        public const int StartSnapshot = 5;
        public const int StopSnapshot = 6;
    }
}
