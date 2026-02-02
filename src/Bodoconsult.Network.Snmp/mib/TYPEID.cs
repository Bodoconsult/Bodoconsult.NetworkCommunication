// Decompiled with JetBrains decompiler
// Type: TYPEID
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib
{
    public class TYPEID : TOKEN
    {
        public override string yyname
        {
            get
            {
                return "TYPEID";
            }
        }

        public override int yynum
        {
            get
            {
                return 22;
            }
        }

        public TYPEID(Lexer yyl)
            : base(yyl)
        {
        }
    }
}
