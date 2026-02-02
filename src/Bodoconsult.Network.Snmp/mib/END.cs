// Decompiled with JetBrains decompiler
// Type: END
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib
{
    public class END : TOKEN
    {
        public override string yyname
        {
            get
            {
                return "END";
            }
        }

        public override int yynum
        {
            get
            {
                return 8;
            }
        }

        public END(Lexer yyl)
            : base(yyl)
        {
        }
    }
}
