// Decompiled with JetBrains decompiler
// Type: RFC1157.Mgmt
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using System;
using System.IO;
using System.Reflection;
using System.Text;
using Bodoconsult.Snmp.Tools.Tools;
using Path = System.IO.Path;

namespace Bodoconsult.Snmp.mib.RFC1157
{
    public class Mgmt
    {
        private Parser p = (Parser)new syntax();
        public Def def;

        public string Lookup(uint[] u)
        {
            for (var index = 0; index < def.path.Length; ++index)
            {
                if ((int)u[index] != (int)def.path[index])
                    throw new Exception("Bad OID");
            }
            return def[u[def.path.Length]].Lookup(u, def.path.Length + 1);
        }

        public string getFullNameFromOID(string OID)
        {
            var strArray = OID.Split('.');
            var u = new uint[strArray.Length];
            for (var index = 0; index < strArray.Length; ++index)
                u[index] = Convert.ToUInt32(strArray[index]);
            return Lookup(u);
        }

        public uint[] OID(string s)
        {
            var strArray = s.Split('.');
            var def = this.def;
            int index1;
            for (index1 = 0; index1 < strArray.Length && !char.IsDigit(strArray[index1][0]); ++index1)
            {
                def = def[strArray[index1]];
                if (def == null)
                    throw new Exception("unrecognised " + strArray[index1]);
            }
            var index2 = 0;
            uint[] numArray;
            if (index1 == 0)
            {
                numArray = new uint[strArray.Length];
            }
            else
            {
                if (index1 >= strArray.Length)
                    return def.path;
                numArray = new uint[def.path.Length + strArray.Length - index1];
                for (index2 = 0; index2 < def.path.Length; ++index2)
                    numArray[index2] = def.path[index2];
            }
            while (index1 < strArray.Length)
                numArray[index2++] = uint.Parse(strArray[index1++]);
            return numArray;
        }

        public string OID(uint[] u)
        {
            return Lookup(u);
        }

        public string giveOID(string s)
        {
            var str = "";
            foreach (var num in OID(s))
                str = str + num.ToString() + ".";
            return str.Substring(0, str.Length - 1);
        }

        public Def getDef(string name)
        {
            var def1 = def;
            var str = name;
            var chArray = new char[1] { '.' };
            foreach (var index in str.Split(chArray))
            {
                var def2 = def1[index];
                if (def2 != null)
                    def1 = def2;
                else
                    break;
            }
            return def1;
        }

        public string getDescription(string OID)
        {
            return getDef(getFullNameFromOID(OID)).help;
        }

        public Mgmt()
        {
            var str = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "").Replace("\\", "/") + "/SNMPv2-SMI.mib";
            var buf = "\r\nSNMPv2-SMI DEFINITIONS ::= BEGIN\r\n\tccitt OBJECT IDENTIFIER ::= { 0 }\r\n\tzeroDotZero OBJECT-IDENTITY\r\n\t\tSTATUS current\r\n\t\tDESCRIPTION\r\n\t\t\t\"A value used for null identifiers.\"\r\n\t::= { ccitt 0 }\r\n\tiso OBJECT IDENTIFIER ::= { 1 }\r\n\torg OBJECT IDENTIFIER ::= { iso 3 }\r\n\tdod OBJECT IDENTIFIER ::= { org 6 }\r\n\tinternet OBJECT IDENTIFIER ::= { dod 1 }\r\n\tdirectory OBJECT IDENTIFIER ::= { internet 1 }\r\n\tmgmt OBJECT IDENTIFIER ::= { internet 2 }\r\n\tmib-2 OBJECT IDENTIFIER ::= { mgmt 1 }\r\n\ttransmission OBJECT IDENTIFIER ::= { mib-2 10 }\r\n\texperimental OBJECT IDENTIFIER ::= { internet 3 }\r\n\tprivate OBJECT IDENTIFIER ::= { internet 4 }\r\n\tenterprises OBJECT IDENTIFIER ::= { private 1 }\r\n\tsecurity OBJECT IDENTIFIER ::= { internet 5 }\r\n\tsnmpV2 OBJECT IDENTIFIER ::= { internet 6 }\r\n\tsnmpDomains OBJECT IDENTIFIER ::= { snmpV2 1 }\r\n\tsnmpProxys OBJECT IDENTIFIER ::= { snmpV2 2 }\r\n\tsnmpModules OBJECT IDENTIFIER ::= { snmpV2 3 }\r\nEND\r\n";
            try
            {
                p.Parse(buf);
            }
            catch //(Exception ex)
            {
                // ignored
            }

            def = (Def)Defs.defs[(object)"internet"];
        }

        public void loadFile(string fileName)
        {
            var input = new StreamReader(fileName, (Encoding)new ASCIIEncoding(), true, 1024);
            try
            {
                p.Parse(input);
            }
            catch //(Exception ex)
            {
                // ignored
            }

            def = (Def)Defs.defs[(object)"internet"];
        }
    }
}
