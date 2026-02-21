

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for validating customer specific data message.
/// </summary>
public interface IDataMessageValidator
{
    /// <summary>
    /// Check if a data message is valid data message to be processed
    /// </summary>
    /// <param name="dataMessage">Received data message</param>
    /// <returns>True if the message was the handshake for the sent message</returns>
    DataMessageValidatorResult IsMessageValid(IInboundDataMessage dataMessage);

}