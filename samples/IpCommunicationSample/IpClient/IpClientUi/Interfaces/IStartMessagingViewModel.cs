// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using ReactiveUI;

namespace IpClientUi.Interfaces;

/// <summary>
/// Interface for start messaging view model
/// </summary>
public interface IStartMessagingViewModel
{
    /// <summary>
    /// Gets a string token representing the current ViewModel, such as 'login' or 'user'.
    /// </summary>
    string UrlPathSegment { get; }

    /// <summary>
    /// Gets the IScreen that this ViewModel is currently being shown in. This
    /// is usually passed into the ViewModel in the Constructor and saved
    /// as a ReadOnly Property.
    /// </summary>
    IScreen HostScreen { get; }

    /// <summary>
    /// Is the request for snapshot (false) or streaming (true)
    /// </summary>
    bool Snapshot { get; set; }

    /// <summary>
    /// Show channel 1?
    /// </summary>
    bool Channel1 { get; set; }

    /// <summary>
    /// Show channel 2?
    /// </summary>
    bool Channel2 { get; set; }
    
    /// <summary>
    /// Show channel 3?
    /// </summary>
    bool Channel3 { get; set; }

    /// <summary>
    /// Show channel 4?
    /// </summary>
    bool Channel4 { get; set; }

    /// <summary>
    /// Start messaging command
    /// </summary>
    ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> StartMessagingCommand { get; }

    /// <summary>
    /// Back command
    /// </summary>
    ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> BackCommand { get; }

    /// <summary>
    /// Method based late injection of <see cref="IScreen"/> instance for navigation
    /// </summary>
    /// <param name="screen"></param>
    void InjectScreen(IScreen screen);
}