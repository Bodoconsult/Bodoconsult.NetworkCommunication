// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using IpClientUi.ViewModels;
using ReactiveUI.Avalonia;

namespace IpClientUi.Views;

/// <summary>
/// Interaktionslogik für SecondView.xaml
/// </summary>
public partial class SecondView :  ReactiveUserControl<SecondViewModel>
{
    public SecondView()
    {
        InitializeComponent();
    }
}