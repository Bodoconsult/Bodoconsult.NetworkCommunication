// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.EnumAndStates;

/// <summary>
/// Default trace codes
/// </summary>
public static class TraceCodes
{
    // Range 1000: SDCP comm

    public static int IdsMsgSdcpOrderOk => 1000;
    public static int IdsMsgSdcpOrderFails => 1001;

    // Range 2000: EDCP comm

    public static int IdsMsgEdcpOrderOk => 2000;
    public static int IdsMsgEdcpOrderFails => 2001;

    // Range 3000: BTCP comm

    public static int IdsMsgBtcpOrderOk => 3000;
    public static int IdsMsgBtcpOrderFails => 3001;

    // Range 4000: telnet style comm

    public static int IdsMsgTelnetOrderOk => 4000;
    public static int IdsMsgTelnetOrderFails => 4001;

}