// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Exceptions
{
    /// <summary>
    /// Datablock codec exception fired during encoding or decoding datablocks
    /// </summary>
    public class DatablockCodecException: Exception
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="inner">Inner exception</param>
        public DatablockCodecException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
