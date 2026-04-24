// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.ReactiveUI.Interfaces;
using IpClient.Bll.App;
using IpCommunicationSample.Common.BusinessTransactions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive;
using System.Reactive.Linq;

namespace IpClientUi.ViewModels;

/// <summary>
/// View model for stop messaging dialog
/// </summary>
public partial class StartMessagingViewModel : ReactiveObject, IUiRegionViewModel
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

    public StartMessagingViewModel()
    {
        _businessTransactionManager = Globals.Instance.DiContainer.Get<IBusinessTransactionManager>();
    }

    // ToDO : remove service locator pattern for BT manager

    public StartMessagingViewModel(IScreen screen)
    {
        HostScreen = screen;
        _businessTransactionManager = Globals.Instance.DiContainer.Get<IBusinessTransactionManager>();
    }

    /// <summary>
    /// Method based late injection of <see cref="ReactiveUI.IScreen"/> instance for navigation
    /// </summary>
    /// <param name="screen"></param>
    public void InjectScreen(IScreen screen)
    {
        HostScreen = screen;
    }

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

    [ReactiveCommand]
    public IObservable<Unit> StartMessaging()
    {
        return Observable.Start(() =>
        {
            var request = new EmptyBusinessTransactionRequestData
            {
                TransactionId = Snapshot ? ClientSideBusinessTransactionIds.StartStreaming : ClientSideBusinessTransactionIds.StopSnapshot
            };

            _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);
        });
    }
}