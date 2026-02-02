// Decompiled with JetBrains decompiler
// Type: SEQUENCE
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib
{
    public class SEQUENCE : TOKEN
    {
        public override string yyname
        {
            get
            {
                return "SEQUENCE";
            }
        }

        public override int yynum
        {
            get
            {
                return 18;
            }
        }

        public SEQUENCE(Lexer yyl)
            : base(yyl)
        {
        }
    }
}
