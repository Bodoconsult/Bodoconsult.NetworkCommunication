Installation of sample app
==================================

First draft. To be improved

# Recommended folder structure

{Your Target folder}
{Your Target folder}}\{Device}
{Your Target folder}\{Backend}
{Your Target folder}\{Client}

# Other prerequisite

All TCP/IP or UDP ports used for the apps must be opened in the firewall inbound and outbound.

To install device and backend background services you must have local admin permissions on the machine.

# Test device digital twin

# Test device digital twin

The digital twin is only intended for testing the backend and client. For production environments it is NOT required.

## Installation

Install .Net 9 runtime if needed.

Copy binaries to your target folder {Your Target folder}\{Device}.

If the service is installed already stop the Windows service IpDeviceService now.


``` powershell
sc.exe stop "IpDeviceService"
```

Copy binaries to your target folder {Your Target folder}\{Device}.

For a fresh installation register the service now:

``` powershell
sc.exe create "IpDeviceService" binpath= "{Your Target folder}\{Device}\IpDeviceService.exe"
```

Start the Windows service IpDeviceService.

``` powershell
sc.exe start "IpDeviceService"
```

## Adjust appsettings.json

IpAddress: IP address of the device
Port: port to use for UPD comm to device

IpAddress2: IP address of the device
Port2: port to use for TCP/IP comm to device

## Adjust appsettings.json

IpAddress: IP address of the device
Port: port to use for UPD comm to device

IpAddress2: IP address of the device
Port2: port to use for TCP/IP comm to device

# Backend

## Installation

Install .Net 9 runtime if needed. 

If the service is installed already stop the Windows service IpBackendService now.


``` powershell
sc.exe stop "IpBackendService"
```

Copy binaries to your target folder {Your Target folder}\{Backend}.

For a fresh installation register the service now:

``` powershell
sc.exe create "IpBackendService" binpath= "{Your Target folder}\{Backend}\IpBackendService.exe"
```

Start the Windows service IpBackendService.

``` powershell
sc.exe start "IpBackendService"
```

## Adjust appsettings.json

IpAddress: IP address of the device
Port: port to use for UPD comm to client

IpAddress2: IP address of the device
Port2: port to use for TCP/IP comm to device

IpAddress3: IP address of the client
Port3: port to use for TCP/IP comm to client

## Uninstall the service

``` powershell
sc.exe stop "IpBackendService"
sc.exe delete "IpBackendService"
```


# Client

## Installation

Install .Net 9 runtime if needed. 

Copy binaries to your target folder {Your Target folder}\{Client}.

Create a link on desktop for file {Your Target folder}\{Client}\IpClientUi.exe.

## Adjust appsettings.json

IpAddress: IP address of the backend
Port: port to use for TCP/IP comm to client
