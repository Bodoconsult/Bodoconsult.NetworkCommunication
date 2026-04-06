//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

//using System.Reactive;
//using ReactiveUI;
//using ReactiveUI.SourceGenerators;

//namespace IpClient.ViewModels;

///// <summary>
///// Viewmodel 2
///// </summary>
//public partial class ViewModel2 : ReactiveObject
//{
//    public ViewModel2(IScreen hostScreen)
//    {
//        HostScreen = hostScreen;

//        //this.Back = HostScreen.Router.NavigateBack;
//    }

//    /// <summary>
//    /// Text
//    /// </summary>
//    [Reactive] private string _text = "View2";

//    public string UrlPathSegment => "View2";

//    public IScreen HostScreen { get; }

//    public ReactiveCommand<Unit, IRoutableViewModel> Back { get; }
//}