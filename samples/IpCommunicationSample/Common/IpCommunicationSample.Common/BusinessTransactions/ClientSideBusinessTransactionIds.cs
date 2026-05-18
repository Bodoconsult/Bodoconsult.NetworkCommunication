// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace IpCommunicationSample.Common.BusinessTransactions;

/// <summary>
/// Client side business transaction IDs
/// </summary>
public static class ClientSideBusinessTransactionIds
{
    public static int GetConfig => 200;
    public static int StartMessaging => 201;
    public static int StopMessaging => 202;

    public static int CreateFftAnalysisReport => 250;
}