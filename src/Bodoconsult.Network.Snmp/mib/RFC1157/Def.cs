// Decompiled with JetBrains decompiler
// Type: RFC1157.Def
// Assembly: mib, Version=1.0.2230.16788, Culture=neutral, PublicKeyToken=null
// MVID: 760E598E-8FFE-4F56-9F79-D789AB773CDA
// Assembly location: D:\Daten\Projekte\Dependencies\Mib\mib.dll

using System.Collections;
using Bodoconsult.Snmp.Tools.Tools;

namespace Bodoconsult.Snmp.mib.RFC1157
{
  public class Def : SYMBOL
  {
    public Def parent = (Def) null;
    public uint ix = 0;
    public string parentname = "";
    public string help = "";
    public Hashtable names = new Hashtable();
    public Hashtable nodes = new Hashtable();
    public string name;

    public Def this[string s]
    {
      get
      {
        return (Def) names[(object) s];
      }
    }

    public Def this[uint k]
    {
      get
      {
        return (Def) nodes[(object) k];
      }
    }

    public uint[] path
    {
      get
      {
        var arrayList = new ArrayList();
        for (var def = this; def != null; def = def.parent)
          arrayList.Add((object) def.ix);
        var numArray = new uint[arrayList.Count];
        for (var index = 0; index < arrayList.Count; ++index)
          numArray[index] = (uint) arrayList[arrayList.Count - index - 1];
        return numArray;
      }
    }

    public string Lookup(uint[] u, int p)
    {
      var str = name;
      if (p >= u.Length)
        return str;
      var def = this[u[p]];
      if (def != null)
        return str + "." + def.Lookup(u, p + 1);
      while (p < u.Length)
        str = str + "." + (object) u[p++];
      return str;
    }

    public string[] Kids()
    {
      var strArray = new string[names.Count];
      var num = 0;
      var enumerator = names.GetEnumerator();
      while (enumerator.MoveNext())
        strArray[num++] = (string) enumerator.Key;
      return strArray;
    }

    public Def(Parser yyp, string i, ObjectType t)
      : base(yyp)
    {
      name = i;
      if (t == null)
        return;
      help = t.help;
    }

    public Def(Parser yyp, string i, ObjectType t, Oid e)
      : this(yyp, i, t)
    {
      var str = "";
      uint num = 0;
      for (; e != null && e.head != null; e = e.tail)
      {
        var name = e.head.name;
        num = e.head.val;
        if (name.Length > 0)
        {
          if ((Def) Defs.defs[(object) name] == null)
            Defs.Enter(new Def(yyp, name, (ObjectType) null)
            {
              parentname = str,
              ix = num
            });
          str = name;
        }
      }
      parentname = str;
      ix = num;
      Defs.Enter(this);
    }

    public override string yyname
    {
      get
      {
        return "Def";
      }
    }

    public override int yynum
    {
      get
      {
        return 32;
      }
    }

    public Def(Parser yyp)
      : base(yyp)
    {
    }
  }
}
