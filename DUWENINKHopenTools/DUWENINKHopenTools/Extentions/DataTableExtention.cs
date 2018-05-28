using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUWENINKHopenTools.Extentions
{
    /// <summary>
    /// 扩展DataTable功能
    /// </summary>
  public static class DataTableExtention
    {
        /// <summary>
        /// 获取<param name="table">DataTable</param> 的列的名称集合
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <returns></returns>
        public static IEnumerable<string> GetColumnsStringList(this DataTable table)
        {
           var result=new List<string>();
            foreach (DataColumn columns in table.Columns)
            {
                result.Add(columns.ColumnName);
            }
            return result;
        }

    }
}
