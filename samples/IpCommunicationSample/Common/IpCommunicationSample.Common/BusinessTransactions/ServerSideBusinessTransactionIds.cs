// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace IpCommunicationSample.Common.BusinessTransactions;

/// <summary>
/// Server side business transaction IDs
/// </summary>
public static class ServerSideBusinessTransactionIds
{
    public static int NotificationFired => 100;

    public static int ReportDeviceError = 101;
    public static int ReportFftData = 102;

    public const int SendClientHello = 205;

    public const int CheckConnection = 206;

    public const int LoadStreamConfig = 207;

    public const int StartDataLogging = 208;

    public const int StopDataLogging = 209;

    public const int FlushDataLoggers = 210;

    public const int StartDataLoggers = 211;
    public const int StopDataLoggers = 212;

    public const int StartDataCollector = 213;
    public const int StopDataCollector = 214;
}