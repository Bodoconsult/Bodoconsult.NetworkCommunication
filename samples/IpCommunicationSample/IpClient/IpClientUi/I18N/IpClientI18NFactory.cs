// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Avalonia.ReactiveUI.I18N;
using Bodoconsult.I18N.DependencyInjection;

namespace IpClientUi.I18N;

/// <summary>
/// Factory to create a fully configured I18N factory using providers directly
/// </summary>
public class IpClientI18NFactory : BaseI18NFactory
{
    /// <summary>
    /// Creating a configured II18N instance
    /// </summary>
    /// <returns>An II18N instance</returns>
    public override II18N CreateInstance()
    {
        var ass = typeof(IpClientI18NFactory).Assembly;

        // Set the fallback language
        I18NInstance.SetFallbackLocale("en");

        // Load Avalonia base package
        var package = new AvaloniaLocalesProviderPackage();
        package.LoadLocalesProviders(I18NInstance);

        // Load more providers or packages if necessary
        // ...

        // Init instance with language from running thread
        I18NInstance.Init();

        // Return the instance
        return I18NInstance;
    }
}