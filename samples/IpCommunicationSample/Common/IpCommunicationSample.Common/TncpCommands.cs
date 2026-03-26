// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpCommunicationSample.Common
{
    /// <summary>
    /// The TNCP commands used in the sample abbpp</summary>
    public static class TncpCommands
    {
        /// <summary>
        /// Get config from device
        /// </summary>
        public static string GetConfig => "GetConfig";

        /// <summary>
        /// Start the streaming
        /// </summary>
        public static string StartStreaming => "StartStreaming";

        /// <summary>
        /// Stop the streaming
        /// </summary>
        public static string StopStreaming => "StopStreaming";

        /// <summary>
        /// Start snapshot
        /// </summary>
        public static string StartSnapshot => "StartSnapshot";

        /// <summary>
        /// Stop snapshot
        /// </summary>
        public static string StopSnapshot => "StopSnaphot";
    }
}
