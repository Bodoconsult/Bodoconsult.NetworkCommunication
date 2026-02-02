// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

internal static class Api
{
    /// <summary>
    /// The DhcpEnumServers function returns an enumerated list of DHCP servers found in the directory service. 
    /// </summary>
    /// <param name="flags">Reserved for future use. This field should be set to 0.</param>
    /// <param name="idInfo">Pointer to an address containing the server's ID block. This field should be set to null.</param>
    /// <param name="servers">Pointer to a <see cref="DhcpDsServers"/> structure that contains the output list of DHCP servers.</param>
    /// <param name="callbackFn">Pointer to the callback function that will be called when the server add operation completes. This field should be null.</param>
    /// <param name="callbackData">Pointer to a data block containing the formatted structure for callback information. This field should be null.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true)]
    public static extern DhcpErrors DhcpEnumServers(uint flags, IntPtr idInfo, out IntPtr servers, IntPtr callbackFn, IntPtr callbackData);

    /// <summary>
    /// The DhcpGetVersion function returns the major and minor version numbers of the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="majorVersion">Specifies the major version number of the DHCP server.</param>
    /// <param name="minorVersion">Specifies the minor version number of the DHCP server.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetVersion(string serverIpAddress, out int majorVersion, out int minorVersion);

    /// <summary>
    /// The DhcpServerGetConfig function returns the specific configuration settings of a DHCP server. Configuration information includes information on the JET database used to store subnet and client lease information, and the supported protocols.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="configInfo">Pointer to a <see cref="DhcpServerConfigInfo"/> structure that contains the specific configuration information for the DHCP server. Note: The memory for this parameter must be free using <see cref="DhcpRpcFreeMemory"/>.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpServerGetConfig(string serverIpAddress, out IntPtr configInfo);

    /// <summary>
    /// The DhcpGetServerSpecificStrings function retrieves the names of the default vendor class and user class.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IPv4 address of the DHCPv4 server.</param>
    /// <param name="serverSpecificStrings">Pointer to a DHCP_SERVER_SPECIFIC_STRINGS structure that receives the information for the default vendor class and user class name strings. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetServerSpecificStrings(string serverIpAddress, out IntPtr serverSpecificStrings);

    /// <summary>
    /// The DhcpGetServerBindingInfo function returns endpoint bindings set on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">Specifies a set of flags describing the endpoints to return.
    /// DHCP_ENDPOINT_FLAG_CANT_MODIFY (0x01) = Returns unmodifiable endpoints only.
    /// </param>
    /// <param name="bindElementsInfo">Pointer to a DHCP_BIND_ELEMENT_ARRAY structure that contains the server network endpoint bindings. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>This function requires network byte ordering for all DHCP_IP_ADDRESS values in parameter structures.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetServerBindingInfo(string serverIpAddress, uint flags, out IntPtr bindElementsInfo);

    /// <summary>
    /// The DhcpGetSubnetDelayOffer function obtains the delay period for DHCP OFFER messages after a DISCOVER message is received.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that contains the IP address of the subnet gateway.</param>
    /// <param name="timeDelayInMilliseconds">Unsigned 16-bit integer value that receive the time to delay an OFFER message after receiving a DISCOVER message as configured on the DHCP server, in milliseconds.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetSubnetDelayOffer(string serverIpAddress, DhcpIpAddress subnetAddress, out ushort timeDelayInMilliseconds);

    /// <summary>
    /// The DhcpSetSubnetDelayOffer function sets the delay period for DHCP OFFER messages after a DISCOVER message is received, for a specific DHCP scope.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that contains the IP address of the subnet gateway.</param>
    /// <param name="timeDelayInMilliseconds">Unsigned 16-bit integer value that specifies the time to delay an OFFER message after receiving a DISCOVER message, in milliseconds, and set for a particular scope. This value must be between 0 and 1000 (milliseconds). The default value is 0.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpSetSubnetDelayOffer(string serverIpAddress, DhcpIpAddress subnetAddress, ushort timeDelayInMilliseconds);

    /// <summary>
    /// The DhcpEnumOptions function returns an enumerated set of options stored on the DHCPv4 server.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a Unicode string that specifies the IPv4 address of the DHCP server.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of options are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth. 
    /// The presence of additional enumerable data is indicated when this function returns ERROR_MORE_DATA. If no additional enumerable data is available on the DHCPv4 server, ERROR_NO_MORE_ITEMS is returned.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of bytes of options to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.
    /// To retrieve all the option definitions for the default user and vendor class, set this parameter to 0xFFFFFFFF.</param>
    /// <param name="options">Pointer to a <see cref="DhcpOptionArray"/> structure containing the returned options. If there are no options available on the DHCPv4 server, this parameter will return null.</param>
    /// <param name="optionsRead">Pointer to a DWORD value that specifies the number of options returned in Options.</param>
    /// <param name="optionsTotal">Pointer to a DWORD value that specifies the total number of remaining options stored on the DHCPv4 server.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumOptions(string serverIpAddress, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr options, out int optionsRead, out int optionsTotal);

    /// <summary>
    /// The DhcpEnumOptions function returns an enumerated set of options stored on the DHCPv4 server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">A set of flags that indicate the option definition for enumeration. 0x00000000 = The option definitions are enumerated for a default vendor class; DHCP_FLAGS_OPTION_IS_VENDOR (0x00000003) = The option definitions are enumerated for a specific vendor class.</param>
    /// <param name="className">Pointer to a Unicode string that contains the name of the class whose options will be enumerated. This parameter is optional. </param>
    /// <param name="vendorName">Pointer to a Unicode string that contains the name of the vendor for the class. This parameter is optional. If a vendor class name is not provided, the default vendor class name is used.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes of option definitions are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of bytes of options to return. If the number of remaining unenumerated option definitions (in bytes) is less than this value, all option definitions are returned.</param>
    /// <param name="options">Pointer to a DHCP_OPTION_ARRAY structure containing the returned option definitions. If there are no option definitions available on the DHCP server, this parameter will return null.</param>
    /// <param name="optionsRead">Pointer to a DWORD value that specifies the number of option definitions returned in Options.</param>
    /// <param name="optionsTotal">Pointer to a DWORD value that specifies the total number of unenumerated option definitions on the DHCP server.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumOptionsV5(string serverIpAddress, uint flags, string className, string vendorName, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr options, out int optionsRead, out int optionsTotal);

    /// <summary>
    /// The DhcpEnumOptionValues function returns an enumerated list of option values (just the option data and the associated ID number) for a given scope.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="scopeInfo">DHCP_OPTION_SCOPE_INFO structure that contains the level (specifically: default, server, scope, or IPv4 reservation level) for which the option values are defined and should be enumerated. </param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of option values are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.
    /// The presence of additional enumerable data is indicated when this function returns ERROR_MORE_DATA. If no additional enumerable data is available on the DHCPv4 server, ERROR_NO_MORE_ITEMS is returned.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of bytes of option values to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.
    /// To retrieve all the option values for the default user and vendor class at the specified level, set this parameter to 0xFFFFFFFF.</param>
    /// <param name="optionValues">Pointer to a <see cref="DhcpOptionValueArray"/> structure that contains the enumerated option values returned for the specified scope. If there are no option values available for this scope on the DHCP server, this parameter will return null.</param>
    /// <param name="optionsRead">Pointer to a DWORD value that specifies the number of option values returned in OptionValues.</param>
    /// <param name="optionsTotal">Pointer to a DWORD value that specifies the total number of remaining option values for this scope stored on the DHCP server.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumOptionValues(string serverIpAddress, IntPtr scopeInfo, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr optionValues, out int optionsRead, out int optionsTotal);

    /// <summary>
    /// The DhcpEnumOptionValuesV5 function returns an enumerated list of option values (just the option data and the associated ID number) for a specific scope within a given user or vendor class.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">Specifies a bit flag that indicates whether or not the option is vendor specific. If it is not vendor specific, this parameter must be 0. 0x00000000 = The option values are enumerated for a default vendor class; DHCP_FLAGS_OPTION_IS_VENDOR (0x00000003) = The option values are enumerated for a specific vendor class.</param>
    /// <param name="className">Pointer to a Unicode string that contains the name of the class whose scope option values will be enumerated.</param>
    /// <param name="vendorName">Pointer to a Unicode string that contains the name of the vendor for the class. This parameter is optional. If a vendor class name is not provided, the option values enumerated for a default vendor class.</param>
    /// <param name="scopeInfo">Pointer to a DHCP_OPTION_SCOPE_INFO structure that contains the scope for which the option values are defined. This value defines the option values that will be retrieved from the server, scope, or default level, or for an IPv4 reservation.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes' worth of option values are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of bytes of option values to return. If the number of remaining unenumerated options (in bytes) is less than this value, all option values are returned.</param>
    /// <param name="optionValues">Pointer to a DHCP_OPTION_VALUE_ARRAY structure that contains the enumerated option values returned for the specified scope. If there are no option values available for this scope on the DHCP server, this parameter will return null.</param>
    /// <param name="optionsRead">Pointer to a DWORD value that specifies the number of option values returned in OptionValues.</param>
    /// <param name="optionsTotal">Pointer to a DWORD value that specifies the total number of as-yet unenumerated option values for this scope stored on the DHCP server.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumOptionValuesV5(string serverIpAddress, uint flags, string className, string vendorName, IntPtr scopeInfo, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr optionValues, out int optionsRead, out int optionsTotal);

    /// <summary>
    /// The DhcpGetOptionValue function retrieves a DHCP option value (the option code and associated data) for a particular scope.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="optionId">DHCP_OPTION_ID value that specifies the code for the option value to retrieve.</param>
    /// <param name="scopeInfo">DHCP_OPTION_SCOPE_INFO structure that contains information on the scope where the option value is set.</param>
    /// <param name="optionValue">DHCP_OPTION_VALUE structure that contains the returned option code and data. NOTE: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetOptionValue(string serverIpAddress, int optionId, IntPtr scopeInfo, out IntPtr optionValue);

    /// <summary>
    /// The DhcpGetOptionValueV5 function retrieves a DHCP option value (the option code and associated data) for a particular scope. This function extends the functionality provided by DhcpGetOptionValue by allowing the caller to specify a class and/or vendor for the option.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">Flag value that indicates whether the option is for a specific or default vendor class. 0x00000000 = The option value is retrieved for a default vendor class; DHCP_FLAGS_OPTION_IS_VENDOR (0x00000003) = The option value is retrieved for a specific vendor class. The vendor name is supplied in VendorName.</param>
    /// <param name="optionId">DHCP_OPTION_ID value that specifies the code for the option value to retrieve.</param>
    /// <param name="className">Unicode string that specifies the DHCP class name of the option. This parameter is optional.</param>
    /// <param name="vendorName">Unicode string that specifies the vendor of the option. This parameter is optional, and should be null when Flags is not set to DHCP_FLAGS_OPTION_IS_VENDOR. If the vendor class is not specified, the option value is returned for the default vendor class.</param>
    /// <param name="scopeInfo">DHCP_OPTION_SCOPE_INFO structure that contains information on the scope where the option value is set.</param>
    /// <param name="optionValue">DHCP_OPTION_VALUE structure that contains the returned option code and data.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetOptionValueV5(string serverIpAddress, uint flags, int optionId, string className, string vendorName, IntPtr scopeInfo, out IntPtr optionValue);

    /// <summary>
    /// The DhcpSetOptionValue function sets information for a specific option value on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="optionId">DHCP_OPTION_ID value that specifies the unique code for a DHCP option.</param>
    /// <param name="scopeInfo">Pointer to a DHCP_OPTION_SCOPE_INFO structure that contains information describing the level (default, server, scope, or IPv4 reservation) at which this option value will be set.</param>
    /// <param name="optionValue">Pointer to a DHCP_OPTION_DATA structure that contains the data value corresponding to the DHCP option code specified by OptionID.</param>
    /// <remarks>
    ///     When this function is called for the first time, it creates the supplied option value in the DHCP server database.
    ///     Otherwise, it modifies the option value for a specific option associated with the default user class and vendor class.
    ///     These values can be set for the default, server, scope, or IPv4 reservation level on the DHCP server.
    /// </remarks>
    /// <returns></returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpSetOptionValue(string serverIpAddress, int optionId, IntPtr scopeInfo, IntPtr optionValue);

    /// <summary>
    /// The DhcpSetOptionValueV5 function sets information for a specific option value on the DHCP server. This function extends the functionality provided by DhcpSetOptionValue by allowing the caller to specify a class and/or vendor for the option.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">Specifies a bit flag that indicates whether or not the options are vendor-specific. If the qualification of vendor options is not necessary, this parameter should be 0. DHCP_FLAGS_OPTION_IS_VENDOR = This flag should be set if vendor-specific options are desired.</param>
    /// <param name="optionId">DHCP_OPTION_ID value that contains the unique option ID number (also called an "option code") of the option being set. Many of these option ID numbers are defined; a complete list of standard DHCP and BOOTP option codes can be found at http://www.ietf.org/rfc/rfc2132.txt.</param>
    /// <param name="className">Unicode string that specifies the DHCP class of the option. This parameter is optional.</param>
    /// <param name="vendorName">Unicode string that specifies the vendor of the option. This parameter is optional, and should be NULL when Flags is not set to DHCP_FLAGS_OPTION_IS_VENDOR.</param>
    /// <param name="scopeInfo">Pointer to a DHCP_OPTION_SCOPE_INFO structure that contains information describing the DHCP scope this option value will be set on.</param>
    /// <param name="optionValue">Pointer to a DHCP_OPTION_DATA structure that contains the data value corresponding to the DHCP option code specified by OptionID.</param>
    /// <returns></returns>
    [DllImport("dhcpsapi.dll", SetLastError = false, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpSetOptionValueV5(string serverIpAddress, uint flags, int optionId, string className, string vendorName, IntPtr scopeInfo, IntPtr optionValue);

    /// <summary>
    /// The DhcpRemoveOptionValue function removes the option value for a specific option on the DHCP4 server for the default user class and vendor class, for the specified scope.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="optionId">DHCP_OPTION_ID value that contains the code uniquely identifying the specific option to remove from the DHCP server.</param>
    /// <param name="scopeInfo">DHCP_OPTION_SCOPE_INFO structure that contains information describing the specific scope (default, server, scope, or IPv4 reservation level) from which to remove the option value.</param>
    /// <returns></returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpRemoveOptionValue(string serverIpAddress, int optionId, IntPtr scopeInfo);

    /// <summary>
    /// The DhcpRemoveOptionValueV5 function removes an option value from a scope defined on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">Specifies a bit flag that indicates whether or not the options are vendor-specific. If the qualification of vendor options is not necessary, this parameter should be 0. DHCP_FLAGS_OPTION_IS_VENDOR = This flag should be set if vendor-specific options are desired.</param>
    /// <param name="optionId">DHCP_OPTION_ID value that specifies the code for the option value to remove.</param>
    /// <param name="className">Unicode string that specifies the DHCP class name of the option value. This parameter is optional.</param>
    /// <param name="vendorName">Unicode string that specifies the vendor of the option. This parameter is optional, and should be NULL when Flags is not set to DHCP_FLAGS_OPTION_IS_VENDOR.</param>
    /// <param name="scopeInfo">DHCP_OPTION_SCOPE_INFO structure that contains information describing the specific scope to remove the option value from.</param>
    /// <returns></returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpRemoveOptionValueV5(string serverIpAddress, uint flags, int optionId, string className, string vendorName, IntPtr scopeInfo);

    /// <summary>
    /// The DhcpGetClassInfo function returns the user or vendor class information configured on a specific DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="reservedMustBeZero">Reserved. This parameter must be set to 0.</param>
    /// <param name="partialClassInfo">DHCP_CLASS_INFO structure that contains data provided by the caller for the following members, with all other fields initialized.
    /// - ClassName;
    /// - ClassData;
    /// - ClassDataLength;
    /// These fields must not be null.</param>
    /// <param name="filledClassInfo">DHCP_CLASS_INFO structure returned after lookup that contains the complete class information.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetClassInfo(string serverIpAddress, uint reservedMustBeZero, DhcpClassInfoManaged partialClassInfo, out IntPtr filledClassInfo);

    /// <summary>
    /// The DhcpEnumClasses function enumerates the user or vendor classes configured for the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="reservedMustBeZero">Reserved. This field must be set to zero.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 100 classes, and 200 classes are stored on the server, the resume handle can be used after the first 100 classes are retrieved to obtain the next 100 on a subsequent call, and so forth.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of classes to return. If the number of remaining unenumerated classes is less than this value, then that amount will be returned. To retrieve all classes available on the DHCP server, set this parameter to 0xFFFFFFFF.</param>
    /// <param name="classInfoArray">Pointer to a DHCP_CLASS_INFO_ARRAY structure that contains the returned classes. If there are no classes available on the DHCP server, this parameter will return null.</param>
    /// <param name="nRead">Pointer to a DWORD value that specifies the number of classes returned in ClassInfoArray.</param>
    /// <param name="nTotal">Pointer to a DWORD value that specifies the total number of classes stored on the DHCP server.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>A DHCP class is a specific category of client, defined either by the vendor or by a user. An example of a vendor-defined class would be all Windows 8 clients, with Microsoft as the vendor. A user-defined class consists of those clients with specific attributes selected by a user or administrator, such as all laptops or clients that support wireless connections.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumClasses(string serverIpAddress, uint reservedMustBeZero, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr classInfoArray, out int nRead, out int nTotal);

    /// <summary>
    /// The DhcpAddSubnetElement function adds an element describing a feature or aspect of the subnet to the subnet entry in the DHCP database.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a Unicode string that contains the IPv4 address of the subnet DHCP server.</param>
    /// <param name="subnetAddress"><see cref="DhcpIpAddress"/> structure that contains the IPv4 address of the subnet.</param>
    /// <param name="addElementInfo">Pointer to a DHCP_SUBNET_ELEMENT_DATA structure that contains information about the subnet element corresponding to the IPv4 subnet specified in SubnetAddress.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpAddSubnetElement(string serverIpAddress, DhcpIpAddress subnetAddress, in DhcpSubnetElementDataManaged addElementInfo);

    /// <summary>
    /// The DhcpAddSubnetElementV5 function adds an element describing a feature or aspect of the subnet to the subnet entry in the DHCP database. Windows 2000 and earlier:  This function is not available.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a Unicode string that contains the IPv4 address of the subnet DHCP server.</param>
    /// <param name="subnetAddress"><see cref="DhcpIpAddress"/> structure that contains the IPv4 address of the subnet.</param>
    /// <param name="addElementInfo">Pointer to a <see cref="DhcpSubnetElementDataV5"/> structure that contains the element data to add to the subnet. The V5 structure adds support for BOOTP clients.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpAddSubnetElementV5(string serverIpAddress, DhcpIpAddress subnetAddress, in DhcpSubnetElementDataV5Managed addElementInfo);

    /// <summary>
    /// The DhcpRemoveSubnetElement function removes an IPv4 subnet element from an IPv4 subnet defined on the DHCPv4 server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCPv4 server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that specifies the IPv4 address of the subnet gateway from which elements are to be removed.</param>
    /// <param name="removeElementInfo">DHCP_SUBNET_ELEMENT_DATA structure that contains information used to find the element that will be removed from subnet specified in SubnetAddress.</param>
    /// <param name="forceFlag">DHCP_FORCE_FLAG enumeration value that indicates whether or not the clients affected by the removal of the subnet element should also be deleted.</param>
    /// <remarks>
    /// If the flag is set to DhcpNoForce and this subnet has served an IPv4 address to DHCPv4/BOOTP clients, the IPv4 range is not deleted; conversely, if the flag is set to DhcpFullForce, the IPv4 range is deleted along with the DHCPv4 client lease record on the DHCPv4 server.
    /// </remarks>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpRemoveSubnetElement(string serverIpAddress, DhcpIpAddress subnetAddress, in DhcpSubnetElementDataManaged removeElementInfo, DhcpForceFlag forceFlag);

    /// <summary>
    /// The DhcpRemoveSubnetElement function removes an IPv4 subnet element from an IPv4 subnet defined on the DHCPv4 server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCPv4 server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that specifies the IPv4 address of the subnet gateway from which elements are to be removed.</param>
    /// <param name="removeElementInfo">DHCP_SUBNET_ELEMENT_DATA structure that contains information used to find the element that will be removed from subnet specified in SubnetAddress.</param>
    /// <param name="forceFlag">DHCP_FORCE_FLAG enumeration value that indicates whether or not the clients affected by the removal of the subnet element should also be deleted.</param>
    /// <remarks>
    /// If the flag is set to DhcpNoForce and this subnet has served an IPv4 address to DHCPv4/BOOTP clients, the IPv4 range is not deleted; conversely, if the flag is set to DhcpFullForce, the IPv4 range is deleted along with the DHCPv4 client lease record on the DHCPv4 server.
    /// </remarks>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpRemoveSubnetElementV5(string serverIpAddress, DhcpIpAddress subnetAddress, in DhcpSubnetElementDataV5Managed removeElementInfo, DhcpForceFlag forceFlag);

    /// <summary>
    /// The DhcpCreateSubnet function creates a new subnet on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress"><see cref="DhcpIpAddress"/> value that contains the IP address of the subnet's gateway.</param>
    /// <param name="subnetInfo"><see cref="DhcpSubnetInfo"/> structure that contains specific settings for the subnet, including the subnet mask and IP address of the subnet gateway.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpCreateSubnet(string serverIpAddress, DhcpIpAddress subnetAddress, in DhcpSubnetInfoManaged subnetInfo);

    /// <summary>
    /// The DhcpDeleteSubnet function deletes a subnet from the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress"><see cref="DhcpIpAddress"/> value that contains the IP address of the subnet gateway used to identify the subnet.</param>
    /// <param name="forceFlag">DHCP_FORCE_FLAG enumeration value that indicates the type of delete operation to perform (full force, failover force, or no force).</param>
    /// <returns></returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpDeleteSubnet(string serverIpAddress, DhcpIpAddress subnetAddress, DhcpForceFlag forceFlag);

    /// <summary>
    /// The DhcpEnumSubnets function returns an enumerated list of subnets defined on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 100, and 200 subnet addresses are stored on the server, the resume handle can be used after the first 100 subnets are retrieved to obtain the next 100 on a subsequent call, and so forth.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of subnet addresses to return. If the number of remaining unenumerated options is less than this value, then that amount will be returned.</param>
    /// <param name="enumInfo">Pointer to a <see cref="DhcpIpArray"/> structure that contains the subnet IDs available on the DHCP server. If no subnets are defined, this value will be null.</param>
    /// <param name="elementsRead">Pointer to a DWORD value that specifies the number of subnet addresses returned in EnumInfo.</param>
    /// <param name="elementsTotal">Pointer to a DWORD value that specifies the number of subnets defined on the DHCP server that have not yet been enumerated.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. If a call is made with the same ResumeHandle value and all items on the server have been enumerated, this method returns ERROR_NO_MORE_ITEMS with ElementsRead and ElementsTotal set to 0. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>When no longer needed, the resources consumed for the enumerated data, and all pointers containted within, should be released with DhcpRpcFreeMemory. This function requires host byte ordering for all DHCP_IP_ADDRESS values in parameter structures.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumSubnets(string serverIpAddress, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr enumInfo, out int elementsRead, out int elementsTotal);

    /// <summary>
    /// The DhcpGetSubnetInfo function returns information on a specific subnet.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that specifies the subnet ID.</param>
    /// <param name="subnetInfo"><see cref="DhcpSubnetInfo"/> structure that contains the returned information for the subnet matching the ID specified by SubnetAddress. Note: The memory for this parameter must be free using <see cref="DhcpRpcFreeMemory"/>.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>This function uses host byte ordering for all DHCP_IP_ADDRESS values in the <see cref="DhcpSubnetInfo"/> structure passed back to the caller in the SubnetInfo parameter. However, this function uses network byte order for the IpAddress member of the DHCP_HOST_INFO structure within the <see cref="DhcpSubnetInfo"/> structure.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetSubnetInfo(string serverIpAddress, DhcpIpAddress subnetAddress, out IntPtr subnetInfo);

    /// <summary>
    /// The DhcpGetSubnetInfoVQ function retrieves the information about a specific IPv4 subnet defined on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that specifies the subnet ID.</param>
    /// <param name="subnetInfo"><see cref="DhcpSubnetInfo"/> structure that contains the returned information for the subnet matching the IPv4 address specified by SubnetAddress. Note: The memory for this parameter must be free using <see cref="DhcpRpcFreeMemory"/>.</param>        /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetSubnetInfoVQ(string serverIpAddress, DhcpIpAddress subnetAddress, out IntPtr subnetInfo);

    /// <summary>
    /// The DhcpSetSubnetInfo function sets information about a subnet defined on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress"><see cref="DhcpIpAddress"/> value that specifies the IP address of the subnet gateway, as well as uniquely identifies the subnet.</param>
    /// <param name="subnetInfo">Pointer to a <see cref="DhcpSubnetInfo"/> structure that contains the information about the subnet.</param>
    /// <returns></returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpSetSubnetInfo(string serverIpAddress, DhcpIpAddress subnetAddress, in DhcpSubnetInfoManaged subnetInfo);

    /// <summary>
    /// The DhcpSetSubnetInfo function sets information about a subnet defined on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress"><see cref="DhcpIpAddress"/> value that specifies the IP address of the subnet gateway, as well as uniquely identifies the subnet.</param>
    /// <param name="subnetInfo">Pointer to a <see cref="DhcpSubnetInfoVq"/> structure that contains the information about the subnet.</param>
    /// <returns></returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpSetSubnetInfoVQ(string serverIpAddress, DhcpIpAddress subnetAddress, in DhcpSubnetInfoVqManaged subnetInfo);

    /// <summary>
    /// The DhcpGetAllOptions function returns an array that contains all options defined on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">Specifies a bit flag that indicates whether or not the options are vendor-specific. If the qualification of vendor options is not necessary, this parameter should be 0. DHCP_FLAGS_OPTION_IS_VENDOR = This flag should be set if vendor-specific options are desired.</param>
    /// <param name="optionStruct">Pointer to a DHCP_ALL_OPTIONS structure containing every option defined for a vendor or default class. If there are no options available on the server, this value will be null. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>There will be one option element in the array specified by OptionStruct for each vendor/class pair defined on the DHCP server.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetAllOptions(string serverIpAddress, uint flags, out IntPtr optionStruct);

    /// <summary>
    /// The DhcpGetAllOptionValues function returns an array that contains all option values defined for a specific scope on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">Specifies a bit flag that indicates whether the options are vendor-specific. If the qualification of vendor options is not necessary, this parameter should be 0.</param>
    /// <param name="scopeInfo">Pointer to a DHCP_OPTION_SCOPE_INFO structure that contains information on the specific scope whose option values will be returned. This information defines the option values that are being retrieved from the default, server, or scope level, or for a specific IPv4 reservation.</param>
    /// <param name="values">Pointer to a DHCP_ALL_OPTION_VALUES structure that contains the returned option values for the scope specified in ScopeInfo.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>There will be one option value in the array specified by Values for each vendor/class pair defined on the DHCP server.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetAllOptionValues(string serverIpAddress, uint flags, IntPtr scopeInfo, out IntPtr values);

    /// <summary>
    /// The DhcpGetOptionInfo function returns information on a specific DHCP option for the default user and vendor class.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IPv4 address of the DHCP server.</param>
    /// <param name="optionId">DHCP_OPTION_ID value that specifies the code for the option to retrieve information on.</param>
    /// <param name="optionInfo">Pointer to a DHCP_OPTION structure that contains the returned information on the option specified by OptionID. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.
    /// ERROR_DHCP_OPTION_NOT_PRESENT = The specified option definition could not be found in the DHCP server database.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetOptionInfo(string serverIpAddress, int optionId, out IntPtr optionInfo);

    /// <summary>
    /// The DhcpGetOptionInfoV5 function returns information on a specific DHCP option.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">Specifies a bit flag that indicates whether or not the option is vendor-specific. If it is not, this parameter should be 0.</param>
    /// <param name="optionId">DHCP_OPTION_ID value that specifies the code for the option to retrieve information on.</param>
    /// <param name="className">Unicode string that specifies the DHCP class name of the option. This parameter is optional.</param>
    /// <param name="vendorName">Unicode string that specifies the vendor of the option. This parameter is optional, and must be null when Flags is not set to DHCP_FLAGS_OPTION_IS_VENDOR. If it is not set, then the option definition for the default vendor class is returned.</param>
    /// <param name="optionInfo">Pointer to a DHCP_OPTION structure that contains the returned information on the option specified by OptionID.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetOptionInfoV5(string serverIpAddress, uint flags, int optionId, string className, string vendorName, out IntPtr optionInfo);

    /// <summary>
    /// The DhcpEnumSubnetClients function returns an enumerated list of clients with served IP addresses in the specified subnet.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that contains the subnet ID. See RFC 950 for more information about subnet ID.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of subnet client information structures are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of bytes of subnet client information structures to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.  The minimum value is 1024 bytes (1KB), and the maximum value is 65536 bytes (64KB); if the input value is greater or less than this range, it will be set to the maximum or minimum value, respectively.</param>
    /// <param name="clientInfo">Pointer to a DHCP_CLIENT_INFO_ARRAY structure that contains information on the clients served under this specific subnet. If no clients are available, this field will be null.</param>
    /// <param name="clientsRead">Pointer to a DWORD value that specifies the number of clients returned in ClientInfo.</param>
    /// <param name="clientsTotal">Pointer to a DWORD value that specifies the number of clients for the specified subnet that have not yet been enumerated. Note  This value is set to the correct value during the final enumeration call; however, prior calls to this function set the value as "0x7FFFFFFF".</param>
    /// <returns>This function returns ERROR_MORE_DATA upon a successful call. The final call to this method with the last set of subnet clients returns ERROR_SUCCESS. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>This function requires host byte ordering for all DHCP_IP_ADDRESS values in parameter structures.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumSubnetClients(string serverIpAddress, DhcpIpAddress subnetAddress, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr clientInfo, out int clientsRead, out int clientsTotal);

    /// <summary>
    /// The DhcpEnumSubnetClientsV4 function returns an enumerated list of client lease records with served IP addresses in the specified subnet. This function extends the functionality provided in DhcpEnumSubnetClients by returning a list of DHCP_CLIENT_INFO_V4 structures that contain the specific client type (DHCP and/or BOOTP).
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value containing the IP address of the subnet gateway.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. This parameter contains the last last IPv4 address retrieved from the DHCPv4 client. The presence of additional enumerable data is indicated when this function returns ERROR_MORE_DATA. If no additional enumerable data is available on the DHCPv4 server, ERROR_NO_MORE_ITEMS is returned.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of bytes of subnet client elements to return. If the number of remaining unenumerated elements (in bytes) is less than this value, then that amount will be returned. The minimum value is 1024 bytes, and the maximum value is 65536 bytes. To retrieve all the subnet client elements for the default user and vendor class at the specified level, set this parameter to 0xFFFFFFFF.</param>
    /// <param name="clientInfo">Pointer to a DHCP_CLIENT_INFO_ARRAY_V4 structure that contains the DHCPv4 client lease record array. If no clients are available, this field will be null.</param>
    /// <param name="clientsRead">Pointer to a DWORD value that specifies the number of client lease records returned in ClientInfo.</param>
    /// <param name="clientsTotal">Pointer to a DWORD value that specifies the total number of client lease records remaining on the DHCPv4 server. For example, if there are 100 DHCPv4 lease records for an IPv4 subnet, and if 10 DHCPv4 lease records are enumerated per call, then this parameter would return a value of 90 after the first call.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>The caller of this function must free the memory for ClientInfo after the call completes. </remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumSubnetClientsV4(string serverIpAddress, DhcpIpAddress subnetAddress, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr clientInfo, out int clientsRead, out int clientsTotal);

    /// <summary>
    /// The DhcpEnumSubnetClientsV5 function returns an enumerated list of clients with served IP addresses in the specified subnet. This function extends the features provided in the DhcpEnumSubnetClients function by returning a list of DHCP_CLIENT_INFO_V5 structures that contain the specific client type (DHCP and/or BOOTP) and the IP address state.
    /// </summary>
    /// <param name="serverIpAddress">A UNICODE string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">A value containing the IP address of the subnet gateway. If this parameter is set to 0, then the DHCP clients for all IPv4 subnets defined on the DHCP server are returned.</param>
    /// <param name="resumeHandle">A pointer to a handle that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of subnet client information structures are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.</param>
    /// <param name="preferredMaximum">The preferred maximum number of bytes of subnet client information structures to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.</param>
    /// <param name="clientInfo">A pointer to a DHCP_CLIENT_INFO_ARRAY_V5 structure containing information on the clients served under this specific subnet. If no clients are available, this field will be null.</param>
    /// <param name="clientsRead">A pointer to a value that specifies the number of clients returned in ClientInfo.</param>
    /// <param name="clientsTotal">A pointer to a value that specifies the total number of clients for the specified subnet stored on the DHCP server.</param>
    /// <returns>The DhcpEnumSubnetClientsV5 function returns ERROR_SUCCESS upon success. </returns>
    /// <remarks>The caller of this function must release the memory used by the DHCP_CLIENT_INFO_ARRAY_V5 structure returned in buffer pointed to by the ClientInfo parameter when the information is no longer needed.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumSubnetClientsV5(string serverIpAddress, DhcpIpAddress subnetAddress, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr clientInfo, out int clientsRead, out int clientsTotal);

    /// <summary>
    /// The DhcpEnumSubnetClientsVQ function retrieves all DHCP clients serviced from the specified IPv4 subnet.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that contains the IPv4 subnet for which the DHCP clients are returned. If this parameter is set to 0, the DHCP clients for all known IPv4 subnets are returned.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation on the DHCP server. Initially, this value must be set to 0. A successful call will return a handle value in this parameter, which can be passed to subsequent enumeration requests. The returned handle value is the last IPv4 address retrieved in the enumeration operation.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of bytes to return in the enumeration operation. the minimum value is 1024 bytes, and the maximum value is 65536 bytes.</param>
    /// <param name="clientInfo">Pointer to a DHCP_CLIENT_INFO_ARRAY_VQ structure that contains the DHCP client lease record set returned by the enumeration operation.</param>
    /// <param name="clientsRead">Pointer to a value that specifies the number of DHCP client records returned in ClientInfo.</param>
    /// <param name="clientsTotal">Pointer to a value that specifies the number of DHCP client record remaining and as-yet unreturned. For example, if there are 100 DHCP client records for a given IPv4 subnet, and if 10 client records are enumerated per call, then after the first call this value would return 90.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>If SubnetAddress is set to zero (0), then all of the DHCP clients from all known IPv4 subnets. The caller of this function must free the data pointed to by ClientInfo.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumSubnetClientsVQ(string serverIpAddress, DhcpIpAddress subnetAddress, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr clientInfo, out int clientsRead, out int clientsTotal);

    /// <summary>
    /// The DhcpEnumSubnetElementsV5 function returns an enumerated list of elements for a specific DHCP subnet.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that specifies the address of the IP subnet whose elements will be enumerated.</param>
    /// <param name="enumElementType">DHCP_SUBNET_ELEMENT_TYPE enumeration value that indicates the type of subnet element to enumerate.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of subnet elements are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.
    /// The presence of additional enumerable data is indicated when this function returns ERROR_MORE_DATA. If no additional enumerable data is available on the DHCPv4 server, ERROR_NO_MORE_ITEMS is returned.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of bytes of subnet elements to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.
    /// To retrieve all the subnet client elements for the default user and vendor class at the specified level, set this parameter to 0xFFFFFFFF.</param>
    /// <param name="enumElementInfo">Pointer to a pointer to a DHCP_SUBNET_ELEMENT_INFO_ARRAY structure containing an enumerated list of all elements available for the specified subnet. If no elements are available for enumeration, this value will be null.</param>
    /// <param name="elementsRead">Pointer to a DWORD value that specifies the number of subnet elements returned in EnumElementInfo.</param>
    /// <param name="elementsTotal">Pointer to a DWORD value that specifies the total number of elements for the specified subnet.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes. Common errors include the following:</returns>
    /// <remarks>When no longer needed, the resources consumed for the enumerated data, and all pointers contained within, should be released with DhcpRpcFreeMemory.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumSubnetElements(string serverIpAddress, DhcpIpAddress subnetAddress, DhcpSubnetElementType enumElementType, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr enumElementInfo, out int elementsRead, out int elementsTotal);

    /// <summary>
    /// The DhcpEnumSubnetElementsV5 function returns an enumerated list of elements for a specific DHCP subnet.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS value that specifies the address of the IP subnet whose elements will be enumerated.</param>
    /// <param name="enumElementType">DHCP_SUBNET_ELEMENT_TYPE enumeration value that indicates the type of subnet element to enumerate.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE value that identifies the enumeration operation. Initially, this value should be zero, with a successful call returning the handle value used for subsequent enumeration requests. For example, if PreferredMaximum is set to 1000 bytes, and 2000 bytes worth of subnet elements are stored on the server, the resume handle can be used after the first 1000 bytes are retrieved to obtain the next 1000 on a subsequent call, and so forth.
    /// The presence of additional enumerable data is indicated when this function returns ERROR_MORE_DATA. If no additional enumerable data is available on the DHCPv4 server, ERROR_NO_MORE_ITEMS is returned.</param>
    /// <param name="preferredMaximum">Specifies the preferred maximum number of bytes of subnet elements to return. If the number of remaining unenumerated options (in bytes) is less than this value, then that amount will be returned.
    /// To retrieve all the subnet client elements for the default user and vendor class at the specified level, set this parameter to 0xFFFFFFFF.</param>
    /// <param name="enumElementInfo">Pointer to a DHCP_SUBNET_ELEMENT_INFO_ARRAY_V5 structure containing an enumerated list of all elements available for the specified subnet. If no elements are available for enumeration, this value will be null.</param>
    /// <param name="elementsRead">Pointer to a DWORD value that specifies the number of subnet elements returned in EnumElementInfo.</param>
    /// <param name="elementsTotal">Pointer to a DWORD value that specifies the total number of elements for the specified subnet.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes. Common errors include the following:</returns>
    /// <remarks>When no longer needed, the resources consumed for the enumerated data, and all pointers contained within, should be released with DhcpRpcFreeMemory.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpEnumSubnetElementsV5(string serverIpAddress, DhcpIpAddress subnetAddress, DhcpSubnetElementType enumElementType, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr enumElementInfo, out int elementsRead, out int elementsTotal);

    /// <summary>
    /// The DhcpGetClientInfo function returns information about a specific DHCP client.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="searchInfo">DHCP_SEARCH_INFO structure that contains the parameters for the search.</param>
    /// <param name="clientInfo">Pointer to a DHCP_CLIENT_INFO structure that contains information describing the DHCP client that most closely matches the provided search parameters. If no client is found, this parameter will be null. Note: The memory for this parameter must be free using DhcpRpcFreeMemory.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>This function requires host byte ordering for all DHCP_IP_ADDRESS values in parameter structures.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetClientInfo(string serverIpAddress, IntPtr searchInfo, out IntPtr clientInfo);

    /// <summary>
    /// The DhcpGetClientInfoVQ function retrieves DHCP client lease record information from the DHCP server database.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="searchInfo">Pointer to a DHCP_SEARCH_INFO structure that defines the key used to search the client lease record database on the DHCP server for a particular client record.</param>
    /// <param name="clientInfo">Pointer to the DHCP_CLIENT_INFO_VQ structure returned by a successful search operation.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    /// <remarks>The caller of this function must release the memory used by the DHCP_CLIENT_INFO_VQ structure returned in ClientInfo.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpGetClientInfoVQ(string serverIpAddress, IntPtr searchInfo, out IntPtr clientInfo);

    /// <summary>
    /// The DhcpCreateClientInfo function creates a client information record on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="clientInfo">DHCP_CLIENT_INFO structure that contains information about the DHCP client, including the assigned IP address, subnet mask, and host.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpCreateClientInfo(string serverIpAddress, IntPtr clientInfo);

    /// <summary>
    /// The DhcpSetClientInfo function sets information on a client whose IP address lease is administrated by the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="clientInfo">Pointer to a DHCP_CLIENT_INFO structure that contains the information on a client in a subnet served by the DHCP server.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpSetClientInfo(string serverIpAddress, in DhcpClientInfoManaged clientInfo);

    /// <summary>
    /// The DhcpDeleteClientInfo function deletes a client information record from the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="clientInfo">DHCP_SEARCH_INFO union structure that contains one of the following items used to search the DHCP client record database: the client IP address, the client MAC address, or the client network name. All records matching the value will be deleted; for example, if a client IP address of 192.1.1.10 is supplied, all records with this address in the ClientIpAddress field will be deleted.</param>
    /// <returns>This function returns ERROR_SUCCESS upon a successful call. Otherwise, it returns one of the DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpDeleteClientInfo(string serverIpAddress, IntPtr clientInfo);

    /// <summary>
    /// The DhcpAuditLogGetParams function returns the audit log configuration settings from the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a Unicode string that specifies the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">Specifies a set of bit flags for filtering the audit log. Currently, this parameter is reserved and should be set to 0.</param>
    /// <param name="auditLogDir">Unicode string that contains the directory where the audit log is stored as an absolute path within the file system.</param>
    /// <param name="diskCheckInterval">Specifies the disk check interval for attempting to write the audit log to the specified file as the number of logged DHCP server events that should occur between checks. The default is 50 DHCP server events between checks.</param>
    /// <param name="maxLogFilesSize">Specifies the maximum log file size, in bytes.</param>
    /// <param name="minSpaceOnDisk">Specifies the minimum required disk space, in bytes, for audit log storage.</param>
    /// <returns></returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpAuditLogGetParams(string serverIpAddress, int flags, [MarshalAs(UnmanagedType.LPWStr)] out string auditLogDir, out int diskCheckInterval, out int maxLogFilesSize, out int minSpaceOnDisk);

    /// <summary>
    /// The DhcpV4FailoverAddScopeToRelationship function adds a DHCPv4 scope to the specified failover relationship.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="pRelationship">Pointer to a DHCP_FAILOVER_RELATIONSHIP structure that contains both the scope information to add and the failover relationship to modify.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverAddScopeToRelationship(string serverIpAddress, in DhcpFailoverRelationshipManaged pRelationship);

    /// <summary>
    /// The DhcpV4FailoverCreateRelationship function creates a new DHCPv4 failover relationship between two servers.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="pRelationship">Pointer to a DHCP_FAILOVER_RELATIONSHIP structure that contains information about the DHCPv4 failover relationship to create.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverCreateRelationship(string serverIpAddress, in DhcpFailoverRelationshipManaged pRelationship);

    /// <summary>
    /// The DhcpV4FailoverDeleteRelationship function deletes a DHCPv4 failover relationship between two servers.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="relationshipName">Pointer to null-terminated Unicode string that represents the name of the relationship to delete.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverDeleteRelationship(string serverIpAddress, string relationshipName);

    /// <summary>
    /// The DhcpV4FailoverDeleteScopeFromRelationship function deletes a DHCPv4 scope from the specified failover relationship.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="relationship">Pointer to a DHCP_FAILOVER_RELATIONSHIP structure that contains the scopes to delete. The scopes are defined in the pScopes member of this structure.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverDeleteScopeFromRelationship(string serverIpAddress, in DhcpFailoverRelationshipManaged relationship);

    /// <summary>
    /// The DhcpV4FailoverEnumRelationship function enumerates all failover relationships present on the server.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="resumeHandle">Pointer to a DHCP_RESUME_HANDLE structure that identifies this enumeration for use in subsequent calls to this function.
    ///     Initially, this value should be zero on input. If successful, the returned value should be used for subsequent enumeration requests. For example, if PreferredMaximum is set to 100, and 200 reservation elements are configured on the server, the resume handle can be used after the first 100 policies are retrieved to obtain the next 100 on a subsequent call.</param>
    /// <param name="preferredMaximum">The maximum number of failover relationship elements to return in pRelationship. If PreferredMaximum is greater than the number of remaining non-enumerated policies on the server, the remaining number of non-enumerated policies is returned.</param>
    /// <param name="relationship">Pointer to a DHCP_FAILOVER_RELATIONSHIP_ARRAY structure that contains an array of the failover relationships available on the DHCP server. If no relationships are configured, this value is NULL.</param>
    /// <param name="relationshipRead">Pointer to a DWORD that specifies the number of failover relationship elements returned in pRelationship.</param>
    /// <param name="relationshipTotal">Pointer to a DWORD that specifies the number of failover relationships configured on the DHCP server that have not yet been enumerated.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverEnumRelationship(string serverIpAddress, ref IntPtr resumeHandle, uint preferredMaximum, out IntPtr relationship, out int relationshipRead, out int relationshipTotal);

    /// <summary>
    /// The DhcpV4FailoverGetAddressStatus function returns the status of a IPv4 address.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="subnetAddress">DHCP_IP_ADDRESS structure that contains the IPv4 address whose status is being requested.</param>
    /// <param name="status">Pointer to a DWORD that returns the status of the IPv4 address as specified in the table below:
    /// 0 = The IPv4 address will be leased by a primary server.
    /// 1 = The IPv4 address will be leased by a secondary server.
    /// 2 = The IPv4 address is part of an exclusion range.
    /// 3 = The IPv4 address is a reservation.
    /// </param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverGetAddressStatus(string serverIpAddress, DhcpIpAddress subnetAddress, out int status);

    /// <summary>
    /// The DhcpV4FailoverGetRelationship function retrieves relationship details for a specific relationship name.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="relationshipName">Pointer to a null-terminated Unicode string which represents the name of the relationship to retrieve.</param>
    /// <param name="relationship">Pointer to a DHCP_FAILOVER_RELATIONSHIP structure that contains information about the retrieved failover relationship.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverGetRelationship(string serverIpAddress, string relationshipName, out IntPtr relationship);

    /// <summary>
    /// The DhcpV4FailoverGetScopeRelationship function retrieves the failover relationship that is configured on a specified DHCPv4 scope.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="scopeId">A DHCP_IP_ADDRESS field that contains the IPv4 scope address for which the relationship details are to be retrieved.</param>
    /// <param name="relationship">Pointer to a DHCP_FAILOVER_RELATIONSHIP structure that contains information about the retrieved failover relationship which contains scopeId field in its pScopes member.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverGetScopeRelationship(string serverIpAddress, DhcpIpAddress scopeId, out IntPtr relationship);

    /// <summary>
    /// The DhcpV4FailoverGetScopeStatistics function retrieves the address usage statistics of a specific scope that is part of a failover relationship.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="scopeId">DHCP_IP_ADDRESS structure that contains the IPv4 scope address of the address usage statistics to retrieve.</param>
    /// <param name="stats">Pointer to a DHCP_FAILOVER_STATISTICS structure that contains the address usage information for scopeId.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverGetScopeStatistics(string serverIpAddress, DhcpIpAddress scopeId, out IntPtr stats);

    /// <summary>
    /// The DhcpV4FailoverGetSystemTime function returns the current time on the DHCP server.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="time">Pointer to a DWORD that returns the current time, in seconds, elapsed since midnight, January 1, 1970, Coordinated Universal Time (UTC), on the DHCP server.</param>
    /// <param name="maxAllowedDeltaTime">?</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverGetSystemTime(string serverIpAddress, out int time, out int maxAllowedDeltaTime);

    /// <summary>
    /// The DhcpV4FailoverSetRelationship function sets or modifies the parameters of a DHCPv4 server failover relationship.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="flags">A bitmask that specifies the fields to update in pRelationship. Each value specifies a member of the DHCP_FAILOVER_RELATIONSHIP structure to be modified.</param>
    /// <param name="relationship">Pointer to a DHCP_FAILOVER_RELATIONSHIP structure that contains update information about the fields in the DHCPv4 failover relationship.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverSetRelationship(string serverIpAddress, DhcpFailoverRelationshipSetFlags flags, in DhcpFailoverRelationshipManaged relationship);

    /// <summary>
    /// The DhcpV4FailoverTriggerAddrAllocation function redistributes the free addresses between the primary server and the secondary server that are part of a failover relationship.
    /// </summary>
    /// <param name="serverIpAddress">Pointer to a null-terminated Unicode string that represents the IP address or hostname of the DHCP server.</param>
    /// <param name="failRelName">Pointer to a null-terminated Unicode string that represents the name of the failover relationship for which free addresses are to be redistributed.</param>
    /// <returns>If the function succeeds, it returns ERROR_SUCCESS. If the function fails, it returns one of the following or an error code from DHCP Server Management API Error Codes.</returns>
    [DllImport("dhcpsapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern DhcpErrors DhcpV4FailoverTriggerAddrAllocation(string serverIpAddress, string failRelName);

    /// <summary>
    /// The DhcpRpcFreeMemory function frees a block of buffer space returned as a parameter.
    /// </summary>
    /// <param name="bufferPointer">Pointer to an address that contains a structure (or structures, in the case of an array) returned as a parameter. </param>
    /// <remarks>This function should be called to release the memory consumed by any structures.</remarks>
    [DllImport("dhcpsapi.dll", SetLastError = true)]
    public static extern void DhcpRpcFreeMemory(IntPtr bufferPointer);

    /// <summary>
    /// Helper which calls <see cref="DhcpRpcFreeMemory(IntPtr)"/> if the pointer != <see cref="IntPtr.Zero"/>
    /// </summary>
    public static void FreePointer(IntPtr pointer)
    {
        if (pointer != IntPtr.Zero)
            DhcpRpcFreeMemory(pointer);
    }
}
