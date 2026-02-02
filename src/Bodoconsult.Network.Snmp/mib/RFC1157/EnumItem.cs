// Decompiled with JetBrains decompiler
// Type: RFC1157.EnumItem
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib.RFC1157
{
  public class EnumItem : SYMBOL
  {
    public string name;
    public uint val;

    public EnumItem(Parser yyp, string n, uint v)
      : base(yyp)
    {
      name = n;
      val = v;
    }

    public override string yyname
    {
      get
      {
        return "EnumItem";
      }
    }

    public override int yynum
    {
      get
      {
        return 36;
      }
    }

    public EnumItem(Parser yyp)
      : base(yyp)
    {
    }
  }
}
