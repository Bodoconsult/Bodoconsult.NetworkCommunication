Enhanced data communication protocol (EDCP)
==========================================

# Overview

Basically the EDCP protocol is same as SDCP protocol but the second byte of each message is a a block code. Client and server use different number ranges for the block code. Let's say server uses block codes from 1 to 20 and client from 21 up to 40. If each party answers a received data message with a handshake it adds the block code received with the data nessage. So the sender of a data message can recognize the handshake received for the sent message clarly.

Another enhancement of EDCP protocl is byte 3 may contain a block code of a requesting data message. This enhancement makes it possible to implemenent data message requests answer by the other side by one or more data messages. If there is no block code for byte 3 delivered it means a data message sent without a request from the other side.

# Message format

A EDCP request message is structured as follows

STX[BlockCode]XXXETX

Requests and replies are structured the same way. The only difference is: a reply gets the block code of the request.


STX     Message start (0x2)

[BlockCode] 1 byte set by the side starting a request. Client and server use separate number ranges. Defaults: server 0 - 127 client 128 - 255. Is the message a reply to a request, the reply gets the blockcode of the request.

XXX     Minimum 0 bytes of payload. No maximum length defined by EDCP. First byte of payload as char is used to indentity the type of payload

ETX     End of message (0x3)



``` csharp

```