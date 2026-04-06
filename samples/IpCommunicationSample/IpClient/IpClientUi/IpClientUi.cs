// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Avalonia.ReactiveUI.App;
using Bodoconsult.App.ReactiveUI.Interfaces;
using IpClientUi.DiContainerProvider;
using IpClientUi.ViewModels;
using IpClientUi.Views;
using ReactiveUI;

namespace IpClientUi;

public class IpClientAppBuilder : BaseAvaloniaReactiveUiAppBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Global app settings</param>
    public IpClientAppBuilder(IAppGlobals appGlobals) : base(appGlobals)
    { }

    /// <summary>
    /// Load the <see cref="IAppBuilder.DiContainerServiceProviderPackage"/>
    /// </summary>
    public override void LoadDiContainerServiceProviderPackage()
    {
        var factory = new IpClientProductionDiContainerServiceProviderPackageFactory(AppGlobals);
        DiContainerServiceProviderPackage = factory.CreateInstance();
    }

    /// <summary>
    /// Load view location
    /// </summary>
    /// <param name="locator">The locator to use for the app instance</param>
    public override void LoadViewLocation(DefaultViewLocator locator)
    {
        locator.Map<FirstViewModel, FirstView>(() => new FirstView());
        locator.Map<SecondViewModel, SecondView>(() => new SecondView());
    }

    /// <summary>
    /// Create the view model for the main window
    /// </summary>
    public override IRxMainWindowViewModel CreateViewModel()
    {
        var viewModel = AppGlobals.DiContainer.Get<IpClientMainWindowViewModel>();
        viewModel.HeaderBackColor = TypoColors.DarkBlue;
        viewModel.BodyBackColor = TypoColors.Beige;
        viewModel.AppExe = AppGlobals.AppStartParameter.AppExe;

        // Load the logo now
        viewModel.LoadLogo(AppGlobals.AppStartParameter.LogoAssembly, AppGlobals.AppStartParameter.LogoRessourcePath);

        return viewModel;
    }
}