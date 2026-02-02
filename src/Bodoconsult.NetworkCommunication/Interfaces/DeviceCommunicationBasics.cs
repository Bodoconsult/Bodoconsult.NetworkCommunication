// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// A holder class for basic device communication values
/// </summary>
public static class DeviceCommunicationBasics
{
        

    #region Messaging

    /// <summary>
    /// Maximum number of send attemps
    /// </summary>
    public static int MaxSendAttemptCount { get; set; } =  3;

    /// <summary>
    /// The minimum size of a data message excluding start, end and data block bytes
    /// </summary>
    public static int DataMessageMinPacketSize { get; set; } = 0;

    /// <summary>
    /// The maximum size of a data message excluding start, end and data block bytes
    /// </summary>
    public static int DataMessageMaxPacketSize { get; set; } = 1000;

    #endregion


    #region Command timeouts

    /// <summary>
    /// The default timeout in milliseconds
    /// </summary>
    public static int DefaultTimeout { get; set; } = 2000;

    /// <summary>
    /// Timeout for a PING sent to the tower
    /// </summary>
    public static int PingTimeout { get; set; } = 500;

    /// <summary>
    /// Timeout for waiting for a handshake like ACK, NACK or CAN
    /// </summary>
    public static int WaitForAckTimeout { get; set; } = 12000;

    #endregion

    #region Message tokens

    /// <summary>
    /// All tokens a message can start with
    /// </summary>
    public static List<byte> MessageStartTokens =
    [
        Ack,
        Can,
        Nack,
        Stx
    ];

    /// <summary>
    /// All tokens a handshake message can start with
    /// </summary>
    public static List<byte> HandshakeMessageStartTokens =
    [
        Ack,
        Can,
        Nack
    ];

    #endregion

    #region Constants

    /// <summary>
    /// Data message start
    /// </summary>
    public const byte Stx = 2;      // 0x02

    /// <summary>
    /// Data message end
    /// </summary>
    public const byte Etx = 3;      // 0x03

    /// <summary>
    /// Handshake ACK
    /// </summary>
    public const byte Ack = 6;      // 0x06

    /// <summary>
    /// Handshake NACK
    /// </summary>
    public const byte Nack = 21;    // 0x15

    /// <summary>
    /// Handshake CAN
    /// </summary>
    public const byte Can = 24;     // 0x18

    /// <summary>
    /// Null byte
    /// </summary>
    public const byte NullByte = 0x0;

    #endregion

        
}