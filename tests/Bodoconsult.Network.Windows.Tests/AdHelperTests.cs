// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Diagnostics;
using System.Runtime.Versioning;
using Bodoconsult.Network.Windows.ActiveDirectory.Helpers;

namespace Bodoconsult.Network.Windows.Tests;

[TestFixture]
[SupportedOSPlatform("windows")]
public class AdHelperTests
{
    [Test]
    public void TestGetLdapDomainForCurrentUser()
    {
        var result = AdHelper.GetLdapDomainForCurrentUser();

        Debug.Print(result);

        Assert.That(string.IsNullOrEmpty(result), Is.False);
    }

}