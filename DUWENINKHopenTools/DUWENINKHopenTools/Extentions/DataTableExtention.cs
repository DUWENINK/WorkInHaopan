using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DUWENINKHopenTools.Entity;

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
            var result = new List<string>();
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
                    switch (p.PropertyType.FullName)
                    {
                        case "System.DateTime":
                            {
                                DateTime time = XD.Base.ConvertOp.Convert2DateTime(row[p.Name]);
                                p.SetValue(ob, time, null);
                                break;
                            }

                        case "System.Decimal":
                            {
                                decimal nums = XD.Base.ConvertOp.Convert2Decimal(row[p.Name]);
                                p.SetValue(ob, nums, null);
                                break;
                            }
                        case "System.String":
                        {
                            var str = XD.Base.ConvertOp.Convert2String(row[p.Name]).Trim();
                            p.SetValue(ob, str, null);
                                break;
                        }
                        case "System.Boolean": //布尔型     
                        {
                            var boolVar = XD.Base.ConvertOp.Convert2Boolean(row[p.Name]);
                            p.SetValue(ob, boolVar, null);
                            break;
                        }
                        case "System.Int16"://整型     
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                        {
                            var boolVar = XD.Base.ConvertOp.Convert2Boolean(row[p.Name]);
                            p.SetValue(ob, boolVar, null);
                            break;

                        }




                        default:
                            p.SetValue(ob, row[p.Name], null);
                            break;
                    }





                });
                //放入到返回的集合中.  
                oblist.Add(ob);
            }
            return oblist;
        }


        ///  <summary>
        ///  将DataTable 转换成 List/<dynamic/>
        ///  reverse 反转：控制返回结果中是只存在 FilterField 指定的字段,还是排除.
        ///  [flase 返回FilterField 指定的字段]|[true 返回结果剔除 FilterField 指定的字段]
        ///  FilterField  字段过滤，FilterField 为空 忽略 reverse 参数；返回DataTable中的全部数
        ///  </summary>
        ///  <param name="table">DataTable</param>
        /// <param name="reverse">
        ///  反转：控制返回结果中是只存在 FilterField 指定的字段,还是排除.
        ///  [flase 返回FilterField 指定的字段]|[true 返回结果剔除 FilterField 指定的字段]
        /// </param>
        ///  <param name="filterField">字段过滤，FilterField 为空 忽略 reverse 参数；返回DataTable中的全部数据</param>
        ///  <returns>List/<dynamic/></returns>
        public static List<dynamic> ToDynamicList(this DataTable table, bool reverse = true, params string[] filterField)
        {
            var modelList = new List<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                dynamic model = new ExpandoObject();
                var dict = (IDictionary<string, object>)model;
                foreach (DataColumn column in table.Columns)
                {
                    if (filterField.Length != 0)
                    {
                        if (reverse)
                        {
                            if (!filterField.Contains(column.ColumnName))
                            {
                                dict[column.ColumnName] = row[column];
                            }
                        }
                        else
                        {
                            if (filterField.Contains(column.ColumnName))
                            {
                                dict[column.ColumnName] = row[column];
                            }
                        }
                    }
                    else
                    {
                        dict[column.ColumnName] = row[column];
                    }
                }
                modelList.Add(model);
            }
            return modelList;
        }




    }
}
