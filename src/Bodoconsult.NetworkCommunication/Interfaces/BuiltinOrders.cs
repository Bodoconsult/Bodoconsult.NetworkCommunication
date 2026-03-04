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
    public const string TestOrder = "TestOrder";
    public const string DummyOrder = "DummyOrder";
}