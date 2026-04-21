// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Interfaces;
using System.Diagnostics;
using Bodoconsult.NetworkCommunication.Interfaces;
using IpDevice.Bll.App;
using IpDevice.Bll.Interfaces;

namespace IpDeviceService.App;

/// <summary>
/// Current implementation of <see cref="IApplicationService"/>
/// </summary>
public class IpDeviceServiceService : IApplicationService
{
    private bool _isStopped;
    private bool _isStarting;

    private readonly IAppLoggerProxy _appLogger;
    private IIpDeviceManager _deviceManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    public IpDeviceServiceService(IAppLoggerProxy appLogger,
        IAppGlobals appGlobals)
    {
        _appLogger = appLogger;
        AppGlobals = appGlobals;
        Offline = false;
    }

    /// <summary>
    /// Request application stop delegate
    /// </summary>
    public RequestApplicationStopDelegate RequestApplicationStopDelegate { get; set; }

    /// <summary>
    /// Current app globals
    /// </summary>
    public IAppGlobals AppGlobals { get; }

    /// <summary>
    /// Application status offline true / false
    /// </summary>
    public bool Offline { get; set; }

    /// <summary>
    /// Register required services like GRPC clients etc.
    /// </summary>
    public void RegisterServices()
    {
        // Do nothing in this demo
    }

    /// <summary>
    /// Start the application
    /// </summary>
    public void StartApplication(CancellationToken? token)
    {
        if (!token.HasValue)
        {
            throw new ArgumentNullException(nameof(token));
        }

        var t = token.Value;


        _isStarting = true;

        if (_isStopped)
        {
            return;
        }

        // Do start your workload here
        var startParams = (I2NetworkDevicesAppStartParameter)AppGlobals.AppStartParameter;

        var deviceTcpIpConfig = new IpConfig { IpAddress = startParams.IpAddress2, Port = startParams.Port2};
        var deviceUdpConfig = new IpConfig { IpAddress = startParams.IpAddress, Port =startParams.Port };

        _deviceManager = AppGlobals.DiContainer.Get<IIpDeviceManager>();
        _deviceManager.BackendTcpIpConfig = deviceTcpIpConfig;
        _deviceManager.BackendUdpConfig = deviceUdpConfig;

        _deviceManager.LoadBackendUdp();
        _deviceManager.LoadBackendTcpIp();
        _deviceManager.LoadBusinessTransactions();

        ArgumentNullException.ThrowIfNull(_deviceManager.BackendUdp);
        ArgumentNullException.ThrowIfNull(_deviceManager.BackendTcpIp);
        ArgumentNullException.ThrowIfNull(_deviceManager.BackendUdp.IpDevice);
        ArgumentNullException.ThrowIfNull(_deviceManager.BackendTcpIp.IpDevice);

        _deviceManager.BackendUdp.IpDevice.StartComm();
        _deviceManager.BackendTcpIp.IpDevice.StartComm();

        _isStarting = false;

        while (!t.IsCancellationRequested)
        {
            Task.Delay(500, t);
        }
    }

    /// <summary>
    /// Suspend the app
    /// </summary>
    public void SuspendApplication()
    {
        if (_isStopped)
        {
            return;
        }

        // Clear DI container: more sophisticated implementation for single instances in the container may be required
        var di = Globals.Instance.DiContainer;
        di.ClearAll();
    }

    /// <summary>
    /// Restart the app if it is in suspend state
    /// </summary>
    public void RestartApplication()
    {
        //if (_isStopped)
        //{
        //    return;
        //}

        //if (_isStarting)
        //{
        //    return;
        //}

        // ToDo: RL: Restart app
    }

    private void OnLicenseMissingEvent(object sender, LicenseMissingArgs e)
    {
        var msg = $"License is missing: {e.ErrorMessage}. Application will stop";
        _appLogger.LogError(msg);
        StopApplication();

        LicenseMissingDelegate?.Invoke(msg);
    }

    /// <summary>
    /// Stop the application
    /// </summary>
    public void StopApplication()
    {
        _appLogger.LogWarning("Stopping application starts...");

        // Start process running? If yes leave here
        if (_isStarting)
        {
            return;
        }

        // Stop the communicatiion
        _deviceManager.BackendUdp?.IpDevice?.StartComm();
        _deviceManager.BackendTcpIp?.IpDevice?.StartComm();

        // Do not stop more than one time
        if (_isStopped)
        {
            return;
        }

        _isStopped = true;
        
        // Do all needed to stop youe app correctly
        var di = Globals.Instance.DiContainer;

        try
        {
            // Stop performance logging
            var perflog = di.Get<IPerformanceLoggerManager>();
            perflog?.StopLogging();
        }
        catch (Exception e)
        {
            Debug.Print(e.Message);
            //_appLogger.LogError($"Performance logging could not be stopped", new object[]{e});
        }

        var gms = di.Get<IGeneralAppManagementManager>();
        var request = new EmptyBusinessTransactionRequestData();

        // Create log dump on app stop
        try
        {
            // ToDo: fill request with useful information for logging
            var result = gms.CreateLogDump(request);

            if (result != null && result.ErrorCode != 0)
            {
                _appLogger?.LogWarning($"CreateLogDump: error code {result.ErrorCode}: {result.Message}");
            }
        }
        catch
        {
            // Do nothing
        }
    }

    /// <summary>
    /// Current <see cref="IApplicationService.LicenseMissingDelegate"/>
    /// </summary>
    public LicenseMissingDelegate LicenseMissingDelegate { get; set; }
}