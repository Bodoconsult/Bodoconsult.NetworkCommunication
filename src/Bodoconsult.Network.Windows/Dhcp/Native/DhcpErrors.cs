// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp.Native;

internal enum DhcpErrors : uint
{
    [DhcpErrorDescription("Success")]
    Success = 0,

    [DhcpErrorDescription("More data is available.")]
    ErrorMoreData = 234,

    [DhcpErrorDescription("No more data is available.")]
    ErrorNoMoreItems = 259,

    [DhcpErrorDescription("There are no more endpoints available from the endpoint mapper.")]
    EptSNotRegistered = 1753,

    [DhcpErrorDescription("The RPC server is unavailable.")]
    RpcSServerUnavailable = 1722,

    [DhcpErrorDescription("The UUID type is not supported.")]
    RpcSUnsupportedType = 1734,

    [DhcpErrorDescription("This call was performed by a client who is not a member of the \"DHCP Administrators\" security group.")]
    ErrorAccessDenied = 5,
    [DhcpErrorDescription("The parameter is incorrect.")]
    ErrorInvalidParameter = 87,

    [DhcpErrorDescription("The system cannot find the file specified.")]
    ErrorFileNotFound = 2,

    [DhcpErrorDescription("The DHCP server registry initialization parameters are incorrect.")]
    RegistryInitFailed = 20000,
    [DhcpErrorDescription("The DHCP server was unable to open the database of DHCP clients.")]
    DatabaseInitFailed = 20001,
    [DhcpErrorDescription("The DHCP server was unable to start as a Remote Procedure Call (RPC) server.")]
    RpcInitFailed = 20002,
    [DhcpErrorDescription("The DHCP server was unable to establish a socket connection.")]
    NetworkInitFailed = 20003,
    [DhcpErrorDescription("The specified subnet already exists on the DHCP server.")]
    SubnetExists = 20004,
    [DhcpErrorDescription("The specified subnet does not exist on the DHCP server.")]
    SubnetNotPresent = 20005,
    [DhcpErrorDescription("The primary host information for the specified subnet was not found on the DHCP server.")]
    PrimaryNotFound = 20006,
    [DhcpErrorDescription("The specified DHCP element has been used by a client and cannot be removed.")]
    ElementCantRemove = 20007,
    [DhcpErrorDescription("The specified option already exists on the DHCP server.")]
    OptionExists = 20009,
    [DhcpErrorDescription("The specified option does not exist on the DHCP server.")]
    OptionNotPresent = 20010,
    [DhcpErrorDescription("The specified IP address is not available.")]
    AddressNotAvailable = 20011,
    [DhcpErrorDescription("The specified IP address range has all of its member addresses leased.")]
    RangeFull = 20012,
    [DhcpErrorDescription(@"An error occurred while accessing the DHCP JET database. For more information about this error, please look at the DHCP server event log.")]
    JetError = 20013,
    [DhcpErrorDescription("The specified client already exists in the database.")]
    ClientExists = 20014,
    [DhcpErrorDescription("The DHCP server received an invalid message.")]
    InvalidDhcpMessage = 20015,
    [DhcpErrorDescription("The DHCP server received an invalid message from the client.")]
    InvalidDhcpClient = 20016,
    [DhcpErrorDescription("The DHCP server is currently paused.")]
    ServicePaused = 20017,
    [DhcpErrorDescription("The specified DHCP client is not a reserved client.")]
    NotReservedClient = 20018,
    [DhcpErrorDescription("The specified DHCP client is a reserved client.")]
    ReservedClient = 20019,
    [DhcpErrorDescription("The specified IP address range is too small.")]
    RangeTooSmall = 20020,
    [DhcpErrorDescription("The specified IP address range is already defined on the DHCP server.")]
    IprangeExists = 20021,
    [DhcpErrorDescription("The specified IP address is currently taken by another client.")]
    ReservedipExists = 20022,
    [DhcpErrorDescription("The specified IP address range either overlaps with an existing range or is invalid.")]
    InvalidRange = 20023,
    [DhcpErrorDescription("The specified IP address range is an extension of an existing range.")]
    RangeExtended = 20024,
    [DhcpErrorDescription(@"The specified IP address range extension is too small. The number of addresses in the extension must be a multiple of 32.")]
    RangeExtensionTooSmall = 20025,
    [DhcpErrorDescription(@"An attempt was made to extend the IP address range to a value less than the specified backward extension. The number of addresses in the extension must be a multiple of 32. ")]
    WarningRangeExtendedLess = 20026,
    [DhcpErrorDescription(@"The DHCP database needs to be upgraded to a newer format. For more information, refer to the DHCP server event log.")]
    JetConvRequired = 20027,
    [DhcpErrorDescription(@"The format of the bootstrap protocol file table is incorrect. The correct format is:
<requested boot file name 1>,<boot file server name 1>, <boot file name 1>
<requested boot file name 2>,<boot file server name 2>, <boot file name 2>
...")]
    ServerInvalidBootFileTable = 20027,
    [DhcpErrorDescription("A boot file name specified in the bootstrap protocol file table is unrecognized or invalid.")]
    ServerUnknownBootFileName = 20029,
    [DhcpErrorDescription("The specified superscope name is too long.")]
    SuperScopeNameTooLong = 20030,
    [DhcpErrorDescription("The specified IP address is already in use.")]
    IpAddressInUse = 20032,
    [DhcpErrorDescription("The specified path to the DHCP audit log file is too long.")]
    LogFilePathTooLong = 20033,
    [DhcpErrorDescription("The DHCP server received a request for a valid IP address not administered by the server.")]
    UnsupportedClient = 20034,
    [DhcpErrorDescription(@"The DHCP server failed to receive a notification when the interface list changed, therefore some of the interfaces will not be enabled on the server.")]
    ServerInterfaceNotificationEvent = 20035,
    [DhcpErrorDescription(@"The DHCP database needs to be upgraded to a newer format (JET97). For more information, refer to the DHCP server event log.")]
    Jet97ConvRequired = 20036,
    [DhcpErrorDescription(@"The DHCP server cannot determine if it has the authority to run, and is not servicing clients on the network. This rogue status may be due to network problems or insufficient server resources.")]
    RogueInitFailed = 20037,
    [DhcpErrorDescription("The DHCP service is shutting down because another DHCP server is active on the network.")]
    RogueSamshutdown = 20038,
    [DhcpErrorDescription("The DHCP server does not have the authority to run, and is not servicing clients on the network.")]
    RogueNotAuthorized = 20039,
    [DhcpErrorDescription(@"The DHCP server is unable to contact the directory service for this domain. The DHCP server will continue to attempt to contact the directory service. During this time, no clients on the network will be serviced.")]
    RogueDsUnreachable = 20040,
    [DhcpErrorDescription("The DHCP server's authorization information conflicts with that of another DHCP server on the network.")]
    RogueDsConflict = 20041,
    [DhcpErrorDescription(@"The DHCP server is ignoring a request from another DHCP server because the second server is a member of a different directory service enterprise.")]
    RogueNotOurEnterprise = 20042,
    [DhcpErrorDescription("The DHCP server has detected a directory service environment on the network. If there is a directory service on the network, the DHCP server can only run if it is a part of the directory service. Since the server ostensibly belongs to a workgroup, it is terminating.")]
    StandaloneInDs = 20043,
    [DhcpErrorDescription("The specified DHCP class name  is unknown or invalid.")]
    ClassNotFound = 20044,
    [DhcpErrorDescription("The specified DHCP class name (or information) is already in use.")]
    ClassAlreadyExists = 20045,
    [DhcpErrorDescription("The specified DHCP scope name is too long, the scope name must not exceed 256 characters.")]
    ScopeNameTooLong = 20046,
    [DhcpErrorDescription("The default scope is already configured on the server.")]
    DefaultScopeExists = 20047,
    [DhcpErrorDescription("The Dynamic BOOTP attribute cannot be turned on or off.")]
    CantChangeAttribute = 20048,
    [DhcpErrorDescription("Conversion of a scope to a \"DHCP Only\" scope or to a \"BOOTP Only\" scope is not allowed when the scope contains other DHCP and BOOTP clients. Either the DHCP or BOOTP clients should be specifically deleted before converting the scope to the other type.")]
    IprangeConvIllegal = 20049,
    [DhcpErrorDescription("The network has changed. Retry this operation after checking for network changes. Network changes may be caused by interfaces that are new or invalid, or by IP addresses that are new or invalid.")]
    NetworkChanged = 20050,
    [DhcpErrorDescription("The bindings to internal IP addresses cannot be modified.")]
    CannotModifyBindings = 20051,
    [DhcpErrorDescription("The DHCP scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.")]
    BadScopeParameters = 20052,
    [DhcpErrorDescription("The DHCP multicast scope parameters are incorrect. Either the scope already exists, or its properties are inconsistent with the subnet address and mask of an existing scope.")]
    MscopeExists = 20053,
    [DhcpErrorDescription("The multicast scope range must have at least 256 IP addresses.")]
    MscopeRangeTooSmall = 20054,
    [DhcpErrorDescription("The DHCP server could not contact Active Directory.")]
    DdsNoDsAvailable = 20070,
    [DhcpErrorDescription("The DHCP service root could not be found in  Active Directory.")]
    DdsNoDhcpRoot = 20071,
    [DhcpErrorDescription("An unexpected error occurred while accessing  Active Directory.")]
    DdsUnexpectedError = 20072,
    [DhcpErrorDescription("There were too many errors to proceed.")]
    DdsTooManyErrors = 20073,
    [DhcpErrorDescription("A DHCP service could not be found.")]
    DdsDhcpServerNotFound = 20074,
    [DhcpErrorDescription("The specified DHCP options are already present in  Active Directory.")]
    DdsOptionAlreadyExists = 20075,
    [DhcpErrorDescription("The specified DHCP options are not present in  Active Directory.")]
    DdsOptionDoesNotExist = 20076,
    [DhcpErrorDescription("The specified DHCP classes are already present in  Active Directory.")]
    DdsClassExists = 20077,
    [DhcpErrorDescription("The specified DHCP classes are not present in  Active Directory.")]
    DdsClassDoesNotExist = 20078,
    [DhcpErrorDescription("The specified DHCP servers are already present in  Active Directory.")]
    DdsServerAlreadyExists = 20079,
    [DhcpErrorDescription("The specified DHCP servers are not present in  Active Directory.")]
    DdsServerDoesNotExist = 20080,
    [DhcpErrorDescription("The specified DHCP server address does not correspond to the identified DHCP server name.")]
    DdsServerAddressMismatch = 20081,
    [DhcpErrorDescription("The specified subnets are already present in  Active Directory.")]
    DdsSubnetExists = 20082,
    [DhcpErrorDescription("The specified subnet belongs to a different superscope.")]
    DdsSubnetHasDiffSuperScope = 20083,
    [DhcpErrorDescription("The specified subnet is not present in  Active Directory.")]
    DdsSubnetNotPresent = 20084,
    [DhcpErrorDescription("The specified reservation is not present in  Active Directory.")]
    DdsReservationNotPresent = 20085,
    [DhcpErrorDescription("The specified reservation conflicts with another reservation present in  Active Directory.")]
    DdsReservationConflict = 20086,
    [DhcpErrorDescription("The specified IP address range conflicts with another IP range present in  Active Directory.")]
    DdsPossibleRangeConflict = 20087,
    [DhcpErrorDescription("The specified IP address range is not present in  Active Directory.")]
    DdsRangeDoesNotExist = 20088,
    [DhcpErrorDescription("Windows 7 or later: This class cannot be deleted.")]
    DeleteBuiltinClass = 20089,
    [DhcpErrorDescription("Windows 7 or later: The given subnet prefix is invalid. It represents either a non-unicast or link local address range.")]
    InvalidSubnetPrefix = 20091,
    [DhcpErrorDescription("Windows 7 or later: The given delay value is invalid. The valid value is from 0 to 1000.")]
    InvalidDelay = 20092,
    [DhcpErrorDescription("Windows 7 or later: Address or Address pattern is already contained in one of the list.")]
    LinklayerAddressExists = 20093,
    [DhcpErrorDescription("Windows 7 or later: Address to be added to Deny list or to be deleted from allow list, has an associated reservation.")]
    LinklayerAddressReservationExists = 20094,
    [DhcpErrorDescription("Windows 7 or later: Address or Address pattern is not contained in either list.")]
    LinklayerAddressDoesNotExist = 20095,
    [DhcpErrorDescription("Windows 7 or later: This Hardware Type is already exempt.")]
    HardwareAddressTypeAlreadyExempt = 20101,
    [DhcpErrorDescription("Windows 7 or later: You are trying to delete an undefined Hardware Type.")]
    UndefinedHardwareAddressType = 20102,
    [DhcpErrorDescription("Windows 7 or later: Conflict in types for the same option on Host and Added DHCP Servers.")]
    OptionTypeMismatch = 20103,
    [DhcpErrorDescription("Windows 8 or later: The parent expression specified does not exist.")]
    PolicyBadParentExpr = 20104,
    [DhcpErrorDescription("Windows 8 or later: The DHCP server policy already exists.")]
    PolicyExists = 20105,
    [DhcpErrorDescription("Windows 8 or later: The DHCP server policy range specified already exists in the given scope.")]
    PolicyRangeExists = 20106,
    [DhcpErrorDescription("Windows 8 or later: The DHCP server policy range specified is invalid or does not match the given subnet.")]
    PolicyRangeBad = 20107,
    [DhcpErrorDescription("Windows 8 or later: DHCP server policy ranges can only be added to scope level policies.")]
    RangeInvalidInServerPolicy = 20108,
    [DhcpErrorDescription("Windows 8 or later: The DHCP server policy contains an invalid expression.")]
    InvalidPolicyExpression = 20109,
    [DhcpErrorDescription("Windows 8 or later: The processing order specified for the DHCP server policy is invalid.")]
    InvalidProcessingOrder = 20110,
    [DhcpErrorDescription("Windows 8 or later: The DHCP server policy was not found.")]
    PolicyNotFound = 20111,
    [DhcpErrorDescription("Windows 8 or later: There is an IP address range configured for a policy in this scope. This operation on the scope IP address range cannot be performed until the policy IP address range is suitably modified. Please change the IP address range of the policy before performing this operation.")]
    ScopeRangePolicyRangeConflict = 20112,
    [DhcpErrorDescription("Windows 8 or later: The DHCP scope is already in a failover relationship.")]
    FoScopeAlreadyInRelationship = 20113,
    [DhcpErrorDescription("Windows 8 or later: The DHCP failover relationship already exists.")]
    FoRelationshipExists = 20114,
    [DhcpErrorDescription("Windows 8 or later: The DHCP failover relationship does not exist.")]
    FoRelationshipDoesNotExist = 20115,
    [DhcpErrorDescription("Windows 8 or later: The DHCP scope is not part of a failover relationship.")]
    FoScopeNotInRelationship = 20116,
    [DhcpErrorDescription("Windows 8 or later: The DHCP failover relationship is a secondary.")]
    FoRelationIsSecondary = 20117,
    [DhcpErrorDescription("Windows 8 or later: The DHCP failover is not supported.")]
    FoNotSupported = 20118,
    [DhcpErrorDescription("Windows 8 or later: The DHCP servers in the failover relationship have timed out of synchronization.")]
    FoTimeOutOfSync = 20119,
    [DhcpErrorDescription("Windows 8 or later: The DHCP failover relationship state is not NORMAL.")]
    FoStateNotNormal = 20120,
    [DhcpErrorDescription("Windows 8 or later: The user does not have administrative permissions for the DHCP server.")]
    NoAdminPermission = 20121,
    [DhcpErrorDescription("Windows 8 or later: The specified DHCP server is not reachable. Please provide a DHCP server that is reachable.")]
    ServerNotReachable = 20122,
    [DhcpErrorDescription("Windows 8 or later: The DHCP Server Service is not running on the specified server. Please ensure that the DHCP Server service is running on the specified computer.")]
    ServerNotRunning = 20123,
    [DhcpErrorDescription("Windows 8 or later: Unable to resolve DNS name.")]
    ServerNameNotResolved = 20124,
    [DhcpErrorDescription("Windows 8 or later: The specified DHCP failover relationship name is too long. The name is limited to a maximum of 126 characters.")]
    FoRelationshipNameTooLong = 20125,
    [DhcpErrorDescription("Windows 8 or later: The specified DHCP Server has reached the end of the selected range while finding the free IP address.")]
    ReachedEndOfSelection = 20126,
    [DhcpErrorDescription("Windows 8 or later: The synchronization of leases in the scopes being added to the failover relationship  failed.")]
    FoAddscopeLeasesNotSynced = 20127,
    [DhcpErrorDescription("Windows 8 or later: The relationship cannot be created on the DHCP server as the maximum number of allowed relationship has been exceeded.")]
    FoMaxRelationships = 20128,
    [DhcpErrorDescription("Windows 8 or later: A Scope configured for failover cannot be changed to type BOOTP or BOTH.")]
    FoIprangeTypeConvIllegal = 20129,
    [DhcpErrorDescription("Windows 8 or later: The number of scopes being added to the failover relationship exceeds the max number of scopes which can be added to a failover relationship at one time.")]
    FoMaxAddScopes = 20130,
    [DhcpErrorDescription("Windows 8 or later: A scope supporting BOOTP clients cannot be added to a failover relationship.")]
    FoBootNotSupported = 20131,
    [DhcpErrorDescription("Windows 8 or later: An IP address range of a scope which is part of a failover relationship cannot be deleted. The scope will need to be removed from the failover relationship before deleting the range.")]
    FoRangePartOfRel = 20132,
}
