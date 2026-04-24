// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BusinessTransactions.RequestData;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.ReactiveUI.Interfaces;
using IpClientUi.Interfaces;
using IpCommunicationSample.Common.BusinessTransactions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using System.Reactive;
using System.Reactive.Linq;

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

    /// <summary>
    /// Command starting the messaging
    /// </summary>
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