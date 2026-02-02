// Decompiled with JetBrains decompiler
// Type: RFC1157.Def_1
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib.RFC1157
{
  public class Def_1 : Def
  {
    public Def_1(Parser yyq)
      : base(yyq, ((ID) yyq.StackAt(5).m_value).id, (ObjectType) yyq.StackAt(4).m_value, (Oid) yyq.StackAt(1).m_value)
    {
    }
  }
}
