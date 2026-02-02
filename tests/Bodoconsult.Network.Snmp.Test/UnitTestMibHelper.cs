using System.Diagnostics;
using Bodoconsult.Snmp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodoconsult.Snmp.Test
{
    [TestClass]
    public class UnitTestMibHelper
    {

        [TestMethod]
        public void TestMib()
        {
            var myMib = new MibHelper();
            myMib.LoadMib(System.IO.Path.Combine(TestHelper.MibPath, @"SYNOLOGY-SYSTEM-MIB.mib"));

            //1.3.6.1.4.1.6574.1.5.3.0"
            //1.3.6.1.4.1.6574.1.1

            var oid = "1.3.6.1.4.1.6574.1.1";

            var erg = myMib.GetFullName(oid);

            var name = myMib.GetSimpleName(oid);

            var desc = myMib.GetDescription(oid);

            var other = myMib.GetDescription(oid);

            Debug.Print("other: {0}", other);
            Debug.Print("erg: {0}", erg);
            Debug.Print("name: {0}", name);
            Debug.Print("dec: {0}", desc);

            Assert.IsFalse(string.IsNullOrEmpty(erg));
        }

        [TestMethod]
        public void TestMib2()
        {
            var myMib = new MibHelper();
            myMib.LoadMib(System.IO.Path.Combine(TestHelper.MibPath, @"SYNOLOGY-SYSTEM-MIB.mib"));

            var erg = myMib.GetFullName("1.3.6.1.4.1.11");

            var name = myMib.GetSimpleName("1.3.6.1.4.1.11");

            var desc = myMib.GetDescription("1.3.6.1.4.1.11");

            var other = myMib.GetDescription("1.3.6.1.4.1.11");

            Debug.Print("other: {0}", other);
            Debug.Print("erg: {0}", erg);
            Debug.Print("name: {0}", name);
            Debug.Print("dec: {0}", desc);

            Assert.IsFalse(string.IsNullOrEmpty(erg));
        }


        [TestMethod]
        public void TestMib3()
        {
            var myMib = new MibHelper();
            //myMib.LoadMib(System.IO.Path.Combine(TestHelper.MibPath,@"SYNOLOGY-SYSTEM-MIB.mib"));
            myMib.LoadMib(System.IO.Path.Combine(TestHelper.MibPath, @"Rfc3805.mib"));

            var oid = "1.3.6.1.4.1.11";

            var erg = myMib.GetFullName(oid);

            var name = myMib.GetSimpleName(oid);

            var desc = myMib.GetDescription(oid);

            var other = myMib.GetDescription(oid);

            Debug.Print("other: {0}", other);
            Debug.Print("erg: {0}", erg);
            Debug.Print("name: {0}", name);
            Debug.Print("dec: {0}", desc);

            Assert.IsTrue(string.IsNullOrEmpty(erg));
        }


        
    }
}
