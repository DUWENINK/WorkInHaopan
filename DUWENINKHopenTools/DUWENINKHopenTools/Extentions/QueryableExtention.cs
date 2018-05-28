using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DUWENINKHopenTools.Extentions
{
    /// <summary>
    /// Queryable扩展
    /// </summary>
    public static class ListExtention
    {
        /// <summary>
        /// WhereIf[在condition为true的情况下应用Where表达式]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static List<T> WhereIf<T>(this List<T> source, bool condition, Func<T, bool> function)
        {
            return condition ? source.Where(function).ToList() : source;
        }


        public static void Trim<T>(this List<T> list)
        {
            list.ForEach(x =>
            {

            });
        }
    }
}
