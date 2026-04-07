// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.Diagnostics;
using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions.Replies;
using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using IpBackend.Bll.App;
using IpBackend.Bll.BusinessLogic;
using IpBackend.Bll.Interfaces;

namespace IpBackendService.App;

/// <summary>
/// Current implementation of <see cref="IApplicationService"/>
/// </summary>
public class IpBackendServiceService : IApplicationService
{
    private bool _isStopped;
    private bool _isStarting;

    private readonly IAppLoggerProxy _appLogger;
    private IBackendManager _backendManager;

    /// <summary>
    /// Default ctor
    /// </summary>
    public IpBackendServiceService(IAppLoggerProxy appLogger,
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

        _backendManager = AppGlobals.DiContainer.Get<IBackendManager>();

        var startParams = (I3NetworkDevicesAppStartParameter)AppGlobals.AppStartParameter;

        var clientConfig = new IpConfig { IpAddress = startParams.IpAddress3, Port = startParams.Port3 };
        var deviceTcpIpConfig = new IpConfig { IpAddress = startParams.IpAddress2, Port = startParams.Port2 };
        var deviceUdpConfig = new IpConfig { IpAddress = startParams.IpAddress, Port = startParams.Port };

        _backendManager.ClientTcpIpConfig = clientConfig;
        _backendManager.IpDeviceTcpIpConfig = deviceTcpIpConfig;
        _backendManager.IpDeviceUdpConfig = deviceUdpConfig;

        _backendManager.LoadIpDeviceUdp();
        _backendManager.LoadIpDeviceTcpIp();
        _backendManager.LoadClient();

        _backendManager.LoadBusinessTransactions();

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

        DefaultBusinessTransactionReply result;

        // Create log dump on app stop
        try
        {

            // ToDo: fill request with useful information for logging
            result = gms.CreateLogDump(request);

            if (result != null)
            {
                _appLogger?.LogWarning($"CreateLogDump: error code {result.ErrorCode}: {result.Message}");
            }
        }
        catch
        {
            // Do nothing
        }

        // Stop logging now
        try
        {
            if (_appLogger != null)
            {
                _appLogger.StopLogging();
                _appLogger.Dispose();
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