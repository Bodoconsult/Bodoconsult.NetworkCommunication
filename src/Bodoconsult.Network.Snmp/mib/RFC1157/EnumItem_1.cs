// Decompiled with JetBrains decompiler
// Type: RFC1157.EnumItem_1
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib.RFC1157
{
  public class EnumItem_1 : EnumItem
  {
    public EnumItem_1(Parser yyq)
      : base(yyq, ((ID) yyq.StackAt(3).m_value).id, ((UNSIGNED) yyq.StackAt(1).m_value).val)
    {
    }
  }
}
