using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUWENINKHopenTools.Entity
{
    /// <summary>
    /// 单元格属性
    /// </summary>
    public class Ceils
    {
        /// <summary>
        /// 列名称
        /// </summary>
        public string CeilName { get; set; }
        /// <summary>
        /// 列类型
        /// </summary>
        public DbType CeilType { get; set; }
        /// <summary>
        /// 列值
        /// </summary>
        public object CeilValue { get; set; }
    }


    public class DbMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
   

    


}
