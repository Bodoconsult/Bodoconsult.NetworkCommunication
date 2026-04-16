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

## Installation

Install .Net 9 runtime if needed.

Copy binaries to your target folder {Your Target folder}\{Device}.

## Adjust appsettings.json

IpAddress: IP address of the device
Port: port to use for UPD comm to device

IpAddress2: IP address of the device
Port2: port to use for TCP/IP comm to device

# Backend

## Installation

Install .Net 9 runtime if needed. 

If the service is installed already stop the Windows service SfxBackendService now.


``` powershell
sc.exe stop "SfxBackendService"
```

Copy binaries to your target folder {Your Target folder}\{Backend}.

For a fresh installation register the service now:

``` powershell
sc.exe create "SfxBackendService" binpath= "{Your Target folder}\{Backend}\IpBackendService.exe"
```

Start the Windows service SfxBackendService.

``` powershell
sc.exe start "SfxBackendService"
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
sc.exe stop "SfxBackendService"
sc.exe delete "SfxBackendService"
```

# Client

## Installation

Install .Net 9 runtime if needed. 

Copy binaries to your target folder {Your Target folder}\{Client}.

## Adjust appsettings.json

IpAddress: IP address of the backend
Port: port to use for TCP/IP comm to client
