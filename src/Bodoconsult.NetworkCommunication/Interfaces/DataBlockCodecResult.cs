// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// The result of a decoding action for a datablock by a codec 
/// </summary>
public class DataBlockCodecResult
{
    /// <summary>
    /// Error code or 0 for no error
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// If <see cref="DataBlock"/> delivers a NULL value this
    /// message is set with the reason why messaging decoding failed
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Data message decoded by the codec
    /// </summary>
    public IDataMessageDataBlock DataBlock { get; set; }

}