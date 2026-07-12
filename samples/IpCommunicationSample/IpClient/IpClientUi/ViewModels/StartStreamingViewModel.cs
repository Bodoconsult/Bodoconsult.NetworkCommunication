// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Reactive;
using System.Reactive.Linq;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.ReactiveUI.Interfaces;
using Bodoconsult.App.ReactiveUI.Regions;
using IpClientUi.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using IpCommunicationSample.Common.BusinessTransactions.Requests;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace IpClientUi.ViewModels;

/// <summary>
/// View model for stop messaging dialog
/// </summary>
public partial class StartMessagingViewModel : ReactiveObject, IUiRegionViewModel, IStartMessagingViewModel
{
    private readonly IBusinessTransactionManager _businessTransactionManager;

    /// <summary>
    /// Gets a string token representing the current ViewModel, such as 'login' or 'user'.
    /// </summary>
    public string UrlPathSegment => "startMessaging";

    /// <summary>
    /// Gets the IScreen that this ViewModel is currently being shown in. This
    /// is usually passed into the ViewModel in the Constructor and saved
    /// as a ReadOnly Property.
    /// </summary>
    public IScreen HostScreen { get; private set; } = new DummyScreen();

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="businessTransactionManager">Current business transaction manager</param>
    public StartMessagingViewModel(IBusinessTransactionManager businessTransactionManager)
    {
        _businessTransactionManager = businessTransactionManager;
        StartMessagingCommand.InvokeCommand(BackCommand);
        CollectionInterval = 5000;
        CollectionTime = 1000;
    }

    /// <summary>
    /// Method based late injection of <see cref="ReactiveUI.IScreen"/> instance for navigation
    /// </summary>
    /// <param name="screen"></param>
    public void InjectScreen(UiRegion screen)
    {
        HostScreen = screen;
        UiRegion = screen;
    }

    /// <summary>
    /// UI region the viewmodel is loaded in
    /// </summary>
    public UiRegion? UiRegion { get; private set; }

    /// <summary>
    /// Is the request for snapshot (false) or streaming (true)
    /// </summary>
    [Reactive] public partial bool Snapshot { get; set; }

    /// <summary>
    /// Show channel 1?
    /// </summary>
    [Reactive] public partial bool Channel1 { get; set; }

    /// <summary>
    /// Show channel 2?
    /// </summary>
    [Reactive] public partial bool Channel2 { get; set; }

    /// <summary>
    /// Show channel 3?
    /// </summary>
    [Reactive] public partial bool Channel3 { get; set; }

    /// <summary>
    /// Show channel 4?
    /// </summary>
    [Reactive] public partial bool Channel4 { get; set; }

    /// <summary>
    /// Use a software defined snapshot
    /// </summary>
    [Reactive] public partial bool UseSoftwareSnapshot { get; set; }

    /// <summary>
    /// Use a software defined snapshot
    /// </summary>
    [Reactive] public partial bool IsDataLoggingActivated { get; set; }

    /// <summary>
    /// Is data presentation as chart activated?
    /// </summary>
    [Reactive] public partial bool IsChartActivated { get; set; }

    /// <summary>
    /// Data collection time interval in ms
    /// </summary>
    [Reactive] public partial int CollectionInterval { get; set; } 

    /// <summary>
    /// Data collection time period in ms
    /// </summary>
    [Reactive] public partial int CollectionTime { get; set; }

    /// <summary>
    /// Command starting the messaging
    /// </summary>
    [ReactiveCommand]
    public IObservable<Unit> StartMessaging()
    {
        return Observable.Start(() =>
        {
            if (Channel1 == false && Channel2 == false && Channel3 == false && Channel4 == false)
            {
                Channel1 = true;
            }

            // ToDo: remove this condition if chart is implemented
            if (IsChartActivated && !IsDataLoggingActivated)
            {
                IsChartActivated = false;
                IsDataLoggingActivated = true;
            }

            if (!IsChartActivated && !IsDataLoggingActivated)
            {
                IsDataLoggingActivated = true;
            }

            if (CollectionInterval <= 0)
            {
                CollectionInterval = 5000;
            }

            if (CollectionTime <= 0)
            {
                CollectionTime = 1000;
            }

            var request = new StartMessagingBusinessTransactionRequestData
            {
                TransactionId = ClientSideBusinessTransactionIds.StartMessaging,
                Snapshot = Snapshot,
                Channel1 = Channel1,
                Channel2 = Channel2,
                Channel3 = Channel3,
                Channel4 = Channel4,
                IsDataLoggingActivated = IsDataLoggingActivated,
                IsChartActivated = IsChartActivated,
                UseSoftwareSnapshot = UseSoftwareSnapshot,
                CollectionInterval = CollectionInterval,
                CollectionTime = CollectionTime
            };

            var reply = _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);

            if (reply.ErrorCode == 0|| UiRegion==null)
            {
                return;
            }

            UiRegion.UiWindow.ShowInfoDialog(reply.Message);
        });
    }

    /// <summary>
    /// Command going back
    /// </summary>
    [ReactiveCommand]
    public IObservable<Unit> Back()
    {
        HostScreen.Router.NavigateBack.Execute(Unit.Default);
        return Observable.Return(Unit.Default);
    }
}