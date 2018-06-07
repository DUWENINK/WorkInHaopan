using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DUWENINKHopenTools.Extentions
{
   public static class StringExtention
    {
        /// <summary>
        /// 是否是标准身份证号码
        /// </summary>
        /// <param name="stringInput">疑似身份证字符串</param>
        /// <returns>ture:是,false:不是</returns>
        public static bool IsIdCard(this string stringInput)
        {
            var reg = new Regex(@"(^[1-9]\d{5}(18|19|([23]\d))\d{2}((0[1-9])|(10|11|12))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]$)|(^[1-9]\d{5}\d{2}((0[1-9])|(10|11|12))(([0-2][1-9])|10|20|30|31)\d{2}[0-9Xx]$)");//身份证号码的正则
            return reg.IsMatch(stringInput);

        }

        
    }
}
