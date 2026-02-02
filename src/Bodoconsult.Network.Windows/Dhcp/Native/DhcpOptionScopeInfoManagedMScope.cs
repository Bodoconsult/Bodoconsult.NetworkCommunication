// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpOptionScopeInfoManagedMScope
{
    private readonly IntPtr scopeType;

    /// <summary>
    /// <see cref="DhcpOptionScopeType"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
    /// </summary>
    public DhcpOptionScopeType ScopeType => (DhcpOptionScopeType)scopeType;

    /// <summary>
    /// Pointer to a Unicode string that contains the multicast scope name (usually represented as the IP address of the multicast router).
    /// </summary>
    [MarshalAs(UnmanagedType.LPWStr)]
    public readonly string MScopeInfo;

    public DhcpOptionScopeInfoManagedMScope(string mScopeInfo)
    {
        scopeType = (IntPtr)DhcpOptionScopeType.DhcpMScopeOptions;
        MScopeInfo = mScopeInfo;
    }
}
