// Decompiled with JetBrains decompiler
// Type: RFC1157.Defs
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using System.Collections;
using System.Diagnostics;
using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib.RFC1157
{
  public class Defs : SYMBOL
  {
    public static Hashtable defs = new Hashtable();

    public static void Enter(Def d)
    {
      var def1 = (Def) defs[(object) d.name];
      if (def1 == null)
      {
        defs[(object) d.name] = (object) d;
        if (d.name == "iso")
          d.ix = 1U;
      }
      else
      {
        var flag = true;
        if (def1.parentname.Length > 0 && d.parentname.Length > 0 && def1.parentname != d.parentname)
          flag = false;
        if (def1.ix != 9999U && d.ix != 9999U && (int) def1.ix != (int) d.ix)
          flag = false;
        if (flag)
        {
          if (def1.parentname.Length == 0)
            def1.parentname = d.parentname;
          if (def1.ix == 9999U)
            def1.ix = d.ix;
          d = def1;
        }
        else
          defs[(object) d.name] = (object) d;
      }
      if (d.parentname.Length <= 0)
        return;
      var def2 = (Def) defs[(object) d.parentname];
      if (def2 == null)
      {
        Debug.Print("no parent " + def1.parentname);
      }
      else
      {
        def2.nodes[(object) d.ix] = (object) d;
        def2.names[(object) d.name] = (object) d;
        d.parent = def2;
      }
    }

    public override string yyname
    {
      get
      {
        return "Defs";
      }
    }

    public override int yynum
    {
      get
      {
        return 31;
      }
    }

    public Defs(Parser yyp)
      : base(yyp)
    {
    }
  }
}
