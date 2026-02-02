# Bodoconsult.Network.Windows

## What does the library

Bodoconsult.Network.Windows provides tools for getting network information froma Windows domain network and other tools like SSH handling.

## How to use the library

The source code contain a NUnit test classes, the following source code is extracted from. The samples below show the most helpful use cases for the library.

# SshHandler class

## Overview

The main class in the library is the class SshHandler with the following methods:

* Connect()
* Disconnect()
* Put(string localPath, string remotePath)
* RemoveFile(string remotePath)
* Exists(string path)
* CreateDirectory(string remotePath)
* RemoveDirectory(string remotePath)
* GetDirectoryItems(string remotePath)
* GetDirectoryItemsRaw(string remotePath)
* DownloadFile(string remotePath, string localPath)

## Samples

```C#
[TestFixture]
public class UnitTestSshHandler
{

	private readonly SshCredentials _credentials = TestHelper.GetCredentials();

	[Test]
	public void TestConnect()
	{

		// Arrange
		var s = new SshHandler(_credentials);
		
		// Act
		s.Connect();

		var erg = s.IsConnected;

		s.Disconnect();

		// Assert
		Assert.IsTrue(erg);
	}


	[Test]
	public void TestPutMainDirectory()
	{

		// Arrange
		var localPath = TestHelper.FtpTestFilePath;

		const string remotePath = "/"+TestHelper.FtpTestFileName;

		var s = new SshHandler(_credentials);


		// Act
		s.Connect();

		var erg1 = s.IsConnected;

		
		s.Put(localPath, remotePath);

		s.Disconnect();

		// Assert
		Assert.IsTrue(erg1);
	}


	[Test]
	public void TestPutDirectory()
	{

		// Arrange
		var localPath = TestHelper.FtpTestFilePath;

		var remotePath = $"{TestHelper.FtpSubDir}/{TestHelper.FtpTestFileName}";

		var s = new SshHandler(_credentials);


		// Act
		s.Connect();

		var erg1 = s.IsConnected;


		s.Put(localPath, remotePath);

		s.Disconnect();

		// Assert
		Assert.IsTrue(erg1);
	}


	[Test]
	public void TestRemoveFile()
	{

		// Arrange
		var localPath = TestHelper.FtpTestFilePath;

		var remotePath = $"{TestHelper.FtpSubDir}/{TestHelper.FtpTestFileName}";

		var s = new SshHandler(_credentials);


		// Act
		s.Connect();

		var erg1 = s.IsConnected;

		s.Put(localPath, remotePath);

		Assert.IsTrue(s.Exists(remotePath));

		// Act
		s.RemoveFile(remotePath);
		var erg2 = s.Exists(remotePath);
		s.Disconnect();

		// Assert
		Assert.IsTrue(erg1);
		Assert.IsFalse(erg2);
	}


	[Test]
	public void TestRemoveFileFileNotExisting()
	{

		// Arrange
		var localPath = TestHelper.FtpTestFilePath;

		var remotePath = $"{TestHelper.FtpSubDir}/AAA{TestHelper.FtpTestFileName}";

		var s = new SshHandler(_credentials);

		s.Connect();

		var erg1 = s.IsConnected;

		s.Put(localPath, remotePath);

		// Act
		s.RemoveFile(remotePath);
		var erg2 = s.Exists(remotePath);
		s.Disconnect();

		// Assert
		Assert.IsTrue(erg1);
		Assert.IsFalse(erg2);
	}


	[Test]
	public void TestCreateDirectory()
	{

		// Arrange
		var remotePath = TestHelper.FtpSubDirCreate;

		var s = new SshHandler(_credentials);


		// Act
		s.Connect();

		var erg1 = s.IsConnected;

		s.RemoveDirectory(remotePath);
		Assert.IsFalse(s.Exists(remotePath));

		// Act
		s.CreateDirectory(remotePath);
		var erg2 = s.Exists(remotePath);
		s.Disconnect();

		// Assert
		Assert.IsTrue(erg1);
		Assert.IsTrue(erg2);
	}


	[Test]
	public void TestListDirectory()
	{

		// Arrange
		const string remotePath = TestHelper.FtpSubDir;

		var remotePath1 = TestHelper.FtpSubDirCreate;

		var s = new SshHandler(_credentials);

		s.Connect();

		if (!s.Exists(remotePath1)) s.CreateDirectory(remotePath1);

		var erg1 = s.IsConnected;

		var localPath = TestHelper.FtpTestFilePath;

		var remotePathFile = $"{TestHelper.FtpSubDir}/{TestHelper.FtpTestFileName}";

		if (!s.Exists(remotePathFile)) s.Put(localPath, remotePathFile);

		// Act
		var erg2 = s.GetDirectoryItems(remotePath).ToList();

		s.Disconnect();

		// Assert
		Assert.IsTrue(erg1);
		Assert.IsTrue(erg2.Any());
		Assert.IsTrue(erg2.Any(x => x.IsDirectory));
		Assert.IsTrue(erg2.Any(x => !x.IsDirectory));
	}



	[Test]
	public void TestListDirectoryRaw()
	{

		// Arrange
		const string remotePath = TestHelper.FtpSubDir;

		var remotePath1 = TestHelper.FtpSubDirCreate;

		var s = new SshHandler(_credentials);

		s.Connect();

		if (!s.Exists(remotePath1)) s.CreateDirectory(remotePath1);

		var erg1 = s.IsConnected;

		var localPath = TestHelper.FtpTestFilePath;

		var remotePathFile = $"{TestHelper.FtpSubDir}/{TestHelper.FtpTestFileName}";

		if (!s.Exists(remotePathFile)) s.Put(localPath, remotePathFile);

		// Act
		var erg2 = s.GetDirectoryItems(remotePath).ToList();

		s.Disconnect();

		// Assert
		Assert.IsTrue(erg1);
		Assert.IsTrue(erg2.Any());
		Assert.IsTrue(erg2.Any(x => x.IsDirectory));
		Assert.IsTrue(erg2.Any(x => !x.IsDirectory));
	}

	[Test]
	public void TestDownloadFile()
	{

		// Arrange
		var remotePath = "/"+ TestHelper.FtpTestFileName;
		var localPath = Path.Combine(TestHelper.LocalTargetPath,"AAA.txt");

		var s = new SshHandler(_credentials);


		if (File.Exists(localPath))
		{
			File.Delete(localPath);
		}


		// Act
		s.Connect();

		var erg1 = s.IsConnected;

		// Act
		s.DownloadFile(remotePath, localPath);
		s.Disconnect();

		var erg2 = File.Exists(localPath);

		// Assert
		Assert.IsTrue(erg1);
		Assert.IsTrue(erg2);
	}


	[Test]
	public void TestRemoveDirectory()
	{

		// Arrange
		var remotePath1 = TestHelper.FtpSubDirCreate;

		var s = new SshHandler(_credentials);

		s.Connect();

		if (!s.Exists(remotePath1)) s.CreateDirectory(remotePath1);

		var erg1 = s.IsConnected;

		// Act
		s.RemoveDirectory(remotePath1);

		// Assert
		var erg2 = s.Exists(remotePath1);

		s.Disconnect();

		
		Assert.IsTrue(erg1);
		Assert.IsFalse(erg2);
	}

}
```

# DHCP server API

A .NET (C#, Visual Basic.NET, etc) wrapper for the native DHCP management APIs exposed by Windows.

An initial focus has been made for read-only access to the data via the APIs, but the project can be easily extended to support read-write access.

## Connect to a DHCP Server

Method 1: Discovering via directory services

```C#
// Discover DHCP Servers
foreach (var dhcpServer in DhcpServer.Servers)
{
    Console.WriteLine(dhcpServer.Name);
}
```

Method 2: Connecting directly

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Connect("SRV-DHCP-01"); // or 192.168.1.1

Console.WriteLine(dhcpServer.Name);
```

## Read Basic DHCP Configuration

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.First();

// Display some configuration
Console.WriteLine($"Protocol Support: {dhcpServer.Configuration.ApiProtocolSupport}");
Console.WriteLine($"Database Name: {dhcpServer.Configuration.DatabaseName}");
Console.WriteLine($"Database Path: {dhcpServer.Configuration.DatabasePath}");

// Show all bound interfaces
foreach (var binding in dhcpServer.BindingElements)
{
    Console.WriteLine($"Binding Interface Id: {binding.InterfaceGuidId}");
    Console.WriteLine($"  Description: {binding.InterfaceDescription}");
    Console.WriteLine($"  Adapter Address: {binding.AdapterPrimaryIpAddress}");
    Console.WriteLine($"  Adapter Subnet: {binding.AdapterSubnetAddress}");
}
```

## Show DHCP Scopes

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.First();

// Display scope information
foreach (var scope in dhcpServer.Scopes)
{
    Console.WriteLine($"Scope: {scope.Name}");
    Console.WriteLine($"  Address: {scope.Address}");
    Console.WriteLine($"  Mask: {scope.Mask}");
    Console.WriteLine($"  Range: {scope.IpRange}");
    Console.WriteLine($"  State: {scope.State}");
}
```

## Show Client Leases

Client leases can be retrieved globally (all leases on the server) or individually for each scope.

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.Skip(1).First();

// Get a scope
var scope = dhcpServer.Scopes.First();

Console.WriteLine($"Scope '{scope.Name}' Clients");
Console.WriteLine();

// Get active client leases
var activeClients = scope.Clients
    .Where(c => c.AddressState == DhcpServerClientAddressStates.Active);

// Display client information
foreach (var client in activeClients)
{
    Console.WriteLine($"{client.IpAddress} [{client.HardwareAddress}] {client.Name}, Expires: {client.LeaseExpires}");
}
```

## Show Reservations

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.First();

// Get a scope
var scope = dhcpServer.Scopes.First();

Console.WriteLine($"Scope '{scope.Name}' Reservations");
Console.WriteLine();

// Display reservation information
foreach (var reservation in scope.Reservations)
{
    Console.WriteLine($"{reservation.IpAddress} [{reservation.HardwareAddress}] {reservation.Client.Name}");
}
```

## Show Scope Options

Options can be retrieved globally (all options on the server), for scopes or individual reservations.

```C#
// Connect to DHCP Server
var dhcpServer = DhcpServer.Servers.Skip(1).First();

// Get a scope
var scope = dhcpServer.Scopes.First();

Console.WriteLine($"Scope '{scope.Name}' Options");
Console.WriteLine();

// Get option values
foreach (var optionValue in scope.OptionValues)
{
    Console.WriteLine($"{optionValue.Option.Name} [{optionValue.OptionId}]:");

    foreach (var value in optionValue.Values)
    {
        Console.WriteLine($"  {value.Value} [{value.Type}]");
    }
}
```

A generic dotnet (C#, Visual Basic.NET, etc) library designed for managing DNS Servers.

A provider (implementation) is planned for Windows Server DNS Server (via the [WMI Provider](https://docs.microsoft.com/en-us/windows/win32/dns/dns-wmi-provider). Providers for other DNS Servers is possible and contributions are welcome.

#  DNS server API

A simple hierachy is present: `DnsServer > DnsZone > DnsRecord`.

Both `DnsServer` and `DnsZone` are abstract and must be implemented by the provider.

## Zones
```csharp
// connect to DNS server via provider
using (var server = new MockDnsServer())
{
    // create zone
    var myZone = server.CreateZone("myzone.mock");

    // get zone by name
    var myZoneRef = server.GetZone("myzone.mock");

    // enumerate zones
    foreach (var zone in server.Zones)
    {
        Console.WriteLine(zone.DomainName);
    }

    // delete zone
    myZone.Delete();
    // or: server.DeleteZone(zone);
    // or: server.DeleteZone("myzone.mock");
}
```

## Records

```csharp
// connect to DNS server
using (var server = new MockDnsServer())
{
    // create zone
    var zone = server.CreateZone("myzone.mock");

    // get records
    var myServerRecords = zone.GetRecords(DnsRecordTypes.A, "myserver.myzone.mock");

    // search records
    var allMyServerRecords = zone.SearchRecords("MyServer");
    var hostMyServerRecords = zone.SearchRecords(DnsRecordTypes.A, "MyServer");

    // update record
    var soa = zone.StartOfAuthority;
    soa.TimeToLive = TimeSpan.FromMinutes(30);
    soa.ResponsiblePerson = "responsible.hostmaster.";
    soa.Save();
    // or: zone.SaveRecord(soa);

    // create host record
    var hostTemplate = new DnsARecord("myhost.myzone.mock", TimeSpan.FromHours(1), "192.168.1.50");
    var hostRecord = zone.CreateRecord(hostTemplate);

    // add second host record
    hostTemplate.IpAddress = "192.168.1.51";
    var hostRecord2 = zone.CreateRecord(hostTemplate);

    // create alias record
    var cnameTemplate = new DnsCNAMERecord("myhostalias.myzone.mock", TimeSpan.FromHours(1), "myhost.myzone.mock");
    var cnameRecord = zone.CreateRecord(cnameTemplate);

    // create service locator record
    var srvTemplate = new DnsSRVRecord(
            domainName: "myzone.mock",
            service: DnsSRVRecord.ServiceNames.LDAP,
            protocol: DnsSRVRecord.ProtocolNames.TCP,
            timeToLive: TimeSpan.FromHours(1),
            priority: 0,
            weight: 10,
            port: DnsSRVRecord.ServicePorts.LDAP,
            targetDomainName: "mycontroller.myzone.mock");
    var srvRecord = zone.CreateRecord(srvTemplate);

    // delete record
    hostRecord.Delete();
    // or: zone.DeleteRecord(aRecord);
}
```



# About us

Bodoconsult (<http://www.bodoconsult.de>) is a Munich based software development company from Germany.

Robert Leisner is senior software developer at Bodoconsult. See his profile on <http://www.bodoconsult.de/Curriculum_vitae_Robert_Leisner.pdf>.

