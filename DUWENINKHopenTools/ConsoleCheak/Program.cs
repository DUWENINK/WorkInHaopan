using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCheak
{
    class Program
    {
        static void Main(string[] args)
        {


            var table = new DataTable();
            table.Columns.Add("123");
            var table2 = new DataTable();
            table2.Columns.Add("123");
            var table3 = new DataTable();
            table3.Columns.Add("123");
            var table4 = new DataTable();
            table4.Columns.Add("123");
            var table5 = new DataTable();
            table5.Columns.Add("123");
            a(table);//进入了方法之后, 123
            aa(table2);//进入了方法之后 123 456
            b(ref table3);//空的
            bb(ref table4);//123 456
            table5 = new DataTable();

        }
        public static void a(DataTable mm)
        {
            mm= new DataTable();
        }
        public static void aa(DataTable mm)
        {
            mm.Columns.Add("456");
        }
        public static void b(ref DataTable mm)
        {
            mm = new DataTable();
        }
        public static void bb(ref DataTable mm)
        {
            mm.Columns.Add("456");
        }
    }
}
