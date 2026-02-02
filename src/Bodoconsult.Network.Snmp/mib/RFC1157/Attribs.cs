// Decompiled with JetBrains decompiler
// Type: RFC1157.Attribs
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib.RFC1157
{
  public class Attribs : SYMBOL
  {
    public string help = "";
    public Attribs tail;
    public Attrib head;

    public Attribs(Parser yyp, Attrib h, Attribs t)
      : base(yyp)
    {
      head = h;
      tail = t;
      if (h.val != null)
      {
        help = h.val;
      }
      else
      {
        if (tail == null)
          return;
        help = tail.help;
      }
    }

    public override string yyname
    {
      get
      {
        return "Attribs";
      }
    }

    public override int yynum
    {
      get
      {
        return 34;
      }
    }

    public Attribs(Parser yyp)
      : base(yyp)
    {
    }
  }
}
