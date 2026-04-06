// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Wpf.Interfaces;
using Bodoconsult.App.Wpf.ReactiveUI.Interfaces;
using ReactiveUI;
using System.Reactive.Disposables.Fluent;
using WpfReactiveUiDemoApp.ViewModels;

namespace WpfReactiveUiDemoApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow: IActivatableView
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void InjectViewModel(IReactiveUiMainWindowViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel)) ;

        var rm = ViewModel.RegionManager;

        RoutedViewHost v;

        //var region1 = new WpfUiRegion(this.DocumentRegion);
        //rm.RegisterRegion(region1);

        //this.WhenActivated(disposables =>
        //{
        //    // Bind the view model router to RoutedViewHost.Router property.
        //    this.OneWayBind(region1, x => x.Router, x => x.DocumentRegion.Router)
        //        .DisposeWith(disposables);
        //});


        //rm.Navigate(nameof(this.DocumentRegion), new ViewModel1(vw));


    }
}