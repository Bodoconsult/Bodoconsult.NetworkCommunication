// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace IpCommunicationSample.Common.BusinessTransactions;

/// <summary>
/// Client side business transaction IDs
/// </summary>
public static class ClientSideBusinessTransactionIds
{
    public static int GetConfig => 200;
    public static int StartStreaming => 201;
    public static int StopStreaming => 202;
    public static int StartSnapshot => 203;
    public static int StopSnapshot => 204;
}