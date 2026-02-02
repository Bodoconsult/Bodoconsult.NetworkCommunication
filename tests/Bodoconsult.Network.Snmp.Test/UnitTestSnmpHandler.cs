using System.Diagnostics;
using System.Linq;
using Bodoconsult.Snmp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodoconsult.Snmp.Test
{
    [TestClass]
    public class UnitTestSnmpHandler
    {

        [TestMethod]
        public void TestGetData()
        {
            var sh = new SnmpHandler(TestHelper.MibPath);

            var request = new SnmpBaseRequest
            {
                Username = "snmpaccess",
                Password = "ks73cGDPlETuE+suN/8e/w==",
            };

            request.IpAddresses.Add("192.168.10.109");

            request.Oids.Add("1.3.6.1.4.1.6574.1.1.0");
            request.Oids.Add("1.3.6.1.4.1.6574.1.2.0");
            request.Oids.Add("1.3.6.1.4.1.6574.1.3.0");

            request.Oids.Add("1.3.6.1.4.1.6574.1.5.3.0");

            


            sh.GetBulk(request);
           
            Assert.IsTrue(request.Results.Count>0);

            foreach (var data in request.Results.OrderBy(x => x.Oid))
            {
                Debug.Print("{0} | {1} | {2} | {3}", data.Oid, data.Value, data.FullName, data.Description);
            }
        }
    }
}