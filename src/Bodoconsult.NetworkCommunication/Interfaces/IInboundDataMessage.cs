// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for byte based inbound messaging containing data i.e. via TCP/IP or UDP
/// </summary>
public interface IInboundDataMessage : IInboundMessage
{
    /// <summary>
    /// Should an acknowledgement be sent if the message is received
    /// </summary>
    bool AnswerWithAcknowledgement { get; }

    /// <summary>
    /// First plausibilty check if a received message can be the expected answer to the request. 
    /// </summary>
    /// <param name="sentMessage">The message sent from the request to the device</param>
    /// <param name="errors">List with error messages to fill</param>
    /// <returns>True if the message was as expected as answer of the sent message else false</returns>
    bool CheckReceivedMessage(IOutboundDataMessage sentMessage, IList<string> errors);
}