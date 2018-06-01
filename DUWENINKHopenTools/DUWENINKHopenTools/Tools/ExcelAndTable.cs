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
        /// <param name="mapList">Excel列的中文名和数据表中的英文名对应关系,共识:MapList中无论中文名还是英文名都不能重复</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="isStrict">是否严格匹配,如果严格匹配的话,map需要和table相等,否则只匹配存在的</param>
        /// <param name="sheetIndex">第几个sheet页</param>
        /// <param name="startRow">从第几行开始读(这一行将会作为列名存在)</param>
        /// <returns></returns>
        public DbMessage ReadXlsToDataTable(ref DataTable dtOutDataTable, List<Entity.Map> mapList,Stream fileStream, bool isStrict=false, int sheetIndex=0,int startRow=0)
        {
            DbMessage message = new DbMessage();
            if (MapListHasRepeat(mapList))
            {
                message.Success = false;
                message.Message = "Map中有重复项";
                return message;
            }
            try
            {
                //根据路径通过已存在的excel来创建HSSFWorkbook，即整个excel文档
                HSSFWorkbook workbook = new HSSFWorkbook(fileStream);
                //获取excel的第一个sheet
                ISheet sheet = workbook.GetSheetAt(sheetIndex);
                //DataTable table = new DataTable();
                //获取sheet的首行
                IRow headerRow = sheet.GetRow(startRow);
                //一行最后一个方格的编号 即总的列数
                int cellCount = headerRow.LastCellNum;
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    var map = mapList.FirstOrDefault(c => c.ExcelName.Equals(headerRow.GetCell(i).StringCellValue) );
                    if (map != null)
                    {
                        DataColumn column = new DataColumn(map.DbColunmsName);
                        dtOutDataTable.Columns.Add(column);
                    }
                    
                }
                if (isStrict)
                {
                    if (headerRow.LastCellNum - headerRow.FirstCellNum != mapList.Count)
                    {
                        message.Success = false;
                        message.Message = "对应关系数量和DataTable列数不匹配";
                        return message;
                    }
                    if (dtOutDataTable.Columns.Count< mapList.Count)
                    {
                        message.Success = false;
                        message.Message = "以下MapList在Excel中找不到对应关系:" +string.Join(";", mapList.ConvertAll(c => c.DbColunmsName).Except(dtOutDataTable.GetColumnsStringList()));
                        return message;
                    }
                }



                //最后一列的标号  即总的行数

                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                        continue;
                    DataRow dataRow = dtOutDataTable.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                            dataRow[j] = row.GetCell(j).ToString();
                    }

                    dtOutDataTable.Rows.Add(dataRow);
                }

                var totalRows = sheet.LastRowNum - sheet.FirstRowNum;
                message.Success = true;
                message.Message =
                    $"从第 {sheetIndex} 个Sheet {startRow} 行开始读取的数据;\r\n严格模式: {(isStrict ? "开启" : "关闭")} ;\r\n共读取数据: {totalRows} 行;";
            }
            catch(Exception exception)
            {
                message.Success = false;
                message.Message = exception.Message;
            }

            return message;


        }

        /// <summary>
        /// 检测是否有重复项
        /// </summary>
        /// <param name="mapList"></param>
        /// <returns></returns>
        private bool MapListHasRepeat(List<Entity.Map> mapList)
        {
            return mapList.GroupBy(c => c.DbColunmsName).Count(o => o.Count() > 1) != 0 ||
                   mapList.GroupBy(c => c.ExcelName).Count(o => o.Count() > 1) != 0;
        }
    }
}
