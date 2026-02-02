// Decompiled with JetBrains decompiler
// Type: ID
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib
{
    public class ID : TOKEN
    {
        public string id;

        public override string yyname
        {
            get
            {
                return "ID";
            }
        }

        public override int yynum
        {
            get
            {
                return 3;
            }
        }

        public ID(Lexer yyl)
            : base(yyl)
        {
        }
    }
}
