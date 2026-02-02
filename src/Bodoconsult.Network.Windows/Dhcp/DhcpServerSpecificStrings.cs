// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

public class DhcpServerSpecificStrings : IDhcpServerSpecificStrings
{
    public string DefaultVendorClassName { get; }
    public string DefaultUserClassName { get; }

    private DhcpServerSpecificStrings(string defaultVendorClassName, string defaultUserClassName)
    {
        DefaultVendorClassName = defaultVendorClassName;
        DefaultUserClassName = defaultUserClassName;
    }

    private static DhcpServerSpecificStrings FromNative(Native.DhcpServerSpecificStrings strings)
        => new(strings.DefaultVendorClassName, strings.DefaultUserClassName);

    internal static DhcpServerSpecificStrings GetSpecificStrings(DhcpServer server)
    {
        var result = Api.DhcpGetServerSpecificStrings(serverIpAddress: server.Address,
            serverSpecificStrings: out var stringsPtr);

        if (result != DhcpErrors.Success)
            throw new DhcpServerException(nameof(Api.DhcpGetServerSpecificStrings), result);

        try
        {
            var strings = stringsPtr.MarshalToStructure<Native.DhcpServerSpecificStrings>();
            return FromNative(strings);
        }
        finally
        {
            Api.DhcpRpcFreeMemory(stringsPtr);
        }
    }
}