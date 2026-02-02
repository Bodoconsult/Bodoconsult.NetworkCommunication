// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_OPTION_VALUE structure defines a DHCP option value (just the option data with an associated ID tag).
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpOptionValue : IDisposable
{
    /// <summary>
    /// DHCP_OPTION_ID value that specifies a unique ID number for the option.
    /// </summary>
    public readonly int OptionID;
    /// <summary>
    /// DHCP_OPTION_DATA structure that contains the data for a DHCP server option.
    /// </summary>
    public readonly DhcpOptionData Value;

    public void Dispose()
    {
        Value.Dispose();
    }
}
