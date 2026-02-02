// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;
using System.Text;
using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

/// <summary>
/// Defines a DHCP option class
/// </summary>
public class DhcpServerClass : IDhcpServerClass
{
    /// <summary>
    /// The associated DHCP Server
    /// </summary>
    public DhcpServer Server { get; }
    IDhcpServer IDhcpServerClass.Server => Server;

    /// <summary>
    /// Name of the Class
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Comment associated with the Class
    /// </summary>
    public string Comment { get; }

    /// <summary>
    /// Indicates whether or not the options are vendor-specific
    /// </summary>
    public bool IsVendorClass { get; }

    /// <summary>
    /// Indicates whether or not the options are user-specific
    /// </summary>
    public bool IsUserClass => !IsVendorClass;

    /// <summary>
    /// A byte buffer that contains specific data for the class
    /// </summary>
    public byte[] Data { get; }

    /// <summary>
    /// An ASCII representation of the <see cref="Data"/> buffer.
    /// </summary>
    public string DataText => (Data == null) ? null : Encoding.ASCII.GetString(Data);

    /// <summary>
    /// Enumerates a list of all Options associated with this class
    /// </summary>
    public IEnumerable<IDhcpServerOption> Options
    {
        get
        {
            if (IsVendorClass)
                return DhcpServerOption.EnumVendorOptions(Server, Name);
            else
                return DhcpServerOption.EnumUserOptions(Server, Name);
        }
    }

    /// <summary>
    /// Enumerates a list of all Global Option Values associated with this class
    /// </summary>
    public IEnumerable<IDhcpServerOptionValue> GlobalOptionValues
    {
        get
        {
            if (IsVendorClass)
                return DhcpServerOptionValue.EnumGlobalVendorOptionValues(Server, Name);
            else
                return DhcpServerOptionValue.EnumGlobalUserOptionValues(Server, Name);
        }
    }

    private DhcpServerClass(DhcpServer server, string name, string comment, bool isVendorClass, byte[] data)
    {
        Server = server;
        Name = name;
        Comment = comment;
        IsVendorClass = isVendorClass;
        Data = data;
    }

    internal static DhcpServerClass GetClass(DhcpServer server, string name)
    {
        var query = new DhcpClassInfoManaged(className: name,
            classDataLength: 0,
            classData: IntPtr.Zero);

        var result = Api.DhcpGetClassInfo(serverIpAddress: server.Address,
            reservedMustBeZero: 0,
            partialClassInfo: query,
            filledClassInfo: out var classIntoPtr);

        if (result != DhcpErrors.Success)
            throw new DhcpServerException(nameof(Api.DhcpGetClassInfo), result);

        try
        {
            using (var classInfo = classIntoPtr.MarshalToStructure<DhcpClassInfo>())
            {
                return FromNative(server, in classInfo);
            }
        }
        finally
        {
            Api.FreePointer(classIntoPtr);
        }
    }

    internal static IEnumerable<DhcpServerClass> GetClasses(DhcpServer server)
    {
        var resumeHandle = IntPtr.Zero;
        var result = Api.DhcpEnumClasses(serverIpAddress: server.Address,
            reservedMustBeZero: 0,
            resumeHandle: ref resumeHandle,
            preferredMaximum: 0xFFFFFFFF,
            classInfoArray: out var enumInfoPtr,
            nRead: out var elementsRead,
            nTotal: out _);

        if (result == DhcpErrors.ErrorNoMoreItems || result == DhcpErrors.EptSNotRegistered)
            yield break;

        if (result != DhcpErrors.Success && result != DhcpErrors.ErrorMoreData)
            throw new DhcpServerException(nameof(Api.DhcpEnumClasses), result);

        try
        {
            if (elementsRead == 0)
                yield break;

            using (var enumInfo = enumInfoPtr.MarshalToStructure<DhcpClassInfoArray>())
            {
                foreach (var element in enumInfo.Classes)
                    yield return FromNative(server, in element);
            }
        }
        finally
        {
            Api.FreePointer(enumInfoPtr);
        }
    }

    internal static DhcpServerClass FromNative(DhcpServer server, in DhcpClassInfo native)
    {
        var data = new byte[native.ClassDataLength];
        Marshal.Copy(native.ClassData, data, 0, native.ClassDataLength);

        return new DhcpServerClass(server: server,
            name: native.ClassName,
            comment: native.ClassComment,
            isVendorClass: native.IsVendor,
            data: data);
    }
}