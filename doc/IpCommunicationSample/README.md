IpCommunicationSample
===========================

# What does the sample

IpCommunicationSample is a sample app shwoing how to use Bodoconsult.Network library.

It demonstrates a three layer app containing an IP device, a backend and a client app. Communication between IP device and backend is based on TCP/IP for control tasks and on UDP for data streaming. Communication between backend and client is based on TCP/IP for control tasks and data streaming.

For the TCP/IP communication between IP device and backend IpCommunicationSample is messaging based on the Telnet communication protocol (TNCP).

TCP/IP communication between backend and client is based on bidirectional business transaction communication protocol (BTCP).

Both protocols TNCP and BTCP are simple protocols defined and implemented by Bodoconsult.Network library.

# Basic communication schema for IpCommunicationSample

## TCP/IP communication between IP device and backend

IP device is TCP/IP server and backend is TCP/IP client.

Client sends telnet style commands to IP device. The device answers at least with a handshake and for a few requests with a telnet style command as answer.

### Backend side requests

**GetConfig**: Get configuration (handshake expected, Config answer expected)

**StartStreaming**: Start streaming (handshake expected, no other answer)

**StopStreaming**: Stop streaming (handshake expected, no other answer)

**StartSnapshot**: Start snapshot (handshake expected, no other answer): 

**StopSnapshot**: Stop snapshot (handshake expected, no other answer): 

### Device side requests

None

## TCP/IP communication between backend and client

Backend is TCP/IP server and client is TCP/IP client.

Comm is based on business transactions. Backend uses transactions in the range from 100 to 199 and client in the range from 200 to 299

### Client side requests

**BT200**: Get configuration (handshake and Config reply expected)

**BT201**: Start streaming (handshake expected)

**BT202**: Stop streaming (handshake expected)

### Backend side requests

**BT100**:  Backend and/or device state changed information (No handshake or answer expected)


## How to use the library

The source code contains NUnit test classes the following source code is extracted from. The samples below show the most helpful use cases for the library.

# App start infrastructure basics

# About us

Bodoconsult <http://www.bodoconsult.de> is a Munich based software company from Germany.

Robert Leisner is senior software developer at Bodoconsult. See his profile on <http://www.bodoconsult.de/Curriculum_vitae_Robert_Leisner.pdf>.