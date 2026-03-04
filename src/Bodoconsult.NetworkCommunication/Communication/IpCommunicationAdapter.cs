// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Microsoft.Diagnostics.Utilities;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Bodoconsult.NetworkCommunication.Communication;

/// <summary>
/// Current IP based implementation of <see cref="ICommunicationAdapter"/>
/// </summary>
public class IpCommunicationAdapter : ICommunicationAdapter
{
    private ICommunicationHandler _communicationHandler;
    private readonly ICommunicationHandlerFactory _communicationHandlerFactory;
    private readonly IOutboundDataMessageFactory _outboundDataMessageFactory;
    private IDeviceState _deviceState = DefaultDeviceStates.DeviceStateOffline;
    private readonly Lock _comDevActionLockObject = new();

    /// <summary>
    /// Is a ComDev action in progress. DO NOT ACCESS directly. Use <see cref="IsComDevActionInProgress"/> instead
    /// </summary>
    private bool _isComDevActionInProgress;

    private int _errorCounter;


    /// <summary>
    /// Default ctor
    /// </summary>
    public IpCommunicationAdapter(IIpDataMessagingConfig dataMessagingConfig,
        ICommunicationHandlerFactory communicationHandlerFactory,
        IOutboundDataMessageFactory outboundDataMessageFactory)
    {
        DataMessagingConfig = dataMessagingConfig;
        _communicationHandlerFactory = communicationHandlerFactory;
        _outboundDataMessageFactory = outboundDataMessageFactory;

        ////DataMessagingConfig.GetTowerStateDelegate = GetTowerState;
        DataMessagingConfig.CheckIfCommunicationIsOnlineDelegate = CheckIfCommunicationIsOnline;
        //DataMessagingConfig.CheckIfDeviceIsReadyDelegate = CheckIfDeviceIsReady;

        DataMessagingConfig.RaiseComDevCloseRequestDelegate = OnRequestComDevClose;
        DataMessagingConfig.ResetOutboundDataMessageFactoryDelegate = _outboundDataMessageFactory.Reset;
    }

    /// <summary>
    /// Check if the tower comm is online
    /// </summary>
    /// <returns>True if the communication is online else false</returns>
    public bool CheckIfCommunicationIsOnline()
    {
        return _deviceState != DefaultDeviceStates.DeviceStateOffline;
    }

    /// <summary>
    /// Is a COM DEV operation running currently
    /// </summary>

    public bool IsComDevActionInProgress
    {
        get
        {
            lock (_comDevActionLockObject)
            {
                return _isComDevActionInProgress;
            }

        }
        private set
        {
            lock (_comDevActionLockObject)
            {
                _isComDevActionInProgress = value;
            }
        }
    }

    /// <summary>
    /// Set the order processing state delegate
    /// </summary>
    public SetOrderProcessingStateDelegate SetOrderProcessingStateDelegate { get; set; }

    /// <summary>
    ///     This property returns the value of the Error-Byte in
    ///     the message coming from Tower
    /// </summary>
    public int Error { get; set; }

    /// <summary>
    /// This property returns whether the communication object is valid and can be used
    /// </summary>
    public bool IsCommunicationHandlerNotNull => _communicationHandler != null;

    /// <summary>
    /// Is the adapter connected
    /// </summary>
    public bool IsConnected => IsFakeSendingActivated || (_communicationHandler?.IsConnected ?? false);

    /// <summary>
    /// This is the last state received from tower
    /// </summary>
    public string StatusRequestReceivedState { get; set; }

    /// <summary>
    /// This property has to be set to false for production. Is only for testing
    /// </summary>
    public bool IsFakeSendingActivated { get; set; }

    /// <summary>
    /// Device configuration for data messaging
    /// </summary>
    public IIpDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Send a data message to the device 
    /// </summary>
    /// <param name="command">Command to send</param>
    /// <returns>Reply of the device</returns>
    public MessageSendingResult SendDataMessage(IOutboundDataMessage command)
    {
        return _communicationHandler.SendMessage(command);
    }

    /// <summary>
    /// Cancel the currently running operation on the device
    /// </summary>
    public virtual MessageSendingResult CancelRunningOperation()
    {
        throw new NotSupportedException("Override in derived class");
    }

    /// <summary>
    /// Reset the com dev without breaking the communication
    /// </summary>
    public void ResetInternalState()
    {
        IsComDevActionInProgress = false;
    }

    /// <summary>
    /// Initialize the communication with the tower
    /// </summary>
    /// <returns>True if the initialiazation was successfull else false</returns>
    public bool ComDevInit()
    {
        //Debug.Print("ComDevInit");

        string msg;

        try
        {
            if (IsComDevActionInProgress)
            {
                DataMessagingConfig.AppLogger.LogWarning($"{DataMessagingConfig.LoggerId}ComDevInit not possible due to another call to ComDevInit or ComDevClose in progress");
                return IsCommunicationHandlerNotNull;
            }

            if (IsCommunicationHandlerNotNull)
            {
                msg = "ComDev for device is not valid. Request ComDevClose";
                DataMessagingConfig.MonitorLogger.LogWarning(msg);
                DataMessagingConfig.AppLogger.LogWarning($"{DataMessagingConfig.LoggerId}{msg}");
                ComDevCloseInternal();
            }

            var isCloseRequired = false;

            try
            {
                IsComDevActionInProgress = true;

                // Stop order handling now in case it is still running 
                SetOrderProcessingStateDelegate?.Invoke(false);

                InitCommunicationObjects();
                //Debug.Print("ComDevInit successful");

                msg = "ComDevInit successful";
                DataMessagingConfig.MonitorLogger.LogInformation(msg);
                DataMessagingConfig.AppLogger.LogInformation($"{DataMessagingConfig.LoggerId}{msg}");
            }
            catch (SocketException se)
            {
                // Logging only every 10 comm checks to keep log file tiny
                _errorCounter++;

                if (_errorCounter == 1 //|| PreviousTowerState != StSysTowerHardwareState.TowerStateOffline
                   )
                {
                    msg = $"socket connection to {DataMessagingConfig.IpAddress}:{DataMessagingConfig.Port} failed: error code {se.ErrorCode}. Request ComDevClose";
                    DataMessagingConfig.MonitorLogger.LogWarning(msg);
                    DataMessagingConfig.AppLogger.LogWarning($"{DataMessagingConfig.LoggerId}{msg}");
                }

                if (_errorCounter > 10)
                {
                    _errorCounter = 0;
                }

                isCloseRequired = true;

            }
            catch (Exception e)
            {
                if (e.StackTrace != null && e.StackTrace.Contains("Socket"))
                {
                    msg = $"ComDev init to {DataMessagingConfig.IpAddress}:{DataMessagingConfig.Port} failed. Request ComDevClose";
                }
                else
                {
                    msg = $"ComDev init to {DataMessagingConfig.IpAddress}:{DataMessagingConfig.Port} failed: {e.Message} {e.StackTrace}. Request ComDevClose";
                }

                DataMessagingConfig.MonitorLogger.LogWarning(msg);
                DataMessagingConfig.AppLogger.LogWarning($"{DataMessagingConfig.LoggerId}{msg}");

                isCloseRequired = true;
            }
            finally
            {
                IsComDevActionInProgress = false;
                SetOrderProcessingStateDelegate?.Invoke(true);
            }

            // Leave here if no error happened before
            if (!isCloseRequired)
            {
                _deviceState = DefaultDeviceStates.DeviceStateOnline;
                return IsCommunicationHandlerNotNull;
            }

            // Close tower comm otherwise
            ComDevCloseInternal(true);

            IsComDevActionInProgress = false;
            SetOrderProcessingStateDelegate?.Invoke(true);

        }
        catch (Exception e)
        {
            msg = "ComDevInit failed";
            DataMessagingConfig.MonitorLogger.LogError(msg, e);
            DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}", e);

            _communicationHandler = null;
            IsComDevActionInProgress = false;
            SetOrderProcessingStateDelegate?.Invoke(true);
        }

        return IsCommunicationHandlerNotNull;
    }

    /// <summary>
    /// Method for closing the communication channel with the tower
    /// </summary>
    public void ComDevClose()
    {
        if (IsComDevActionInProgress)
        {
            return;
        }

        ComDevCloseInternal();
        IsComDevActionInProgress = false;
    }

    private void ComDevCloseInternal(bool noLogging = false)
    {
        if (IsComDevActionInProgress)
        {
            return;
        }

        SetOrderProcessingStateDelegate?.Invoke(false);

        if (_communicationHandler == null)
        {
            SetOrderProcessingStateDelegate?.Invoke(true);
            IsComDevActionInProgress = false;
            return;
        }

        try
        {
            IsComDevActionInProgress = true;

            if (_communicationHandler.IsConnected)
            {
                _communicationHandler.Disconnect();
                DataMessagingConfig.ResetOutboundDataMessageFactoryDelegate?.Invoke();
            }

            _communicationHandler.Dispose();
        }
        catch (Exception e)
        {
            // Do nothing
            DataMessagingConfig.MonitorLogger.LogError("error occured while trying to close the communication ", e);
        }
        finally
        {
            // RL: Comm handler has to be reset in every case!!!!!!!
            _communicationHandler = null;

            SetOrderProcessingStateDelegate?.Invoke(true);

            IsComDevActionInProgress = false;

            // Handle the communication break on higher business logic levels
            DataMessagingConfig.ResetOutboundDataMessageFactoryDelegate?.Invoke();

            // ReSharper disable once InvertIf
            if (!noLogging)
            {
                const string msg = "ComDevClose: all steps performed";
                DataMessagingConfig.MonitorLogger.LogError(msg);
                DataMessagingConfig.AppLogger.LogError($"{DataMessagingConfig.LoggerId}{msg}");
                //Debug.Print($"ComDevClose: tower state {TowerState}");
                //DataMessagingConfig.AppLogger.LogInformation($"{DataMessagingConfig.LoggerId}ComDevClose: all steps performed ");
            }
        }
    }


    private void InitCommunicationObjects()
    {
        if (_communicationHandler != null)
        {
            _communicationHandler.Disconnect();
            _communicationHandler.Dispose();
            _communicationHandler = null;
        }

        _communicationHandler = _communicationHandlerFactory.CreateInstance(DataMessagingConfig);
        _communicationHandler.Connect();

        var connectionEstablishedMessage = $"{DataMessagingConfig.LoggerId}connection to {DataMessagingConfig.IpAddress}:{DataMessagingConfig.Port} established: {_communicationHandler?.IsConnected}.";
        DataMessagingConfig.AppLogger.LogInformation(connectionEstablishedMessage);
    }

    private void OnRequestComDevClose(string requestSource)
    {
        try
        {
            DataMessagingConfig.AppLogger.LogWarning($"{DataMessagingConfig.LoggerId}CLOSE requested for connection because of error while reading bytes on socket. Close reason: {requestSource}.");
            ComDevClose();
        }
        catch (Exception e)
        {
            DataMessagingConfig.AppLogger.LogError("ComDevClose failed", e);
        }
    }

    private readonly Dictionary<string, Ping> _pingInstances = new();

    /// <summary>
    /// Is the tower pingable. Each IP address uses its own PING instance
    /// </summary>
    /// <returns>True if the tower is pingable</returns>
    public async Task<bool> IsPingableAsync()
    {
        var ipAddress = DataMessagingConfig.IpAddress;

        // Do not ping localhost
        if (ipAddress == "127.0.0.1")
        {
            return true;
        }

        var ipObject = IPAddress.Parse(ipAddress);

        PingReply pingResult;

        //lock (PingLock)
        {
            var success = _pingInstances.TryGetValue(ipAddress, out var ping);

            if (!success)
            {
                ping = new Ping();
                _pingInstances.Add(ipAddress, ping);
            }

            pingResult = await ping.SendPingAsync(ipObject, DeviceCommunicationBasics.PingTimeout);

        }

        return pingResult is { Status: IPStatus.Success };
    }

    /// <summary>
    /// The timestamp of the last message received
    /// </summary>
    public long LastMessageTimeStamp => DataMessagingConfig.DataMessageProcessingPackage?.WaitStateManager?.LastMessageTimeStamp ?? 0;

    /// <summary>
    /// Reset the com dev to a defined state as if there were never a communication with the tower. No logging for ComDevClose activated
    /// </summary>
    public void ComDevReset()
    {
        ComDevCloseInternal(true);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _communicationHandler?.Dispose();
    }
}