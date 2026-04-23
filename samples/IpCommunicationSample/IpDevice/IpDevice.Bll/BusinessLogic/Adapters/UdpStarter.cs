// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace IpDevice.Bll.BusinessLogic.Adapters
{
    public class UdpStarter
    {
        /// <summary>
        /// Current business transaction ID
        /// </summary>
        public int BusinessTransactionId { get; private set; }

        /// <summary>
        /// Snapshot mode?
        /// </summary>
        public bool Snapshot { get; set; }

        public void ParseCommand(string command)
        {
            if (command == "set,stream,mode,snapshot")
            {
                Snapshot = true;
            }

            if (command == "set,status,start")
            {
                BusinessTransactionId = Snapshot ? 3 : 1;
            }

            if (command == "set,status,stop")
            {
                BusinessTransactionId = 2;
            }

        }

    }
}
