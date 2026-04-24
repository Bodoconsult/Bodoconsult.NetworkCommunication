// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Reactive;
using IpClientUi.Interfaces;
using ReactiveUI;

namespace IpClientUi.ViewModels;

/// <summary>
/// Fake implemenation for designtime datacontext
/// </summary>
public class FakeStopMessagingViewModel : IStopMessagingViewModel
{
    /// <summary>
    /// Gets a string token representing the current ViewModel, such as 'login' or 'user'.
    /// </summary>
    public string UrlPathSegment { get; set; } = null!;

    /// <summary>
    /// Gets the IScreen that this ViewModel is currently being shown in. This
    /// is usually passed into the ViewModel in the Constructor and saved
    /// as a ReadOnly Property.
    /// </summary>
    public IScreen HostScreen { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> StopMessagingCommand { get; set; } = null!;
    public ReactiveCommand<Unit, Unit> BackCommand { get; set; } = null!;
    public void InjectScreen(IScreen screen)
    {
        throw new NotImplementedException();
    }
}