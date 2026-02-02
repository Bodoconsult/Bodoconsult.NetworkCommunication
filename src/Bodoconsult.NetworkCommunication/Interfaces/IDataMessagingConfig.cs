// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using System.Text.Json.Serialization;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Configuration to use for data messaging
/// </summary>
public interface IDataMessagingConfig
{

    /// <summary>
    /// A readable string for identitying the device used for logging
    /// </summary>
    string LoggerId { get; }

    /// <summary>
    /// Current socket to use
    /// </summary>
    ISocketProxy SocketProxy { get; set; }

    /// <summary>
    /// Data message procssing package
    /// </summary>
    IDataMessageProcessingPackage DataMessageProcessingPackage { get; set; }

    /// <summary>
    /// Update data message processing package
    /// </summary>
    UpdateDataMessageProcessingPackageDelegate UpdateDataMessageProcessingPackageDelegate { get; set; }

    /// <summary>
    /// Current general logger
    /// </summary>
    [JsonIgnore]
    IAppLoggerProxy AppLogger { get; set; }

    /// <summary>
    /// Current monitor logger
    /// </summary>
    [JsonIgnore]
    IAppLoggerProxy MonitorLogger { get; set; }

    #region Communication

    /// <summary>
    /// A delegate for a method returning true if the communications is online or false if offline
    /// </summary>
    /// <returns>A delegate</returns>
    [JsonIgnore]
    CheckIfCommunicationIsOnlineDelegate CheckIfCommunicationIsOnlineDelegate { get; set; }

    /// <summary>
    /// A delegate for a method returning true if the device is or false if not
    /// </summary>
    /// <returns>true if the device is ready else false</returns>
    [JsonIgnore]
    CheckIfDeviceIsReadyDelegate CheckIfDeviceIsReadyDelegate { get; set; }


    /// <summary>
    /// Request a closing of the current communication connection from the business logic delegate
    /// </summary>
    [JsonIgnore]
    RaiseComDevCloseRequestDelegate RaiseComDevCloseRequestDelegate { get; set; }

    /// <summary>
    /// Delegate for handling central exception handling in <see cref="IDuplexIo"/> implementations.
    /// Set internally normally. Public implementation intended for testing purposes.
    /// </summary>
    [JsonIgnore]
    DuplexIoErrorHandlerDelegate DuplexIoErrorHandlerDelegate { get; set; }

    #endregion

    #region Message sending

    /// <summary>
    /// Message not sent delegate
    /// </summary>
    RaiseDataMessageNotSentDelegate RaiseDataMessageNotSentDelegate { get; set; }

    /// <summary>
    /// Message sent delegate
    /// </summary>
    RaiseDataMessageSentDelegate RaiseDataMessageSentDelegate { get; set; }

    #endregion

    #region Receiving messages

    /// <summary>
    /// Delegate fired on comm level if a data message has been received. Should be used in <see cref="ICommunicationHandler"/> impls to implement there handshake responses and then forward to the next layer
    /// </summary>
    RaiseDataMessageReceivedDelegate RaiseCommLayerDataMessageReceivedDelegate { get; set; }

    /// <summary>
    /// Delegate raised on app level if data message was received
    /// </summary>
    RaiseDataMessageReceivedDelegate RaiseAppLayerDataMessageReceivedDelegate { get; set; }


    /// <summary>
    /// Delegate raised if a device message does not fit the expectations (length, content, ...)
    /// </summary>
    RaiseUnexpectedDataMessageReceivedDelegate RaiseUnexpectedDataMessageReceivedDelegate { get; set; }

    #endregion
}