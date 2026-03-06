Business transaction communication protocol (BTCP)
==========================================

# Overview

Business transaction communication protocol (BTCP) is intended as a protocol to simplify the communication of a client and a backend.

A business transaction is a piece of business logic in the backend you can access remotely via business transaction (BT) number and pass parameters you want to deliver to this BT.

# Communication scheme

![BTCP communication scheme](../../images/BTCP_Communication.png)

BTCP can basically be used fully duplexed. Means client and device/server can initially communication.

# Message format

A BTCP request message is structured as follows

![BTCP data message structure](../../images/BTCP.png)

[STX][0/1]NNN[EOT]XXX[ETX]

A BTCP reply message is structured as follows

STX1NNNEOTXXXETX


STX     Message start (0x2)

0       Request (0x1)

1       Reply (0x1)

NNN     Business transaction number encode as ASCII 0 - 9 (hex 0x38 - 0x39). Use as much digits as you require. Number ends with following EOT.

EOT     End of business transaction number (0x4)

XXX     Minimum 0 bytes of payload. No maximum length defined by BTCP. First byte of payload as char is used to indentity the type of payload

ETX     End of message (0x3)



``` csharp

```