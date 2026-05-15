// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace IpCommunicationSample.Common.BusinessTransactions;

/// <summary>
/// Server side business transaction IDs
/// </summary>
public static class ServerSideBusinessTransactionIds
{
    public static int NotificationFired => 100;

    public static int ReportDeviceError = 101;
    
    public const int SendClientHello = 205;

    public const int CheckConnection = 206;
}