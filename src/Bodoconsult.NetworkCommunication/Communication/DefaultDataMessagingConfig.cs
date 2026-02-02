//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


//using Bodoconsult.NetworkCommunication.Interfaces;
//using Bodoconsult.App.Interfaces;
//using Bodoconsult.NetworkCommunication.Delegates;

//namespace Bodoconsult.NetworkCommunication.Communication;

//public class DefaultDataMessagingConfig: IIpDataMessagingConfig
//{
//    public string LoggerId { get; }
//    public ISocketProxy SocketProxy { get; set; }
//    public IDataMessageProcessingPackage DataMessageProcessingPackage { get; set; }
//    public UpdateDataMessageProcessingPackageDelegate UpdateDataMessageProcessingPackageDelegate { get; set; }
//    public IAppLoggerProxy AppLogger { get; set; }
//    public IAppLoggerProxy MonitorLogger { get; set; }
//    public CheckIfCommunicationIsOnlineDelegate CheckIfCommunicationIsOnlineDelegate { get; set; }
//    public CheckIfDeviceIsReadyDelegate CheckIfDeviceIsReadyDelegate { get; set; }
//    public RaiseComDevCloseRequestDelegate RaiseComDevCloseRequestDelegate { get; set; }
//    public DuplexIoErrorHandlerDelegate DuplexIoErrorHandlerDelegate { get; set; }
//    public RaiseDataMessageNotSentDelegate RaiseDataMessageNotSentDelegate { get; set; }
//    public RaiseDataMessageSentDelegate RaiseDataMessageSentDelegate { get; set; }
//    public RaiseDataMessageReceivedDelegate RaiseCommLayerDataMessageReceivedDelegate { get; set; }
//    public RaiseDataMessageReceivedDelegate RaiseAppLayerDataMessageReceivedDelegate { get; set; }
//    public RaiseUnexpectedDataMessageReceivedDelegate RaiseUnexpectedDataMessageReceivedDelegate { get; set; }

//    /// <summary>
//    /// IP address
//    /// </summary>
//    public string IpAddress { get; set; }

//    /// <summary>
//    /// Port to use for communication
//    /// </summary>
//    public int Port { get; set; }
//}