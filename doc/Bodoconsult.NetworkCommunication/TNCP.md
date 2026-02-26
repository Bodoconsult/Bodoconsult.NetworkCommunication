Telnet communication protocol (TNCP)
==========================================

# Overview

Telnet communication protocol is a IP based protocol sending commands nearly the same way as from a telnet console.

# Message format

A TNCP messages are structured as follows:

XXXCR

There is no difference between a request and a reply. This may limit the potential use cases.

XXX    Minimum 1 byte of payload. No maximum length defined by TNCP. XXX is interpreted as ASCII string. If it contains commas, the first token is interpreted as command, the rest of the tokens as parameters for this command. If there are no commas the whole string is interpreted as command.

CR     Carriage return (0xd)

Sample:

log,chstat[CR]

``` csharp

```