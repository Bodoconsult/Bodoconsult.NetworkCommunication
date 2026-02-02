// Decompiled with JetBrains decompiler
// Type: tokens
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib
{
    public class tokens : Lexer
    {
        public tokens()
            : base((YyLexer) new yytokens(new ErrorHandler(false)))
        {
        }

        public tokens(ErrorHandler eh)
            : base((YyLexer) new yytokens(eh))
        {
        }

        public tokens(YyLexer tks)
            : base(tks)
        {
        }
    }
}
