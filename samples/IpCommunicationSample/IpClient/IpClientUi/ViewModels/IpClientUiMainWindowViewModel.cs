// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Avalonia.Controls;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.ReactiveUI.Extensions;
using Bodoconsult.App.ReactiveUI.Interfaces;
using Bodoconsult.App.ReactiveUI.Menus;
using Bodoconsult.App.ReactiveUI.Ui;
using Bodoconsult.App.ReactiveUI.ViewModels;
using ReactiveUI.SourceGenerators;
using System.Reactive;
using System.Reactive.Linq;
using System.Transactions;

namespace IpClientUi.ViewModels;

/// <summary>
/// ViewModel for MainWindow window
/// </summary>
public partial class IpClientMainWindowViewModel : MainWindowViewModel
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="listener">Current app event listener</param>
    /// <param name="translationService">Translation service</param>
    /// <param name="regionManager">Region manager</param>
    public IpClientMainWindowViewModel(IAppEventListener listener, II18N translationService,
        IRegionManager regionManager) : base(listener, translationService, regionManager)
    { }

    /// <summary>
    /// Create the main form of the application
    /// </summary>
    /// <returns></returns>
    public override Window CreateWindow()
    {
        var w = new MainWindow
        {
            ViewModel = this,
            IsVisible = true,
        };

        WindowState = UiWindowState.Maximized;
        return w;
    }

    /// <summary>
    /// Define the menu items to be stored in <see cref="IUiMenuWindow.MenuItems"/>
    /// </summary>
    public override void DefineMenuItems()
    {
        var groupItem = new GroupUiMenuItem("SFX receiver");
        MenuItems.Add(groupItem);

        var command1 = new CommandUiMenuItem("Start streaming")
        {
            CommandDefinition = new UiCommandDefinition(GoToStartStreaming, null)
        };

        groupItem.AddChild(command1);

        var command2 = new CommandUiMenuItem("Stop streaming")
        {
            CommandDefinition = new UiCommandDefinition(GoToStopStreaming, null)
        };

        groupItem.AddChild(command2);


        var command3 = new CommandUiMenuItem("Start snapshot")
        {
            CommandDefinition = new UiCommandDefinition(GoToStartSnapshot, null)
        };

        groupItem.AddChild(command3);

        var command4 = new CommandUiMenuItem("Stop snapshot")
        {
            CommandDefinition = new UiCommandDefinition(GoToStopSnapshot, null)
        };

        groupItem.AddChild(command4);

    }

    // Async commands 

    /// <summary>
    /// Async version of the command
    /// </summary>
    /// <returns></returns>
    [ReactiveCommand]
    public IObservable<Unit> GoToStartStreaming()
    {
        ArgumentNullException.ThrowIfNull(Region1);

        try
        {
            var vm = new StartMessagingViewModel(Region1)
            {
                Snapshot = false
            };
            Region1?.Navigate(vm);
            return Observable.Return(Unit.Default);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [ReactiveCommand]
    public IObservable<Unit> GoToStopStreaming()
    {
        ArgumentNullException.ThrowIfNull(Region1);

        try
        {
            var vm = new StopMessagingViewModel(Region1);
            vm.InjectScreen(Region1);
            Region1?.Navigate(vm);
            return Observable.Return(Unit.Default);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [ReactiveCommand]
    public IObservable<Unit> GoToStartSnapshot()
    {
        ArgumentNullException.ThrowIfNull(Region1);

        try
        {
            var vm = new StartMessagingViewModel(Region1)
            {
                Snapshot = true
            };
            Region1?.Navigate(vm);
            return Observable.Return(Unit.Default);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [ReactiveCommand]
    public IObservable<Unit> GoToStopSnapshot()
    {
        ArgumentNullException.ThrowIfNull(Region1);

        try
        {
            var vm = new StopMessagingViewModel(Region1);
            Region1?.Navigate(vm);
            return Observable.Return(Unit.Default);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}