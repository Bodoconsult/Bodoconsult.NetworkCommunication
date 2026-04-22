// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.EnumAndStates;

/// <summary>
/// Default trace codes
/// </summary>
public static class TraceCodes
{
    // Range 1000: SDCP comm

    /// <summary>
    /// Trace code for SDCP order successful
    /// </summary>
    public static int IdsMsgSdcpOrderOk => 1000;

    /// <summary>
    /// Trace code for SDCP order failed
    /// </summary>
    public static int IdsMsgSdcpOrderFails => 1001;

    // Range 2000: Tncp comm

    /// <summary>
    /// Trace code for EDCP order successful
    /// </summary>
    public static int IdsMsgEdcpOrderOk => 2000;

    /// <summary>
    /// Trace code for EDCP order failed
    /// </summary>
    public static int IdsMsgEdcpOrderFails => 2001;

    // Range 3000: BTCP comm

    /// <summary>
    /// Trace code for BTCP order successful
    /// </summary>
    public static int IdsMsgBtcpOrderOk => 3000;

    /// <summary>
    /// Trace code for BTCP order failed
    /// </summary>
    public static int IdsMsgBtcpOrderFails => 3001;

    // Range 4000: TNCP telnet style comm

    /// <summary>
    /// Trace code for TNCP order successful
    /// </summary>
    public static int IdsMsgTncpOrderOk => 4000;

    /// <summary>
    /// Trace code for TNCP order failed
    /// </summary>
    public static int IdsMsgTncpOrderFails => 4001;

    // Range 5000: Test order

    /// <summary>
    /// Trace code for test order successful
    /// </summary>
    public static int IdsMsgTestOrderOk => 5000;

    /// <summary>
    /// Trace code for test order failed
    /// </summary>
    public static int IdsMsgTestOrderFails => 5001;

    // Range 6000: Dummy order

    /// <summary>
    /// Trace code for dummy order successful
    /// </summary>
    public static int IdsMsgDummyOrderOk => 6000;

    /// <summary>
    /// Trace code for dummy order failed
    /// </summary>
    public static int IdsMsgDummyOrderFails => 6001;

    // Range 7000: Empty order

    /// <summary>
    /// Trace code for empty order successful
    /// </summary>
    public static int IdsMsgEmptyOrderOk => 7000;

    /// <summary>
    /// Trace code for empty order failed
    /// </summary>
    public static int IdsMsgEmptyOrderFails => 7001;
}