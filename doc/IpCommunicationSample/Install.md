Installation of sample app
==================================

First draft. To be improved

# Recommended folder structure

{Your Target folder}
{Your Target folder}/{Device}
{Your Target folder}/{Backend}
{Your Target folder}/{Client}

# Other prerequisite

All TCP/IP or UDP ports used for the apps must be opened in the firewall inbound and outbound.

To install device and backend background services you must have local admin permissions on the machine.

# Device

## Installation

Install .Net 9 runtime if needed

Copy binaries to your target folder {Your Target folder}/{Device}

## Adjust appsettings.json

IpAddress       IP address of the device
Port            UPD port to use

IpAddress2       IP address of the device
Port2           TCP/IP port to use

# Backend

## Installation

Install .Net 9 runtime if needed 

Copy binaries to your target folder {Your Target folder}/{Backend}

## Adjust appsettings.json

Install the service

## Adjust appsettings.json

IpAddress       IP address of the device
Port            UPD port to use

IpAddress2       IP address of the device
Port2           TCP/IP port to use



# Client

## Installation

Install .Net 9 runtime if needed 

Copy binaries to your target folder {Your Target folder}/{Client}

## Adjust appsettings.json

IpAddress       IP address of the backend
Port            TCP/IP port to use



