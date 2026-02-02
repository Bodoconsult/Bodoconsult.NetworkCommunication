using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Bodoconsult.Inventory.Util;

public static class DataTableExtensions
{
    public static List<DataRow> ToGenericList(this DataTable datatable)
    {
        return (from row in datatable.AsEnumerable()
            select (row)).ToList();
    }
}