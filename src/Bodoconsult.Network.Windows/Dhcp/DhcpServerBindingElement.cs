// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

public class DhcpServerBindingElement : IDhcpServerBindingElement
{
    private readonly byte[] _interfaceId;

    /// <summary>
    /// The associated DHCP Server
    /// </summary>
    public IDhcpServer Server { get; }

    /// <summary>
    /// The binding specified in this structure cannot be modified.
    /// </summary>
    public bool CantModify { get; }

    /// <summary>
    /// Specifies whether or not this binding is set on the DHCP server. If TRUE, the binding is set; if FALSE, it is not.
    /// </summary>
    public bool IsBound { get; }

    /// <summary>
    /// DHCP_IP_ADDRESS value that specifies the IP address assigned to the ethernet adapter of the DHCP server.
    /// </summary>
    public DhcpServerIpAddress AdapterPrimaryIpAddress { get; }

    /// <summary>
    /// DHCP_IP_ADDRESS value that specifies the subnet IP mask used by this ethernet adapter.
    /// </summary>
    public DhcpServerIpMask AdapterSubnetAddress { get; }

    /// <summary>
    /// Unicode string that specifies the name assigned to this network interface device.
    /// </summary>
    public string InterfaceDescription { get; }

    /// <summary>
    /// Specifies the network interface device ID.
    /// </summary>
    public byte[] InterfaceId
    {
        get
        {
            if (_interfaceId == null)
                return InterfaceGuidId.ToByteArray();
            else
                return _interfaceId;
        }
    }

    public Guid InterfaceGuidId { get; }

    private DhcpServerBindingElement(DhcpServer server, bool cantModify, bool isBound, DhcpServerIpAddress adapterPrimaryIpAddress, DhcpServerIpMask adapterSubnetAddress, string interfaceDescription, Guid interfaceGuidId, byte[] interfaceId)
    {
        Server = server;
        CantModify = cantModify;
        IsBound = isBound;
        AdapterPrimaryIpAddress = adapterPrimaryIpAddress;
        AdapterSubnetAddress = adapterSubnetAddress;
        InterfaceDescription = interfaceDescription;
        _interfaceId = interfaceId;
        InterfaceGuidId = interfaceGuidId;
    }

    internal static IEnumerable<DhcpServerBindingElement> GetBindingInfo(DhcpServer server)
    {
        var result = Api.DhcpGetServerBindingInfo(serverIpAddress: server.Address,
            flags: 0,
            bindElementsInfo: out var elementsPtr);

        if (result != DhcpErrors.Success)
            throw new DhcpServerException(nameof(Api.DhcpGetServerBindingInfo), result);

        try
        {
            using (var elements = elementsPtr.MarshalToStructure<DhcpBindElementArray>())
            {
                foreach (var element in elements.Elements)
                    yield return FromNative(server, in element);
            }
        }
        finally
        {
            Api.FreePointer(elementsPtr);
        }
    }

    private static DhcpServerBindingElement FromNative(DhcpServer server, in DhcpBindElement native)
    {
        return new DhcpServerBindingElement(server: server,
            cantModify: (native.Flags & Constants.DhcpEndpointFlagCantModify) == Constants.DhcpEndpointFlagCantModify,
            isBound: native.BoundToDHCPServer,
            adapterPrimaryIpAddress: native.AdapterPrimaryAddress.AsHostToIpAddress(),
            adapterSubnetAddress: native.AdapterSubnetAddress.AsHostToIpMask(),
            interfaceDescription: native.IfDescription,
            interfaceGuidId: native.IfIdGuid,
            interfaceId: native.IfIdIsGuid ? null : native.IfId);
    }
}