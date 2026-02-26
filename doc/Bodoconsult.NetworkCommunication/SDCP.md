Simple data communication protocol (SDCP)
==========================================

# Overview



# Message format

A SDCP messages are structured as follows:

STX[BlockCode]XXXETX

There is no difference between a request and a reply. This may limit the potential use cases.


STX     Message start (0x2)

XXX     Minimum 0 bytes of payload. No maximum length defined by EDCP. First byte of payload as char is used to indentity the type of payload

ETX     End of message (0x3)



``` csharp

```