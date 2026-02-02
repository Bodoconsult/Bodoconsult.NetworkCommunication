// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessagingConfig;

/// <summary>
/// Default config file for one the client-server network communication with one client device
/// </summary>
public class DefaultDataMessagingConfig: IIpDataMessagingConfig
{

    /// <summary>
    /// A readable string for identitying the device used for logging
    /// </summary>
    public string LoggerId => "TestDevice";

    /// <summary>
    /// Current socket to use
    /// </summary>
    public ISocketProxy SocketProxy { get; set; }

    /// <summary>
    /// Data message procssing package
    /// </summary>
    public IDataMessageProcessingPackage DataMessageProcessingPackage { get; set; }

    /// <summary>
    /// Update data message processing package
    /// </summary>
    public UpdateDataMessageProcessingPackageDelegate UpdateDataMessageProcessingPackageDelegate { get; set; }

    /// <summary>
    /// Current general logger
    /// </summary>
    public IAppLoggerProxy AppLogger { get; set; }

    /// <summary>
    /// Current monitor logger
    /// </summary>
    public IAppLoggerProxy MonitorLogger { get; set; }

    /// <summary>
    /// A delegate for a method returning true if the communications is online or false if offline
    /// </summary>
    /// <returns>A delegate</returns>
    public CheckIfCommunicationIsOnlineDelegate CheckIfCommunicationIsOnlineDelegate { get; set; }

    /// <summary>
    /// A delegate for a method returning true if the device is or false if not
    /// </summary>
    /// <returns>true if the device is ready else false</returns>
    public CheckIfDeviceIsReadyDelegate CheckIfDeviceIsReadyDelegate { get; set; }

    /// <summary>
    /// Request a closing of the current communication connection from the business logic delegate
    /// </summary>
    public RaiseComDevCloseRequestDelegate RaiseComDevCloseRequestDelegate { get; set; }

    /// <summary>
    /// Delegate for handling central exception handling in <see cref="IDuplexIo"/> implementations.
    /// Set internally normally. Public implementation intended for testing purposes.
    /// </summary>
    public DuplexIoErrorHandlerDelegate DuplexIoErrorHandlerDelegate { get; set; }

    /// <summary>
    /// Message not sent delegate
    /// </summary>
    public RaiseDataMessageNotSentDelegate RaiseDataMessageNotSentDelegate { get; set; }

    /// <summary>
    /// Message sent delegate
    /// </summary>
    public RaiseDataMessageSentDelegate RaiseDataMessageSentDelegate { get; set; }

    /// <summary>
    /// Delegate fired on comm level if a data message has been received. Should be used in <see cref="ICommunicationHandler"/> impls to implement there handshake responses and then forward to the next layer
    /// </summary>
    public RaiseDataMessageReceivedDelegate RaiseCommLayerDataMessageReceivedDelegate { get; set; }

    /// <summary>
    /// Delegate raised on app level if data message was received
    /// </summary>
    public RaiseDataMessageReceivedDelegate RaiseAppLayerDataMessageReceivedDelegate { get; set; }

    /// <summary>
    /// Delegate raised if a device message does not fit the expectations (length, content, ...)
    /// </summary>
    public RaiseUnexpectedDataMessageReceivedDelegate RaiseUnexpectedDataMessageReceivedDelegate { get; set; }

    /// <summary>
    /// IP address of the device
    /// </summary>
    public string IpAddress { get; set; } = "127.0.0.1";

    /// <summary>
    /// Port to use for device communication
    /// </summary>
    public int Port { get; set; } = 9000;
}