

namespace Bodoconsult.NetworkCommunication.Interfaces;

public interface IDataMessageHandshakeFactory
{
    /// <summary>
    /// Get an ACK handshake message
    /// </summary>
    /// <param name="message">Current message received</param>
    /// <returns>ACK handshake message to send</returns>
    IInboundDataMessage GetAckResponse(IInboundDataMessage message);

    /// <summary>
    /// Get a NAK handshake message
    /// </summary>
    /// <param name="message">Current message received</param>
    /// <returns>NAK handshake message to send</returns>
    IInboundDataMessage GetNakResponse(IInboundDataMessage message);

    /// <summary>
    /// Get a CAN handshake message
    /// </summary>
    /// <param name="message">Current message received</param>
    /// <returns>CAN handshake message to send</returns>
    IInboundDataMessage GetCanResponse(IInboundDataMessage message);
}