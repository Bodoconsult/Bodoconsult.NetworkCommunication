// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Builtin order types
/// </summary>
public static class BuiltinOrders
{
    public const string EmptyOrder = "EmptyOrder";
    public const string SdcpOrder = "SdcpOrder";
    public const string EdcpClientOrder = "EdcpClientOrder";
    public const string EdcpServerOrder = "EdcpServerOrder";
    public const string BtcpOrder = "BtcpOrder";
    public const string TncpOrder = "TncpOrder";
    
    public const string NoAnswerSdcpOrder = "NoAnswerSdcpOrder";
    public const string NoAnswerEdcpClientOrder = "NoAnswerEdcpClientOrder";
    public const string NoAnswerEdcpServerOrder = "NoAnswerEdcpServerOrder";
    public const string NoAnswerBtcpOrder = "NoAnswerBtcpOrder";
    public const string NoAnswerTncpOrder = "NoAnswerTncpOrder";

    public const string NoHandshakeNoAnswerSdcpOrder = " NoHandshakeNoAnswerSdcpOrder";
    public const string NoHandshakeNoAnswerEdcpClientOrder = "NoHandshakeNoAnswerEdcpClientOrder";
    public const string NoHandshakeNoAnswerEdcpServerOrder = "NoHandshakeNoAnswerEdcpServerOrder";
    public const string NoHandshakeNoAnswerBtcpOrder = "NoHandshakeNoAnswerBtcpOrder";
    public const string NoHandshakeNoAnswerTncpOrder = "NoHandshakeNoAnswerTncpOrder";

    public const string TestOrder = "TestOrder";
    public const string LongRunningTestOrder = "LongRunningTestOrder";
    public const string ExtraLongRunningTestOrder = "ExtraLongRunningTestOrder";
    public const string DummyOrder = "DummyOrder";
}