using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

        /// <summary>
        /// 把<param name="dt">DataTable</param>转化成List
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static List<TResult> ToList<TResult>(this DataTable dt) where TResult : class, new()
        {
            List<TResult> oblist = new List<TResult>();
            if (dt.Rows.Count == 0)
            { return new List<TResult>(); }
            //创建一个属性的列表  
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口  
            Type t = typeof(TResult);
            //获得TResult 的所的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表  
            Array.ForEach(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合  
            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例  
                TResult ob = new TResult();
                //找到对应的数据,并赋值  
                prlist.ForEach(p =>
                {
                    if (row[p.Name] != DBNull.Value && p.CanWrite)
                    {
                        p.SetValue(ob, row[p.Name].ToString().Trim(), null);
                    }
                    else
                    {
                        p.SetValue(ob, string.Empty, null);
                    }
                });
                //放入到返回的集合中.  
                oblist.Add(ob);
            }
            return oblist;
        }





    }
}
