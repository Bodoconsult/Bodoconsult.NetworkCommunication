// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Builtin order types
/// </summary>
public static class BuiltinOrders
{
    /// <summary>
    /// Empty order name
    /// </summary>
    public const string EmptyOrder = "EmptyOrder";

    /// <summary>
    /// Sdcp order name
    /// </summary>
    public const string SdcpOrder = "SdcpOrder";

    /// <summary>
    /// EDCP client order name
    /// </summary>
    public const string EdcpClientOrder = "EdcpClientOrder";

    /// <summary>
    /// EDCP server order name
    /// </summary>
    public const string EdcpServerOrder = "EdcpServerOrder";

    /// <summary>
    /// BTCP order name
    /// </summary>
    public const string BtcpOrder = "BtcpOrder";

    /// <summary>
    /// TNCP order name
    /// </summary>
    public const string TncpOrder = "TncpOrder";

    /// <summary>
    /// No answer SDCP order name
    /// </summary>
    public const string NoAnswerSdcpOrder = "NoAnswerSdcpOrder";

    /// <summary>
    /// No answer EDCP client order name
    /// </summary>
    public const string NoAnswerEdcpClientOrder = "NoAnswerEdcpClientOrder";

    /// <summary>
    /// No answer EDCP server order name
    /// </summary>
    public const string NoAnswerEdcpServerOrder = "NoAnswerEdcpServerOrder";

    /// <summary>
    /// No answer BTCP order name
    /// </summary>
    public const string NoAnswerBtcpOrder = "NoAnswerBtcpOrder";

    /// <summary>
    /// No answer TNCP order name
    /// </summary>
    public const string NoAnswerTncpOrder = "NoAnswerTncpOrder";

    /// <summary>
    /// No answer and no handshake SDCP order name
    /// </summary>
    public const string NoHandshakeNoAnswerSdcpOrder = " NoHandshakeNoAnswerSdcpOrder";

    /// <summary>
    /// No answer and no handshake EDCP client order name
    /// </summary>
    public const string NoHandshakeNoAnswerEdcpClientOrder = "NoHandshakeNoAnswerEdcpClientOrder";

    /// <summary>
    /// No answer and no handshake EDCP server order name
    /// </summary>
    public const string NoHandshakeNoAnswerEdcpServerOrder = "NoHandshakeNoAnswerEdcpServerOrder";

    /// <summary>
    /// No answer and no handshake BTCP order name
    /// </summary>
    public const string NoHandshakeNoAnswerBtcpOrder = "NoHandshakeNoAnswerBtcpOrder";

    /// <summary>
    /// No answer and no handshake TNCP order name
    /// </summary>
    public const string NoHandshakeNoAnswerTncpOrder = "NoHandshakeNoAnswerTncpOrder";

    /// <summary>
    /// Test order name
    /// </summary>
    public const string TestOrder = "TestOrder";

    /// <summary>
    /// Longrunning test order name
    /// </summary>
    public const string LongRunningTestOrder = "LongRunningTestOrder";

    /// <summary>
    /// Extra longrunning test order name
    /// </summary>
    public const string ExtraLongRunningTestOrder = "ExtraLongRunningTestOrder";

    /// <summary>
    /// Dummy order name
    /// </summary>
    public const string DummyOrder = "DummyOrder";
}