// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

public class DhcpServerScopeReservation : IDhcpServerScopeReservation
{
    public DhcpServer Server => Scope.Server;
    IDhcpServer IDhcpServerScopeReservation.Server => Server;
    public DhcpServerScope Scope { get; }
    IDhcpServerScope IDhcpServerScopeReservation.Scope => Scope;

    private IDhcpServerClient _client;
    public DhcpServerIpAddress Address { get; }
    public DhcpServerHardwareAddress HardwareAddress { get; }

    public DhcpServerClientTypes AllowedClientTypes { get; }

    public IDhcpServerClient Client => _client ??= DhcpServerClient.GetClient(Server, Scope, Address);

    public IDhcpServerScopeReservationOptionValueCollection Options { get; }

    private DhcpServerScopeReservation(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, DhcpServerClientTypes allowedClientTypes)
    {
        Scope = scope;
        Address = address;
        HardwareAddress = hardwareAddress;
        AllowedClientTypes = allowedClientTypes;

        Options = new DhcpServerScopeReservationOptionValueCollection(this);
    }

    public void Delete()
        => DhcpServerScope.RemoveSubnetReservationElement(Server, Address, HardwareAddress);

    internal static DhcpServerScopeReservation CreateReservation(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress)
        => CreateReservation(scope, address, hardwareAddress, DhcpServerClientTypes.DhcpAndBootp);
    internal static DhcpServerScopeReservation CreateReservation(DhcpServerScope scope, DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, DhcpServerClientTypes allowedClientTypes)
    {
        if (!scope.IpRange.Contains(address))
            throw new ArgumentOutOfRangeException(nameof(address), "The DHCP scope does not include the provided address");

        DhcpServerScope.AddSubnetReservationElement(scope.Server, scope.Address, address, hardwareAddress, allowedClientTypes);

        return new DhcpServerScopeReservation(scope, address, hardwareAddress, allowedClientTypes);
    }

    internal static IEnumerable<DhcpServerScopeReservation> GetReservations(DhcpServerScope scope)
    {
        var resumeHandle = IntPtr.Zero;
        var result = Api.DhcpEnumSubnetElementsV5(serverIpAddress: scope.Server.Address,
            subnetAddress: scope.Address.ToNativeAsNetwork(),
            enumElementType: DhcpSubnetElementType.DhcpReservedIps,
            resumeHandle: ref resumeHandle,
            preferredMaximum: 0xFFFFFFFF,
            enumElementInfo: out var reservationsPtr,
            elementsRead: out var elementsRead,
            elementsTotal: out _);

        if (result == DhcpErrors.ErrorNoMoreItems || result == DhcpErrors.EptSNotRegistered)
            yield break;

        if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData)
            throw new DhcpServerException(nameof(Api.DhcpEnumSubnetElementsV5), result);

        try
        {
            if (elementsRead == 0)
                yield break;

            using (var reservations = reservationsPtr.MarshalToStructure<DhcpSubnetElementInfoArrayV5>())
            {
                foreach (var element in reservations.Elements)
                {
                    var elementIp = element.ReadReservedIp();
                    yield return FromNative(scope, in elementIp);
                }
            }
        }
        finally
        {
            Api.FreePointer(reservationsPtr);
        }
    }

    private static DhcpServerScopeReservation FromNative(DhcpServerScope scope, in DhcpIpReservationV4 native)
    {
        var reservedForClient = native.ReservedForClient;

        return new DhcpServerScopeReservation(scope: scope,
            address: native.ReservedIpAddress.AsNetworkToIpAddress(),
            hardwareAddress: reservedForClient.ClientHardwareAddress,
            allowedClientTypes: (DhcpServerClientTypes)native.AllowedClientTypes);
    }

    public override string ToString() => $"{Address} [{HardwareAddress}] ({AllowedClientTypes})";
}