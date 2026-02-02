// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.Network.Windows.Dhcp.Native;
using DateTime = System.DateTime;

namespace Bodoconsult.Network.Windows.Dhcp;

public class DhcpServerClient : IDhcpServerClient
{
    public DhcpServer Server { get; }
    IDhcpServer IDhcpServerClient.Server => Server;
    public DhcpServerScope Scope { get; }
    IDhcpServerScope IDhcpServerClient.Scope => Scope;

    public DhcpServerIpAddress IpAddress { get; }
    private ClientInfo _info;

    public DhcpServerIpMask SubnetMask => _info.SubnetMask;
    public DhcpServerHardwareAddress HardwareAddress => _info.HardwareAddress;

    public string Name
    {
        get => _info.Name;
        set => SetName(value);
    }
    public string Comment
    {
        get => _info.Comment;
        set => SetComment(value);
    }

    [Obsolete($"Caused confusion. Use {nameof(LeaseExpiresUtc)} or {nameof(LeaseExpiresLocal)} instead.")]
    public DateTime LeaseExpires => _info.LeaseExpiresUtc;
    public DateTime LeaseExpiresUtc => _info.LeaseExpiresUtc;
    public DateTime LeaseExpiresLocal => _info.LeaseExpiresUtc.ToLocalTime();

    public bool LeaseExpired => DateTime.UtcNow > _info.LeaseExpiresUtc;
    public bool LeaseHasExpiry => _info.LeaseExpiresUtc != DateTime.MaxValue;

    public DhcpServerClientTypes Type => _info.Type;

    public DhcpServerClientAddressStates AddressState => _info.AddressState;
    public DhcpServerClientNameProtectionStates NameProtectionState => _info.NameProtectionState;
    public DhcpServerClientDnsStates DnsState => _info.DnsState;

    public DhcpServerClientQuarantineStatuses QuarantineStatus => _info.QuarantineStatus;

    public DateTime ProbationEnds => _info.ProbationEnds;

    public bool QuarantineCapable => _info.QuarantineCapable;

    private DhcpServerClient(DhcpServer server, DhcpServerScope scope, DhcpServerIpAddress ipAddress, ClientInfo info)
    {
        Server = server;
        Scope = scope;
        IpAddress = ipAddress;
        _info = info;
    }

    public void Delete()
        => Delete(Server, IpAddress);

    public IDhcpServerScopeReservation ConvertToReservation()
    {
        if (Scope == null)
            throw new NullReferenceException("This client is not associated with a DHCP scope");

        return Scope.Reservations.AddReservation(this);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrEmpty(name))
            name = string.Empty;

        if (!name.Equals(_info.Name, StringComparison.Ordinal))
        {
            var proposedInfo = _info.UpdateName(name);
            SetInfo(proposedInfo);
        }
    }

    private void SetComment(string comment)
    {
        if (string.IsNullOrEmpty(comment))
            comment = string.Empty;

        if (!comment.Equals(_info.Comment, StringComparison.Ordinal))
        {
            var proposedInfo = _info.UpdateComment(comment);
            SetInfo(proposedInfo);
        }
    }

    internal static DhcpServerClient GetClient(DhcpServer server, DhcpServerScope scope, DhcpServerIpAddress ipAddress)
    {
        var searchInfo = new DhcpSearchInfoManagedIpAddress(ipAddress.ToNativeAsNetwork());

        using (var searchInfoPtr = BitHelper.StructureToPtr(searchInfo))
        {
            if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
                return GetClientVq(server, scope, searchInfoPtr);
            else if (server.IsCompatible(DhcpServerVersions.Windows2000))
                return GetClientV0(server, scope, searchInfoPtr);
            else
                throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");
        }
    }

    private static DhcpServerClient GetClientV0(DhcpServer server, DhcpServerScope scope, IntPtr searchInfo)
    {
        var result = Api.DhcpGetClientInfo(server.Address, searchInfo, out var clientPtr);

        if (result == DhcpErrors.JetError)
            return null;

        if (result != DhcpErrors.Success)
            throw new DhcpServerException(nameof(Api.DhcpGetClientInfo), result);

        try
        {
            using (var client = clientPtr.MarshalToStructure<DhcpClientInfo>())
            {
                return FromNative(server, scope, in client);
            }
        }
        finally
        {
            Api.FreePointer(clientPtr);
        }
    }

    private static DhcpServerClient GetClientVq(DhcpServer server, DhcpServerScope scope, IntPtr searchInfo)
    {
        var result = Api.DhcpGetClientInfoVQ(server.Address, searchInfo, out var clientPtr);

        if (result == DhcpErrors.JetError)
            return null;

        if (result != DhcpErrors.Success)
            throw new DhcpServerException(nameof(Api.DhcpGetClientInfoVQ), result);

        try
        {
            using (var client = clientPtr.MarshalToStructure<DhcpClientInfoVq>())
            {
                return FromNative(server, scope, in client);
            }
        }
        finally
        {
            Api.FreePointer(clientPtr);
        }
    }

    internal static IEnumerable<DhcpServerClient> GetClients(DhcpServer server)
    {
        if (server.IsCompatible(DhcpServerVersions.Windows2008R2))
            return GetClientsVq(server);
        else if (server.IsCompatible(DhcpServerVersions.Windows2000))
            return GetClientsV0(server);
        else
            throw new PlatformNotSupportedException($"DHCP Server v{server.VersionMajor}.{server.VersionMinor} does not support this feature");
    }

    private static IEnumerable<DhcpServerClient> GetClientsV0(DhcpServer server)
    {
        var scopeLookup = server.Scopes.ToDictionary(s => s.Address);
        var resultHandle = IntPtr.Zero;
        var result = Api.DhcpEnumSubnetClients(serverIpAddress: server.Address,
            subnetAddress: (DhcpIpAddress)0,
            resumeHandle: ref resultHandle,
            preferredMaximum: 0x10000,
            clientInfo: out var clientsPtr,
            clientsRead: out _,
            clientsTotal: out _);

        // shortcut if no subnet clients are returned
        if (result == DhcpErrors.ErrorNoMoreItems)
            yield break;

        if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData)
            throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClients), result);

        try
        {
            while (result == DhcpErrors.Success || result == DhcpErrors.ErrorMoreData)
            {
                using (var clients = clientsPtr.MarshalToStructure<DhcpClientInfoArray>())
                {
                    foreach (var client in clients.Clients)
                    {
                        var clientValue = client.Value;
                        var scope = scopeLookup.TryGetValue(clientValue.SubnetAddress, out var s) ? s : null;
                        yield return FromNative(server, (DhcpServerScope)scope, in clientValue);
                    }
                }

                if (result == DhcpErrors.Success)
                    yield break; // Last results

                Api.FreePointer(clientsPtr);
                result = Api.DhcpEnumSubnetClients(serverIpAddress: server.Address,
                    subnetAddress: (DhcpIpAddress)0,
                    resumeHandle: ref resultHandle,
                    preferredMaximum: 0x10000,
                    clientInfo: out clientsPtr,
                    clientsRead: out _,
                    clientsTotal: out _);

                if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData && result != DhcpErrors.ErrorNoMoreItems)
                    throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClients), result);
            }
        }
        finally
        {
            Api.FreePointer(clientsPtr);
        }
    }

    private static IEnumerable<DhcpServerClient> GetClientsVq(DhcpServer server)
    {
        var scopeLookup = server.Scopes.ToDictionary(s => s.Address);
        var resultHandle = IntPtr.Zero;
        var result = Api.DhcpEnumSubnetClientsVQ(serverIpAddress: server.Address,
            subnetAddress: (DhcpIpAddress)0,
            resumeHandle: ref resultHandle,
            preferredMaximum: 0x10000,
            clientInfo: out var clientsPtr,
            clientsRead: out _,
            clientsTotal: out _);

        // shortcut if no subnet clients are returned
        if (result == DhcpErrors.ErrorNoMoreItems)
            yield break;

        if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData)
            throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsVQ), result);

        try
        {
            while (result == DhcpErrors.Success || result == DhcpErrors.ErrorMoreData)
            {
                using (var clients = clientsPtr.MarshalToStructure<DhcpClientInfoArrayVq>())
                {
                    foreach (var client in clients.Clients)
                    {
                        var clientValue = client.Value;
                        var scope = scopeLookup.TryGetValue(clientValue.SubnetAddress, out var s) ? s : null;
                        yield return FromNative(server, (DhcpServerScope)scope, in clientValue);
                    }
                }

                if (result == DhcpErrors.Success)
                    yield break; // Last results

                Api.FreePointer(clientsPtr);
                result = Api.DhcpEnumSubnetClientsVQ(serverIpAddress: server.Address,
                    subnetAddress: (DhcpIpAddress)0,
                    resumeHandle: ref resultHandle,
                    preferredMaximum: 0x10000,
                    clientInfo: out clientsPtr,
                    clientsRead: out _,
                    clientsTotal: out _);

                if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData)
                    throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsVQ), result);
            }
        }
        finally
        {
            Api.FreePointer(clientsPtr);
        }
    }

    internal static IEnumerable<DhcpServerClient> GetScopeClients(DhcpServerScope scope)
    {
        if (scope.Server.IsCompatible(DhcpServerVersions.Windows2008R2))
            return GetScopeClientsVq(scope);
        else if (scope.Server.IsCompatible(DhcpServerVersions.Windows2000))
            return GetScopeClientsV0(scope);
        else
            throw new PlatformNotSupportedException($"DHCP Server v{scope.Server.VersionMajor}.{scope.Server.VersionMinor} does not support this feature");
    }

    private static IEnumerable<DhcpServerClient> GetScopeClientsV0(DhcpServerScope scope)
    {
        var resultHandle = IntPtr.Zero;
        var result = Api.DhcpEnumSubnetClients(serverIpAddress: scope.Server.Address,
            subnetAddress: scope.Address.ToNativeAsNetwork(),
            resumeHandle: ref resultHandle,
            preferredMaximum: 0x10000,
            clientInfo: out var clientsPtr,
            clientsRead: out _,
            clientsTotal: out _);

        // shortcut if no subnet clients are returned
        if (result == DhcpErrors.ErrorNoMoreItems)
            yield break;

        if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData)
            throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClients), result);

        try
        {
            while (result == DhcpErrors.Success || result == DhcpErrors.ErrorMoreData)
            {
                using (var clients = clientsPtr.MarshalToStructure<DhcpClientInfoArray>())
                {
                    foreach (var client in clients.Clients)
                        yield return FromNative(scope.Server, scope, in client.Value);
                }

                if (result == DhcpErrors.Success)
                    yield break; // Last results

                Api.FreePointer(clientsPtr);
                result = Api.DhcpEnumSubnetClients(serverIpAddress: scope.Server.Address,
                    subnetAddress: scope.Address.ToNativeAsNetwork(),
                    resumeHandle: ref resultHandle,
                    preferredMaximum: 0x10000,
                    clientInfo: out clientsPtr,
                    clientsRead: out _,
                    clientsTotal: out _);

                if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData && result != DhcpErrors.ErrorNoMoreItems)
                    throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClients), result);
            }
        }
        finally
        {
            Api.FreePointer(clientsPtr);
        }
    }

    private static IEnumerable<DhcpServerClient> GetScopeClientsVq(DhcpServerScope scope)
    {
        var resultHandle = IntPtr.Zero;
        var result = Api.DhcpEnumSubnetClientsVQ(serverIpAddress: scope.Server.Address,
            subnetAddress: scope.Address.ToNativeAsNetwork(),
            resumeHandle: ref resultHandle,
            preferredMaximum: 0x10000,
            clientInfo: out var clientsPtr,
            clientsRead: out _,
            clientsTotal: out _);

        // shortcut if no subnet clients are returned
        if (result == DhcpErrors.ErrorNoMoreItems)
            yield break;

        if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData)
            throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsVQ), result);

        try
        {
            while (result == DhcpErrors.Success || result == DhcpErrors.ErrorMoreData)
            {
                using (var clients = clientsPtr.MarshalToStructure<DhcpClientInfoArrayVq>())
                {
                    foreach (var client in clients.Clients)
                        yield return FromNative(scope.Server, scope, in client.Value);
                }

                if (result == DhcpErrors.Success)
                    yield break; // Last results

                Api.FreePointer(clientsPtr);
                result = Api.DhcpEnumSubnetClientsVQ(serverIpAddress: scope.Server.Address,
                    subnetAddress: scope.Address.ToNativeAsNetwork(),
                    resumeHandle: ref resultHandle,
                    preferredMaximum: 0x10000,
                    clientInfo: out clientsPtr,
                    clientsRead: out _,
                    clientsTotal: out _);

                if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData)
                    throw new DhcpServerException(nameof(Api.DhcpEnumSubnetClientsVQ), result);
            }
        }
        finally
        {
            Api.FreePointer(clientsPtr);
        }
    }

    internal static DhcpServerClient CreateClient(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress)
        => CreateClientV0(scope, address, hardwareAddress, name: null, comment: null, leaseExpires: DateTime.MaxValue, ownerHost: null);

    internal static DhcpServerClient CreateClient(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name)
        => CreateClientV0(scope, address, hardwareAddress, name, comment: null, leaseExpires: DateTime.MaxValue, ownerHost: null);

    internal static DhcpServerClient CreateClient(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment)
        => CreateClientV0(scope, address, hardwareAddress, name, comment, leaseExpires: DateTime.MaxValue, ownerHost: null);

    internal static DhcpServerClient CreateClient(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires)
        => CreateClientV0(scope, address, hardwareAddress, name, comment, leaseExpires, ownerHost: null);

    internal static DhcpServerClient CreateClient(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, DhcpServerHost ownerHost)
        => CreateClientV0(scope, address, hardwareAddress, name, comment, leaseExpires, ownerHost);

    internal static DhcpServerClient CreateClient(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, DhcpServerHost ownerHost, DhcpServerClientTypes clientType, DhcpServerClientAddressStates addressState, DhcpServerClientQuarantineStatuses quarantineStatus, DateTime probationEnds, bool quarantineCapable)
    {
        if (scope.Server.IsCompatible(DhcpServerVersions.Windows2008R2))
            return CreateClientVq(scope, address, hardwareAddress, name, comment, leaseExpires, ownerHost, clientType, addressState, quarantineStatus, probationEnds, quarantineCapable);
        else
            return CreateClientV0(scope, address, hardwareAddress, name, comment, leaseExpires, ownerHost);
    }

    private static DhcpServerClient CreateClientV0(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, DhcpServerHost ownerHost)
    {
        if (!scope.IpRange.Contains(address))
            throw new ArgumentOutOfRangeException(nameof(address), "The address is not within the IP range of the scope");

        using (var clientInfo = new DhcpClientInfoManaged(clientIpAddress: address.ToNativeAsNetwork(),
                   subnetMask: scope.Mask.ToNativeAsNetwork(),
                   clientHardwareAddress: hardwareAddress.ToNativeBinaryData(),
                   clientName: name,
                   clientComment: comment,
                   clientLeaseExpires: leaseExpires,
                   ownerHost: (ownerHost ?? DhcpServerHost.Empty).ToNative()))
        {
            using (var clientInfoPtr = BitHelper.StructureToPtr(clientInfo))
            {
                var result = Api.DhcpCreateClientInfo(serverIpAddress: scope.Server.Address, clientInfo: clientInfoPtr);

                if (result != DhcpErrors.Success)
                    throw new DhcpServerException(nameof(Api.DhcpCreateClientInfo), result);
            }
        }

        return GetClient(scope.Server, scope, address);
    }

    private static DhcpServerClient CreateClientVq(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, DhcpServerHost ownerHost, DhcpServerClientTypes clientType, DhcpServerClientAddressStates addressState, DhcpServerClientQuarantineStatuses quarantineStatus, DateTime probationEnds, bool quarantineCapable)
    {
        if (!scope.IpRange.Contains(address))
            throw new ArgumentOutOfRangeException(nameof(address), "The address is not within the IP range of the scope");

        using (var clientInfo = new DhcpClientInfoVqManaged(clientIpAddress: address.ToNativeAsNetwork(),
                   subnetMask: scope.Mask.ToNativeAsNetwork(),
                   clientHardwareAddress: hardwareAddress.ToNativeBinaryData(),
                   clientName: name,
                   clientComment: comment,
                   clientLeaseExpires: leaseExpires,
                   ownerHost: (ownerHost ?? DhcpServerHost.Empty).ToNative(),
                   (DhcpClientType)clientType,
                   (byte)addressState,
                   (QuarantineStatus)quarantineStatus,
                   probationEnds,
                   quarantineCapable))
        {
            using (var clientInfoPtr = BitHelper.StructureToPtr(clientInfo))
            {
                var result = Api.DhcpCreateClientInfo(serverIpAddress: scope.Server.Address, clientInfo: clientInfoPtr);

                if (result != DhcpErrors.Success)
                    throw new DhcpServerException(nameof(Api.DhcpCreateClientInfo), result);
            }
        }

        return GetClient(scope.Server, scope, address);
    }

    private void SetInfo(ClientInfo info)
    {
        // only supporting v0 at this stage
        SetInfoV0(info);

        // update cache
        _info = info;
    }

    private void SetInfoV0(ClientInfo info)
    {
        using (var infoNative = info.ToNativeV0())
        {
            var result = Api.DhcpSetClientInfo(serverIpAddress: Server.Address,
                clientInfo: in infoNative);

            if (result != DhcpErrors.Success)
                throw new DhcpServerException(nameof(Api.DhcpSetClientInfo), result);
        }
    }

    private static void Delete(DhcpServer server, DhcpServerIpAddress address)
    {
        var searchInfo = new DhcpSearchInfoManagedIpAddress(address.ToNativeAsNetwork());

        using (var searchInfoPtr = BitHelper.StructureToPtr(searchInfo))
        {
            var result = Api.DhcpDeleteClientInfo(server.Address, searchInfoPtr);

            if (result != DhcpErrors.Success)
                throw new DhcpServerException(nameof(Api.DhcpDeleteClientInfo), result);
        }
    }

    private static DhcpServerClient FromNative(DhcpServer server, DhcpServerScope scope, in DhcpClientInfo native)
    {
        var info = ClientInfo.FromNative(in native);

        return new DhcpServerClient(server: server,
            scope: scope,
            ipAddress: native.ClientIpAddress.AsNetworkToIpAddress(),
            info: info);
    }

    private static DhcpServerClient FromNative(DhcpServer server, DhcpServerScope scope, in DhcpClientInfoVq native)
    {
        var info = ClientInfo.FromNative(in native);

        return new DhcpServerClient(server: server,
            scope: scope,
            ipAddress: native.ClientIpAddress.AsNetworkToIpAddress(),
            info: info);
    }

    public override string ToString()
    {
        return $"[{HardwareAddress}] {AddressState} {IpAddress}/{SubnetMask.SignificantBits} ({(LeaseHasExpiry ? (LeaseExpired ? "expired" : LeaseExpiresLocal.ToString()) : "no expiry")}): {Name}{(string.IsNullOrEmpty(Comment) ? null : $" ({Comment})")}";
    }

    private class ClientInfo
    {
        public readonly DhcpIpAddress Address;
        public readonly DhcpServerIpMask SubnetMask;
        public readonly DhcpServerHardwareAddress HardwareAddress;
        public readonly string Name;
        public readonly string Comment;
        public readonly DateTime LeaseExpiresUtc;
        public readonly DhcpServerHost OwnerHost;
        public readonly DhcpServerClientTypes Type;
        public readonly DhcpServerClientAddressStates AddressState;
        public readonly DhcpServerClientNameProtectionStates NameProtectionState;
        public readonly DhcpServerClientDnsStates DnsState;
        public readonly DhcpServerClientQuarantineStatuses QuarantineStatus;
        public readonly DateTime ProbationEnds;
        public readonly bool QuarantineCapable;

        public ClientInfo(DhcpIpAddress address, DhcpServerIpMask subnetMask, DhcpServerHardwareAddress hardwareAddress, string name, string comment,
            DateTime leaseExpiresUtc, DhcpServerHost ownerHost, DhcpServerClientTypes type, DhcpServerClientAddressStates addressState,
            DhcpServerClientNameProtectionStates nameProtectionState, DhcpServerClientDnsStates dnsState, DhcpServerClientQuarantineStatuses quarantineStatus,
            DateTime probationEnds, bool quarantineCapable)
        {
            Address = address;
            SubnetMask = subnetMask;
            HardwareAddress = hardwareAddress;
            Name = name;
            Comment = comment;
            LeaseExpiresUtc = leaseExpiresUtc;
            OwnerHost = ownerHost;
            Type = type;
            AddressState = addressState;
            NameProtectionState = nameProtectionState;
            DnsState = dnsState;
            QuarantineStatus = quarantineStatus;
            ProbationEnds = probationEnds;
            QuarantineCapable = quarantineCapable;
        }

        public static ClientInfo FromNative(in DhcpClientInfo native)
        {
            return new ClientInfo(address: native.ClientIpAddress,
                subnetMask: native.SubnetMask.AsNetworkToIpMask(),
                hardwareAddress: native.ClientHardwareAddress.DataAsHardwareAddress,
                name: native.ClientName,
                comment: native.ClientComment,
                leaseExpiresUtc: ((DateTime)native.ClientLeaseExpires).ToUniversalTime(),
                ownerHost: DhcpServerHost.FromNative(native.OwnerHost),
                type: DhcpServerClientTypes.Unspecified,
                addressState: DhcpServerClientAddressStates.Unknown,
                nameProtectionState: DhcpServerClientNameProtectionStates.Unknown,
                dnsState: DhcpServerClientDnsStates.Unknown,
                quarantineStatus: DhcpServerClientQuarantineStatuses.NoQuarantineInformation,
                probationEnds: DateTime.MaxValue,
                quarantineCapable: false);
        }

        public static ClientInfo FromNative(in DhcpClientInfoVq native)
        {
            return new ClientInfo(address: native.ClientIpAddress,
                subnetMask: native.SubnetMask.AsNetworkToIpMask(),
                hardwareAddress: native.ClientHardwareAddress.DataAsHardwareAddress,
                name: native.ClientName,
                comment: native.ClientComment,
                leaseExpiresUtc: ((DateTime)native.ClientLeaseExpires).ToUniversalTime(),
                ownerHost: DhcpServerHost.FromNative(native.OwnerHost),
                type: (DhcpServerClientTypes)native.ClientType,
                addressState: (DhcpServerClientAddressStates)(native.AddressState & 0x03), // bits 0 & 1
                nameProtectionState: (DhcpServerClientNameProtectionStates)((native.AddressState >> 2) & 0x03), // bits 2 & 3
                dnsState: (DhcpServerClientDnsStates)((native.AddressState >> 4) & 0x0F), // bits 4-7
                quarantineStatus: (DhcpServerClientQuarantineStatuses)native.Status,
                probationEnds: native.ProbationEnds,
                quarantineCapable: native.QuarantineCapable);
        }

        public ClientInfo UpdateName(string name)
            => new(Address, SubnetMask, HardwareAddress, name, Comment, LeaseExpiresUtc, OwnerHost, Type, AddressState, NameProtectionState, DnsState, QuarantineStatus, ProbationEnds, QuarantineCapable);
        public ClientInfo UpdateComment(string comment)
            => new(Address, SubnetMask, HardwareAddress, Name, comment, LeaseExpiresUtc, OwnerHost, Type, AddressState, NameProtectionState, DnsState, QuarantineStatus, ProbationEnds, QuarantineCapable);

        public DhcpClientInfoManaged ToNativeV0()
            => new(Address, SubnetMask.ToNativeAsNetwork(), HardwareAddress.ToNativeBinaryData(), Name, Comment, LeaseExpiresUtc, OwnerHost.ToNative());
    }
}