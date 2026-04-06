// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Avalonia.Controls;
using Bodoconsult.App.Avalonia.ReactiveUI.Regions;
using Bodoconsult.App.ReactiveUI.Extensions;
using Bodoconsult.App.ReactiveUI.Interfaces;
using Bodoconsult.App.ReactiveUI.Regions;
using IpClientUi.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace IpClientUi.Views;

/// <summary>
/// Interaktionslogik für Window1.xaml
/// </summary>
public partial class Window1 : ReactiveWindow<Window1ViewModel>, IUiWindow
{
    public Window1()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.WhenAnyValue(x => x.ViewModel).ObserveOn(RxSchedulers.MainThreadScheduler).Subscribe(x =>
            {
                if (x == null)
                {
                    return;
                }

                RegisterAllRouterBindings(x, disposables);
            });
        });
    }

    public void RegisterAllRouterBindings(Window1ViewModel viewModel, CompositeDisposable disposables)
    {
        //if (viewModel == null)
        //{
        //    return;
        //}

        var rm = (AvaloniaRegionManager)viewModel.RegionManager;
        var window = rm.RegisterInstances<Window1, Window1ViewModel>(this, disposables);

        ArgumentNullException.ThrowIfNull(DocumentRegion.Name);
        ArgumentNullException.ThrowIfNull(MenuRegion.Name);

        viewModel.Region1 = window.FindRegion(DocumentRegion.Name);
        viewModel.Region2 = window.FindRegion(MenuRegion.Name);

        if (viewModel.Region1 == null)
        {
            ArgumentNullException.ThrowIfNull(viewModel.Region1);
        }

        if (viewModel.Region2 == null)
        {
            ArgumentNullException.ThrowIfNull(viewModel.Region2);
        }

        this.OneWayBind<Window1ViewModel, Window1, RoutingState, RoutingState>(viewModel, p => p.Region1!.Router, xy => xy.DocumentRegion.Router!)
            .DisposeWith(disposables);

        this.OneWayBind<Window1ViewModel, Window1, RoutingState, RoutingState>(viewModel, p => p.Region2!.Router, xy => xy.MenuRegion.Router!)
            .DisposeWith(disposables);

        this.BindCommand<Window1, Window1ViewModel, ReactiveCommand<Unit, Unit>, Button>(viewModel, x => x.GoToSecondViewCommand, x => x.GoNextButton)
            .DisposeWith(disposables);

        //this.BindCommand(viewModel, x => x.GoToWindow1Command, x => x.GoNewWindowButton)
        //    .DisposeWith(disposables);

        this.BindCommand<Window1, Window1ViewModel, ReactiveCommand<Unit, IRoutableViewModel>, Button>(viewModel, x => x.Region1!.GoBack, x => x.GoBackButton)
            .DisposeWith(disposables);

        var vm2 = new SecondViewModel(viewModel.Region2);

        viewModel.Region2.Navigate(vm2);
    }

    /// <summary>
    /// Window instance name
    /// </summary>
    public string? InstanceName { get; set; }

    /// <summary>
    /// Current region manager
    /// </summary>
    public IRegionManager? RegionManager { get; set; }

    /// <summary>
    /// Region in the current window
    /// </summary>
    public List<UiRegion> UiRegions { get; } = [];

    /// <summary>
    /// Dispose this window from region manager
    /// </summary>
    /// <param name="sender">Do not use</param>
    /// <param name="e">Do not use</param>
    public void Dispose(object? sender, EventArgs e)
    {
        RegionManager?.Dispose(this);

        // Clean the event to avoid memory leaking
        try
        {
            Closed -= Dispose;
        }
        catch
        {
            // Do nothing
        }
    }

    /// <summary>
    /// Load the region manager
    /// </summary>
    /// <param name="regionManager">Current region manager instance</param>
    public void LoadRegionManager(IRegionManager regionManager)
    {
        RegionManager = regionManager;
    }
}