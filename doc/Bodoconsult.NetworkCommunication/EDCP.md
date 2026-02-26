Enhanced data communication protocol (EDCP)
==========================================

# Overview



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