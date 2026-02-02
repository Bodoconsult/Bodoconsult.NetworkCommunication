// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

/// <summary>
/// Defines Dynamic DNS settings
/// </summary>
public class DhcpServerDnsSettings : IDhcpServerDnsSettings
{
    private const uint FlagDefaultSettings = Constants.DnsFlagEnabled | Constants.DnsFlagCleanupExpired;
    private readonly uint _flags;

    /// <summary>
    /// True when DNS dynamic updates are enabled
    /// </summary>
    public bool DynamicDnsUpdatesEnabled => (_flags & Constants.DnsFlagEnabled) == Constants.DnsFlagEnabled;

    /// <summary>
    /// True when DNS dynamic updates are enabled only when requested by the DHCP clients
    /// </summary>
    public bool DynamicDnsUpdatedOnlyWhenRequested => (_flags & Constants.DnsFlagUpdateBothAlways) == 0;

    /// <summary>
    /// True when DNS dynamic updates always update DNS records 
    /// </summary>
    public bool DynamicDnsUpdatedAlways => (_flags & Constants.DnsFlagUpdateBothAlways) == Constants.DnsFlagUpdateBothAlways;

    /// <summary>
    /// True when A and PTR records are discarded when the lease is deleted
    /// </summary>
    public bool DiscardRecordsWhenLeasesDeleted => (_flags & Constants.DnsFlagCleanupExpired) == Constants.DnsFlagCleanupExpired;

    /// <summary>
    /// True when DNS records are dynamically updated for DHCP clients that do not request updates (for example, clients running Windows NT 4.0)
    /// </summary>
    public bool UpdateRecordsForDownLevelClients => (_flags & Constants.DnsFlagUpdateDownlevel) == Constants.DnsFlagUpdateDownlevel;

    /// <summary>
    /// True when Dynamic updates for DNS PTR records are disabled
    /// </summary>
    public bool DisableDynamicPtrRecordUpdates => (_flags & Constants.DnsFlagDisablePtrUpdate) == Constants.DnsFlagDisablePtrUpdate;

    private DhcpServerDnsSettings(uint flags)
    {
        _flags = flags;
    }

    internal static DhcpServerDnsSettings GetGlobalDnsSettings(DhcpServer server)
    {
        // Flag is Global Option 81
        try
        {
            var option = DhcpServerOptionValue.GetGlobalDefaultOptionValue(server, 81);

            if (option.Values.FirstOrDefault() is DhcpServerOptionElementDWord value)
                return new DhcpServerDnsSettings((uint)value.RawValue);
            else
                return new DhcpServerDnsSettings(FlagDefaultSettings);
        }
        catch (DhcpServerException e) when (e.ApiErrorId == (uint)DhcpErrors.ErrorFileNotFound)
        {
            // Default Settings
            return new DhcpServerDnsSettings(FlagDefaultSettings);
        }
    }

    internal static DhcpServerDnsSettings GetScopeDnsSettings(DhcpServerScope scope)
        => GetScopeDnsSettings(scope.Server, scope.Address);

    internal static DhcpServerDnsSettings GetScopeDnsSettings(DhcpServer server, DhcpServerIpAddress address)
    {
        // Flag is Option 81
        try
        {
            var option = DhcpServerOptionValue.GetScopeDefaultOptionValue(server, address, 81);
            if (option.Values.FirstOrDefault() is DhcpServerOptionElementDWord value)
                return new DhcpServerDnsSettings((uint)value.RawValue);
            else
                return GetGlobalDnsSettings(server);
        }
        catch (DhcpServerException e) when (e.ApiErrorId == (uint)DhcpErrors.ErrorFileNotFound)
        {
            return GetGlobalDnsSettings(server);
        }
    }
}