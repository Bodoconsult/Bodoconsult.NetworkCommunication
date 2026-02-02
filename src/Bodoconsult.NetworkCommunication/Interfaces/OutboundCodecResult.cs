// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// The result of a decoding action by a codec 
/// </summary>
public class OutboundCodecResult
{
    /// <summary>
    /// Error code or 0 for no error
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// If message delivers a length 0 value this
    /// message is set with the reason why messaging decoding failed
    /// </summary>
    public string ErrorMessage { get; set; }

    ///// <summary>
    ///// Message encoded by the codec
    ///// </summary>
    //public ReadOnlyMemory<byte> Message { get; set; }

}