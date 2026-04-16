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
public partial class StopMessagingViewModel : ReactiveObject, IUiRegionViewModel
{
    private readonly IBusinessTransactionManager _businessTransactionManager;

    /// <summary>
    /// Gets a string token representing the current ViewModel, such as 'login' or 'user'.
    /// </summary>
    public string UrlPathSegment => "stopMessaging";

    /// <summary>
    /// Gets the IScreen that this ViewModel is currently being shown in. This
    /// is usually passed into the ViewModel in the Constructor and saved
    /// as a ReadOnly Property.
    /// </summary>
    public IScreen HostScreen { get; private set; } = new DummyScreen();

    public StopMessagingViewModel()
    { 
        _businessTransactionManager = Globals.Instance.DiContainer.Get<IBusinessTransactionManager>();
    }

    // ToDO : remove service locator patetrn for BT manager

    public StopMessagingViewModel(IScreen screen)
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

    [ReactiveCommand]
    public IObservable<Unit> StopMessaging()
    {
        var request = new EmptyBusinessTransactionRequestData
        {
            TransactionId = ClientSideBusinessTransactionIds.StopStreaming
        };

        _businessTransactionManager.RunBusinessTransaction(request.TransactionId, request);

        return Observable.Return(Unit.Default);
    }
}