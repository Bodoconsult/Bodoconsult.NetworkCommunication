// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.ReactiveUI.Extensions;
using Bodoconsult.App.ReactiveUI.Interfaces;
using Bodoconsult.App.ReactiveUI.Regions;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace IpClientUi.ViewModels;

public partial class Window1ViewModel: ReactiveObject, IUiWindowViewModel
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="regionManager">Current region manager instance</param>
    public Window1ViewModel(IRegionManager regionManager)
    {
        RegionManager = regionManager;
    }

    /// <summary>
    /// Instance name of the window. If null or string.Empty the window instance name is derived from the window type name (loading the window as a singleton instance)
    /// </summary>
    public string? InstanceName { get; set; } = null;

    /// <summary>
    /// Current region manager
    /// </summary>
    public IRegionManager RegionManager { get; }

    /// <summary>
    /// Region 1
    /// </summary>
    [Reactive]
    public partial UiRegion? Region1 { get; set; }

    /// <summary>
    /// Region 2
    /// </summary>
    [Reactive]
    public partial UiRegion? Region2 { get; set; }

    /// <summary>
    /// Region 3
    /// </summary>
    [Reactive]
    public partial UiRegion? Region3 { get; set; }

    /// <summary>
    /// Is the window registered already
    /// </summary>
    [Reactive]
    public partial bool IsRegistered { get; set; }

    [ReactiveCommand]
    public void GoToSecondView()
    {
        try
        {
            Region1?.Navigate(new SecondViewModel(Region1));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    //[ReactiveCommand]
    //public void GoToMainWindow()
    //{
    //    try
    //    {
    //        var vm = new SecondViewModel();

    //        RegionManager.Navigate(windowViewModel, vm, "DocumentRegion");
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine(e);
    //        throw;
    //    }
    //}

}