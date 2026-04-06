// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using Avalonia.Controls;
using Bodoconsult.App.Avalonia.ReactiveUI.Converters;
using Bodoconsult.App.Avalonia.ReactiveUI.Menus;
using Bodoconsult.App.Avalonia.ReactiveUI.Regions;
using Bodoconsult.App.Avalonia.ReactiveUI.ViewModels;
using Bodoconsult.App.ReactiveUI.Extensions;
using Bodoconsult.App.ReactiveUI.Interfaces;
using Bodoconsult.App.ReactiveUI.Regions;
using IpClientUi.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace IpClientUi;

public partial class MainWindow : ReactiveWindow<IpClientMainWindowViewModel>, IUiWindow
{
    private AvaloniaUiMenuBuilder? _menuBuilder;
    private MenuControlViewModel? _menuControlViewModel;

    public MainWindow()
    {
        this.Name = nameof(MainWindow);

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

    public void RegisterAllRouterBindings(IpClientMainWindowViewModel viewModel, CompositeDisposable disposables)
    {
        ArgumentNullException.ThrowIfNull(MainMenu.DataContext);

        // Bind WindowState
        this.Bind(viewModel, vm => vm.WindowState,
                view => view.WindowState,
                InlineConverterMethods.FromUiWindowStateToWindowState,
                InlineConverterMethods.FromWindowStateToUiWindowState)
            .DisposeWith(disposables);

        // Get the viemodel of the menu control
        _menuControlViewModel = (MenuControlViewModel)MainMenu.DataContext;

        // Now build the menu
        viewModel.DefineMenuItems();

        _menuBuilder = new AvaloniaUiMenuBuilder(viewModel.TranslationService);
        viewModel.MenuBuilder = _menuBuilder;
        _menuControlViewModel.LoadMenuBuilder(_menuBuilder);
        viewModel.BuildIt();

        // Now set the regions for routing
        RegionManager = viewModel.RegionManager;

        var rm = (AvaloniaRegionManager)viewModel.RegionManager;
        var window = rm.RegisterInstances<MainWindow, IpClientMainWindowViewModel>(this, disposables);

        ArgumentNullException.ThrowIfNull(DocumentRegion.Name);
        ArgumentNullException.ThrowIfNull(MenuRegion.Name);

        viewModel.Region1 = window.FindRegion(DocumentRegion.Name);
        viewModel.Region2 = window.FindRegion(MenuRegion.Name);

        ArgumentNullException.ThrowIfNull(viewModel.Region1);
        ArgumentNullException.ThrowIfNull(viewModel.Region2);

        // Bind regions
        this.OneWayBind<IpClientMainWindowViewModel, MainWindow, RoutingState, RoutingState>(viewModel, p => p.Region1!.Router, xy => xy.DocumentRegion.Router!)
            .DisposeWith(disposables);

        this.OneWayBind<IpClientMainWindowViewModel, MainWindow, RoutingState, RoutingState>(viewModel, p => p.Region2!.Router, xy => xy.MenuRegion.Router!)
            .DisposeWith(disposables);

        //// Bind commands to buttons
        //this.BindCommand<MainWindow, IpClientMainWindowViewModel, ReactiveCommand<Unit, Unit>, Button>(viewModel, x => x.GoToFirstViewCommand, x => x.GoNextButton)
        //    .DisposeWith(disposables);

        //this.BindCommand<MainWindow, IpClientMainWindowViewModel, ReactiveCommand<Unit, Unit>, Button>(viewModel, x => x.GoToWindow1Command, x => x.GoNewWindowButton)
        //    .DisposeWith(disposables);

        //this.BindCommand<MainWindow, IpClientMainWindowViewModel, ReactiveCommand<Unit, Unit>, Button>(viewModel, x => x.GoToWindow1Instance2Command, x => x.GoNewWindowInstance2Button)
        //    .DisposeWith(disposables);

        //this.BindCommand<MainWindow, IpClientMainWindowViewModel, ReactiveCommand<Unit, IRoutableViewModel>, Button>(viewModel, x => x.Region1!.GoBack, x => x.GoBackButton)
        //    .DisposeWith(disposables);

        //var vm2 = new SecondViewModel(viewModel.Region2);

        //viewModel.Region2.Navigate(vm2);
    }

    /// <summary>
    /// Window instance name
    /// </summary>
    public string? InstanceName { get; set; }

    /// <summary>
    /// Current region manager
    /// </summary>
    public IRegionManager? RegionManager { get; private set; }

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

    public void LoadRegionManager(IRegionManager regionManager)
    {
        throw new NotImplementedException();
    }

    public bool IsRegistered { get; }

    ///// <summary>
    ///// Allows the ViewModel to be used on the XAML via a dependency property
    ///// </summary>
    //public static readonly DependencyProperty ViewModelProperty =
    //        DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainWindow),
    //                                    new PropertyMetadata(default(MainViewModel)));
}
