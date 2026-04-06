//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

//using ReactiveUI;
//using ReactiveUI.SourceGenerators;
//using System.Reactive;

//namespace IpClient.ViewModels;

///// <summary>
///// Viewmodel 1
///// </summary>
//public partial class ViewModel1: ReactiveObject, IRoutableViewModel
//{
//    public ViewModel1(IScreen hostScreen)
//    {
//        HostScreen = hostScreen;

//        //this.Back = HostScreen.Router.NavigateBack;
//    }

//    /// <summary>
//    /// Text
//    /// </summary>
//    [Reactive] private string _text = "View1";

//    /// <summary>
//    /// Gets a string token representing the current ViewModel, such as 'login' or 'user'.
//    /// </summary>
//    public string UrlPathSegment => "View1";

//    /// <summary>
//    /// Gets the IScreen that this ViewModel is currently being shown in. This
//    /// is usually passed into the ViewModel in the Constructor and saved
//    /// as a ReadOnly Property.
//    /// </summary>
//    public IScreen HostScreen { get; }

//    public ReactiveCommand<Unit, IRoutableViewModel> Back { get; }
//}