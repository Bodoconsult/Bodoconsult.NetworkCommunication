// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_ALL_OPTIONS structure defines the set of all options available on a DHCP server.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpAllOptions : IDisposable
{
    /// <summary>
    /// Reserved. This value should be set to 0.
    /// </summary>
    public readonly int Flags;
    /// <summary>
    /// DHCP_OPTION_ARRAY structure that contains the set of non-vendor options.
    /// </summary>
    private readonly IntPtr NonVendorOptionsPointer;
    /// <summary>
    /// Specifies the number of vendor options listed in VendorOptions.
    /// </summary>
    public readonly int NumVendorOptions;
    /// <summary>
    /// Pointer to a list of structures that contain the following fields.
    /// - Option: DHCP_OPTION structure that contains specific information describing the option.
    /// - VendorName: Unicode string that contains the name of the vendor for the option.
    /// - ClassName: Unicode string that contains the name of the DHCP class for the option.
    /// </summary>
    private readonly IntPtr VendorOptionsPointer;

    /// <summary>
    /// DHCP_OPTION_ARRAY structure that contains the set of non-vendor options.
    /// </summary>
    public DhcpOptionArray NonVendorOptions => NonVendorOptionsPointer.MarshalToStructure<DhcpOptionArray>();

    /// <summary>
    /// Pointer to a list of <see cref="DhcpVendorOption"/> structures containing DHCP server options and the associated data.
    /// </summary>
    public IEnumerable<DhcpVendorOption> VendorOptions
    {
        get
        {
            if (NumVendorOptions == 0 || VendorOptionsPointer == IntPtr.Zero)
                yield break;

            var size = Marshal.SizeOf(typeof(DhcpVendorOption));
            var iter = VendorOptionsPointer;
            for (var i = 0; i < NumVendorOptions; i++)
            {
                yield return iter.MarshalToStructure<DhcpVendorOption>();
                iter += size;
            }
        }
    }

    public void Dispose()
    {
        foreach (var option in VendorOptions)
            option.Dispose();
        Api.FreePointer(VendorOptionsPointer);

        if (NonVendorOptionsPointer != IntPtr.Zero)
        {
            NonVendorOptions.Dispose();
            Api.FreePointer(NonVendorOptionsPointer);
        }
    }
}
