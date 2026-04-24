// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Reactive;
using IpClientUi.Interfaces;
using ReactiveUI;

namespace IpClientUi.ViewModels;

/// <summary>
/// Fake implemenation for designtime datacontext
/// </summary>
public class FakeStartMessagingViewModel : IStartMessagingViewModel
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

    /// <summary>
    /// Is the request for snapshot (false) or streaming (true)
    /// </summary>
    public bool Snapshot { get; set; }

    /// <summary>
    /// Show channel 1?
    /// </summary>
    public bool Channel1 { get; set; }

    /// <summary>
    /// Show channel 2?
    /// </summary>
    public bool Channel2 { get; set; }

    /// <summary>
    /// Show channel 3?
    /// </summary>
    public bool Channel3 { get; set; }

    /// <summary>
    /// Show channel 4?
    /// </summary>
    public bool Channel4 { get; set; }

    /// <summary>
    /// Start messaging command
    /// </summary>
    public ReactiveCommand<Unit, Unit> StartMessagingCommand { get; set; } = null!;

    /// <summary>
    /// Back command
    /// </summary>
    public ReactiveCommand<Unit, Unit> BackCommand { get; set; } = null!;

    /// <summary>
    /// Method based late injection of <see cref="ReactiveUI.IScreen"/> instance for navigation
    /// </summary>
    /// <param name="screen"></param>
    public void InjectScreen(IScreen screen)
    {
        throw new NotImplementedException();
    }
}