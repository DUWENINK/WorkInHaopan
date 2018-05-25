using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DUWENINKHopenTools.Entity;
using DUWENINKHopenTools.Extentions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using XD.Base;

namespace DUWENINKHopenTools.Tools
{
   public class ExcelAndTable
    {
        /// <summary>
        /// xls
        /// </summary>
        /// <param name="dtOutDataTable"></param>
        /// <param name="mapList"></param>
        /// <param name="fileStream">文件流</param>
        /// <param name="isStrict">是否严格匹配,如果严格匹配的话,map需要和tanle相等</param>
        /// <param name="sheetIndex">第几个sheet页</param>
        /// <param name="startRow">从第几行开始读(这一行将会作为列名存在)</param>
        /// <returns></returns>
        public DbMessage ReadXlsToDataTable(ref DataTable dtOutDataTable, List<Entity.Map> mapList,Stream fileStream, bool isStrict=false, int sheetIndex=0,int startRow=0)
        {
            DbMessage message = new DbMessage();
            if (mapList.GroupBy(c => c.DbColunmsName).Count(o => o.Count() > 1) != 0|| mapList.GroupBy(c => c.ExcelName).Count(o => o.Count() > 1) != 0)
            {
                message.Success = false;
                message.Message = "Map中有重复项";
                return message;
            }
            if (isStrict)
            {
                if (dtOutDataTable.Columns.Count != mapList.Count)
                {
                    message.Success = false;
                    message.Message = "对应关系数量和DataTable列数不匹配";
                    return message;
                }
            }
            try
            {
                //根据路径通过已存在的excel来创建HSSFWorkbook，即整个excel文档
                HSSFWorkbook workbook = new HSSFWorkbook(fileStream);
                //获取excel的第一个sheet
                ISheet sheet = workbook.GetSheetAt(sheetIndex);
                DataTable table = new DataTable();
                //获取sheet的首行
                IRow headerRow = sheet.GetRow(startRow);
                //一行最后一个方格的编号 即总的列数
                int cellCount = headerRow.LastCellNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(
                        mapList.FirstOrDefault(c => c.ExcelName == headerRow.GetCell(i).StringCellValue) == null
                            ? headerRow.GetCell(i).StringCellValue
                            : mapList.FirstOrDefault(c => c.ExcelName == headerRow.GetCell(i).StringCellValue)
                                ?.DbColunmsName);
                    table.Columns.Add(column);
                }
                //最后一列的标号  即总的行数
                
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                        continue;
                    DataRow dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                            dataRow[j] = row.GetCell(j).ToString();
                    }

                    table.Rows.Add(dataRow);
                }

                var totalRows = sheet.LastRowNum - sheet.FirstRowNum;
                dtOutDataTable = table;
                message.Success = true;
                message.Message =
                    $"从 {sheetIndex} 行开始读取的数据;\r\n严格模式: {(isStrict ? "开启" : "关闭")} ;\r\n共读取数据: {totalRows} 行;";
            }
            catch(Exception exception)
            {
                message.Success = false;
                message.Message = exception.Message;
            }

            return message;


        }
    }
}
