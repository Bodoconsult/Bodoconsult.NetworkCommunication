// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.ReactiveUI.Interfaces;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace IpClientUi.ViewModels;

public partial class SecondViewModel : ReactiveObject, IUiRegionViewModel
{
    /// <summary>
    /// Gets a string token representing the current ViewModel, such as 'login' or 'user'.
    /// </summary>
    public string UrlPathSegment => "second";

    /// <summary>
    /// Test text
    /// </summary>
    [Reactive] public partial string Test { get; set; }

    /// <summary>
    /// Gets the IScreen that this ViewModel is currently being shown in. This
    /// is usually passed into the ViewModel in the Constructor and saved
    /// as a ReadOnly Property.
    /// </summary>
    public IScreen HostScreen { get; private set; } = new DummyScreen();

    public SecondViewModel()
    {
        _test = "Mummmpf";
    }

    public SecondViewModel(IScreen screen)
    {
        HostScreen = screen;
        _test = "Mummmpf";
    }

    /// <summary>
    /// Method based late injection of <see cref="ReactiveUI.IScreen"/> instance for navigation
    /// </summary>
    /// <param name="screen"></param>
    public void InjectScreen(IScreen screen)
    {
        HostScreen = screen;
    }
}