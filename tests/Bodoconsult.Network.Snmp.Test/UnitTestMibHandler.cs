using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodoconsult.Snmp.Test
{
    [TestClass]
    public class UnitTestMibHandler
    {

        [TestMethod]
        public void TestMib()
        {
            MibHandler.MibDirectory = TestHelper.MibPath;
            MibHandler.LoadAllMibFiles();

            //1.3.6.1.4.1.6574.1.5.3.0"
            //1.3.6.1.4.1.6574.1.1


            // First OID
            var oid = "1.3.6.1.4.1.6574.1.1";

            Debug.Print(oid);

            var erg = MibHandler.GetFullName(oid);

            var name = MibHandler.GetSimpleName(oid);

            var desc = MibHandler.GetDescription(oid);

            Debug.Print("erg: {0}", erg);
            Debug.Print("name: {0}", name);
            Debug.Print("desc: {0}", desc);

            Assert.IsFalse(string.IsNullOrEmpty(erg));

            // 2nd OID
            oid = "1.3.6.1.4.1.6574.1.5.3.0";
            Debug.Print(oid);

            erg = MibHandler.GetFullName(oid);

            name = MibHandler.GetSimpleName(oid);

            desc = MibHandler.GetDescription(oid);


            Debug.Print("erg: {0}", erg);
            Debug.Print("name: {0}", name);
            Debug.Print("desc: {0}", desc);

            Assert.IsFalse(string.IsNullOrEmpty(erg));


            // 3rd OID
            oid = "1.3.6.1.4.1.6574.2.1.1.3.3";
            Debug.Print(oid);

            erg = MibHandler.GetFullName(oid);

            name = MibHandler.GetSimpleName(oid);

            desc = MibHandler.GetDescription(oid);

            Debug.Print("desc: {0}", desc);
            Debug.Print("erg: {0}", erg);
            Debug.Print("name: {0}", name);

            Assert.IsFalse(string.IsNullOrEmpty(erg));
        }
    }
}