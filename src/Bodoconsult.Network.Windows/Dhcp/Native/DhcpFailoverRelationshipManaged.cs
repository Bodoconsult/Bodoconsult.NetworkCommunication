// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_FAILOVER_RELATIONSHIP structure defines information about a DHCPv4 server failover relationship.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal readonly struct DhcpFailoverRelationshipManaged : IDisposable
{
    /// <summary>
    /// This member specifies the IPv4 address of the primary server in the failover relationship.
    /// </summary>
    public readonly DhcpIpAddress PrimaryServer;
    /// <summary>
    /// This member specifies the IPv4 address of the secondary server in the failover relationship.
    /// </summary>
    public readonly DhcpIpAddress SecondaryServer;
    /// <summary>
    /// This member specifies the mode of the failover relationship.
    /// </summary>
    public readonly DhcpFailoverMode Mode;
    /// <summary>
    /// This member is specifies the type of failover server.
    /// </summary>
    public readonly DhcpFailoverServer ServerType;
    /// <summary>
    /// This member specifies the state of the failover relationship.
    /// </summary>
    public readonly FsmState State;
    /// <summary>
    /// This member specifies the previous state of the failover relationship.
    /// </summary>
    public readonly FsmState PrevState;
    /// <summary>
    /// This member defines the maximum client lead time (MCLT) of the failover relationship, in seconds.
    /// </summary>
    public readonly int Mclt;
    /// <summary>
    /// This member specifies a safe period time in seconds, that the DHCPv4 server will wait before transitioning the server from the COMMUNICATION-INT state to PARTNER-DOWN state, as described in [IETF-DHCPFOP-12], section 10.
    /// </summary>
    public readonly int SafePeriod;
    /// <summary>
    /// This member is a pointer to a null-terminated Unicode string containing the name of the failover relationship that uniquely identifies a failover relationship. There is no restriction on the length of this Unicode string.
    /// </summary>
    public readonly string RelationshipName;
    /// <summary>
    /// This member is a pointer to a null-terminated Unicode string containing the host name of the primary server in the failover relationship. There is no restriction on the length of this Unicode string.
    /// </summary>
    public readonly string PrimaryServerName;
    /// <summary>
    /// This member is a pointer to a null-terminated Unicode string containing the host name of the secondary server in the failover relationship. There is no restriction on the length of this Unicode string.
    /// </summary>
    public readonly string SecondaryServerName;
    /// <summary>
    /// This member is a pointer of type LPDHCP_IP_ARRAY, which contains the list of IPv4 subnet addresses that are part of the failover relationship.
    /// </summary>
    private readonly IntPtr ScopesPointer;
    /// <summary>
    /// This member indicates the ratio of the DHCPv4 client load shared between a primary and secondary server in the failover relationship.
    /// </summary>
    public readonly byte Percentage;
    /// <summary>
    /// This member is a pointer to a null-terminated Unicode string containing the shared secret key associated with this failover relationship. There is no restriction on the length of this string.
    /// </summary>
    public readonly string SharedSecret;

    /// <summary>
    /// This member is a pointer of type LPDHCP_IP_ARRAY, which contains the list of IPv4 subnet addresses that are part of the failover relationship.
    /// </summary>
    public DhcpIpArrayManaged Scopes => ScopesPointer.MarshalToStructure<DhcpIpArrayManaged>();

    public DhcpFailoverRelationshipManaged(DhcpIpAddress primaryServer, DhcpIpAddress secondaryServer, DhcpFailoverMode mode, DhcpFailoverServer serverType, FsmState state, FsmState prevState, int mclt, int safePeriod, string relationshipName, string primaryServerName, string secondaryServerName, IntPtr scopesPointer, byte percentage, string sharedSecret)
    {
        PrimaryServer = primaryServer;
        SecondaryServer = secondaryServer;
        Mode = mode;
        ServerType = serverType;
        State = state;
        PrevState = prevState;
        Mclt = mclt;
        SafePeriod = safePeriod;
        RelationshipName = relationshipName;
        PrimaryServerName = primaryServerName;
        SecondaryServerName = secondaryServerName;
        ScopesPointer = scopesPointer;
        Percentage = percentage;
        SharedSecret = sharedSecret;
    }

    public DhcpFailoverRelationshipManaged(DhcpIpAddress primaryServer, DhcpIpAddress secondaryServer, DhcpFailoverMode mode, DhcpFailoverServer serverType, FsmState state, FsmState prevState, int mclt, int safePeriod, string relationshipName, string primaryServerName, string secondaryServerName, DhcpIpArrayManaged scopes, byte percentage, string sharedSecret)
    {
        PrimaryServer = primaryServer;
        SecondaryServer = secondaryServer;
        Mode = mode;
        ServerType = serverType;
        State = state;
        PrevState = prevState;
        Mclt = mclt;
        SafePeriod = safePeriod;
        RelationshipName = relationshipName;
        PrimaryServerName = primaryServerName;
        SecondaryServerName = secondaryServerName;
        Percentage = percentage;
        SharedSecret = sharedSecret;

        ScopesPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DhcpIpArrayManaged)));
        Marshal.StructureToPtr(scopes, ScopesPointer, false);
    }

    /// <summary>
    /// Constructor to support DhcpV4FailoverAddScopeToRelationship
    /// </summary>
    public DhcpFailoverRelationshipManaged(string relationshipName, DhcpIpArrayManaged scopes)
    {
        PrimaryServer = (DhcpIpAddress)0;
        SecondaryServer = (DhcpIpAddress)0;
        Mode = (DhcpFailoverMode)(-1);
        ServerType = (DhcpFailoverServer)(-1);
        State = (FsmState)(-1);
        PrevState = (FsmState)(-1);
        Mclt = -1;
        SafePeriod = -1;
        RelationshipName = relationshipName;
        PrimaryServerName = null;
        SecondaryServerName = null;
        Percentage = 0xFF;
        SharedSecret = null;

        ScopesPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(DhcpIpArrayManaged)));
        Marshal.StructureToPtr(scopes, ScopesPointer, false);
    }

    public DhcpFailoverRelationshipManaged InvertRelationship()
    {
        return new DhcpFailoverRelationshipManaged(primaryServer: PrimaryServer,
            secondaryServer: SecondaryServer,
            mode: Mode,
            serverType: ServerType == DhcpFailoverServer.PrimaryServer ? DhcpFailoverServer.SecondaryServer : DhcpFailoverServer.PrimaryServer,
            state: State,
            prevState: PrevState,
            mclt: Mclt,
            safePeriod: SafePeriod,
            relationshipName: RelationshipName,
            primaryServerName: PrimaryServerName,
            secondaryServerName: SecondaryServerName,
            scopesPointer: ScopesPointer,
            percentage: Percentage,
            sharedSecret: SharedSecret);
    }

    public void Dispose()
    {
        if (ScopesPointer != IntPtr.Zero)
            Scopes.Dispose();
    }
}
