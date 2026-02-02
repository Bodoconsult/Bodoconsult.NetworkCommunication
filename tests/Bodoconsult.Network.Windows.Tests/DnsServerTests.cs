using System.Diagnostics;
using Bodoconsult.Network.Windows.Dns;
using Bodoconsult.Network.Windows.Tests.Helpers;

namespace Bodoconsult.Network.Windows.Tests;

[TestFixture]
public class DnsServerTests
{
    [Test]
    public void TestGetListOfDomains()
    {
        var settings = TestHelper.GetAppSettings();

        var pwd = TestHelper.GetSecureString(settings.Password);

        var d = new DnsServer(settings.DomainServer, settings.Domain, settings.UserName, pwd);

        Debug.Print($"DNS structure {settings.DomainServer}");

        Debug.Print("DNS domains");
        foreach (var domain in d.GetListOfDomains())
        {
            Debug.Print($"\t{domain.Name} ({domain.ZoneType}{(domain.ReverseZone ? ", Reverse zone" : "")})");
            //and a list of all the records in the domain:-
            foreach (var record in d.GetRecordsForDomain(domain.Name))
            {
                Debug.Print($"\t\t{record}");
                //any domains we are primary for we could go and edit the record now!
            }
        }
    }
}