// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace IpClientUi.Interfaces;

/// <summary>
/// Interface for start messaging view model
/// </summary>
public interface IStopMessagingViewModel
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
    /// Stop messaging command
    /// </summary>
    ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> StopMessagingCommand { get; }

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