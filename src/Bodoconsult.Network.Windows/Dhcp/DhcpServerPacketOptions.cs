// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp;

public class DhcpServerPacketOptions : MarshalByRefObject
{
    private readonly Native.DhcpServerOptions _options;

    public DhcpServerPacketOptions(IntPtr pointer)
    {
        _options = pointer.MarshalToStructure<Native.DhcpServerOptions>();
    }

    /// <summary>
    /// DHCP message type.
    /// </summary>
    public DhcpServerPacketMessageTypes? MessageType => _options.MessageType == IntPtr.Zero ? null : (DhcpServerPacketMessageTypes?)Marshal.ReadByte(_options.MessageType);

    /// <summary>
    /// Subnet mask.
    /// </summary>
    public DhcpServerIpMask? SubnetMask => _options.SubnetMask == IntPtr.Zero ? (DhcpServerIpMask?)null : DhcpServerIpMask.FromNative(_options.SubnetMask);

    /// <summary>
    /// Requested IP address.
    /// </summary>
    public DhcpServerIpAddress? RequestedAddress => _options.RequestedAddress == IntPtr.Zero ? (DhcpServerIpAddress?)null : DhcpServerIpAddress.FromNative(_options.RequestedAddress);

    /// <summary>
    /// Requested duration of the IP address lease.
    /// </summary>
    public TimeSpan? RequestedLeaseTime => _options.RequestLeaseTime == IntPtr.Zero ? (TimeSpan?)null : new TimeSpan(Marshal.ReadInt32(_options.RequestLeaseTime) * 10_000_000L);

    // OverlayFields ?

    /// <summary>
    /// IP address of the default gateway.
    /// </summary>
    public DhcpServerIpAddress? RouterAddress => _options.RouterAddress == IntPtr.Zero ? (DhcpServerIpAddress?)null : DhcpServerIpAddress.FromNative(_options.RouterAddress);

    /// <summary>
    /// IP address of the DHCP Server.
    /// </summary>
    public DhcpServerIpAddress? Server => _options.Server == IntPtr.Zero ? (DhcpServerIpAddress?)null : DhcpServerIpAddress.FromNative(_options.Server);

    /// <summary>
    /// List of requested parameters.
    /// </summary>
    public DhcpServerOptionIds[] ParameterRequestList
    {
        get
        {
            if (_options.ParameterRequestListLength == 0 || _options.ParameterRequestList == IntPtr.Zero)
                return new DhcpServerOptionIds[0];

            var list = new DhcpServerOptionIds[(int)_options.ParameterRequestListLength];

            for (var i = 0; i < list.Length; i++)
                list[i] = (DhcpServerOptionIds)Marshal.ReadByte(_options.ParameterRequestList, i);

            return list;
        }
    }

    /// <summary>
    /// Machine name (host name) of the computer making the request.
    /// </summary>
    public string MachineName => _options.MachineNameLength == 0 ? string.Empty : Marshal.PtrToStringAnsi(_options.MachineName, (int)_options.MachineNameLength);

    /// <summary>
    /// Type of hardware address expressed in <see cref="ClientHardwareAddress"/>.
    /// </summary>
    public DhcpServerHardwareType ClientHardwareAddressType => _options.ClientHardwareAddressType;

    /// <summary>
    /// Client hardware address.
    /// </summary>
    public DhcpServerHardwareAddress ClientHardwareAddress
    {
        get
        {
            if (_options.ClientHardwareAddressLength == 0 ||
                _options.ClientHardwareAddressLength > DhcpServerHardwareAddress.MaximumLength)
            {
                return DhcpServerHardwareAddress.FromNative(ClientHardwareAddressType, 0, 0, 0);
            }
            else
            {
                return DhcpServerHardwareAddress.FromNative(ClientHardwareAddressType, _options.ClientHardwareAddress, _options.ClientHardwareAddressLength);
            }
        }
    }

    /// <summary>
    /// Class identifier for the client.
    /// </summary>
    public string ClassIdentifier => _options.ClassIdentifierLength == 0 ? string.Empty : Marshal.PtrToStringAnsi(_options.ClassIdentifier, (int)_options.ClassIdentifierLength);

    /// <summary>
    /// Vendor class, if applicable.
    /// </summary>
    public byte[] VendorClass
    {
        get
        {
            if (_options.VendorClassLength == 0)
            {
                return BitHelper.EmptyByteArray;
            }
            else
            {
                var buffer = new byte[_options.VendorClassLength];
                Marshal.Copy(_options.VendorClass, buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }

    /// <summary>
    /// Flags used for DNS.
    /// </summary>
    public int DnsFlags
    {
        get
        {
            return (int)_options.DNSFlags;
        }
    }

    /// <summary>
    /// The DNS name.
    /// </summary>
    public string DnsName => _options.DNSNameLength == 0 ? string.Empty : Marshal.PtrToStringAnsi(_options.DNSName, (int)_options.DNSNameLength);

    /// <summary>
    /// Specifies whether the domain name is requested.
    /// </summary>
    public bool DsDomainNameRequested
    {
        get
        {
            return _options.DSDomainNameRequested;
        }
    }

    /// <summary>
    /// The domain name.
    /// </summary>
    public string DsDomainName => _options.DSDomainNameLen == 0 ? string.Empty : Marshal.PtrToStringAnsi(_options.DSDomainName, (int)_options.DSDomainNameLen);

    /// <summary>
    /// Scope identifier for the IP address.
    /// </summary>
    public int? ScopeId => _options.ScopeId == IntPtr.Zero ? (int?)null : Marshal.ReadInt32(_options.ScopeId);

}