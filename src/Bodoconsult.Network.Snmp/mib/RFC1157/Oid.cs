// Decompiled with JetBrains decompiler
// Type: RFC1157.Oid
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib.RFC1157
{
  public class Oid : SYMBOL
  {
    public EnumItem head = (EnumItem) null;
    public Oid tail;

    public Oid(Parser yyp, EnumItem h, Oid t)
      : base(yyp)
    {
      head = h;
      tail = t;
    }

    public override string yyname
    {
      get
      {
        return "Oid";
      }
    }

    public override int yynum
    {
      get
      {
        return 35;
      }
    }

    public Oid(Parser yyp)
      : base(yyp)
    {
    }
  }
}
