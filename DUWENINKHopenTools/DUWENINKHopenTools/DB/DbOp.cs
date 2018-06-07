using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DUWENINKHopenTools.Entity;
using DUWENINKHopenTools.Extentions;
using DUWENINKHopenTools.Tools;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace DUWENINKHopenTools.DB
{
    
   public class DbOp
    {
        private readonly string _connectionString = "ConnectionString";
        /// <summary>
        /// 类的构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        public DbOp(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                _connectionString = connectionString;
            }
        }

        /// <summary>
        /// 实体类的插入方法
        /// Author：DUWENINK
        /// </summary>
        /// <typeparam name="T">实体类的类型,类型必须和表名严格对应</typeparam>
        /// <param name="t">实体类变量</param>
        /// <param name="primarykeyField">自增型主键,跳过插入</param>
        /// <param name="needParameterReturn">是否返回参数化sql</param>
        public DbMessage Insert<T>(T t, string primarykeyField = null, bool needParameterReturn=true)
        {
            DbMessage message=new DbMessage();
            try
            {
                Database db = new DatabaseProviderFactory().Create(_connectionString);
                List<Ceils> ceilsList = new List<Ceils>();
                var type = t.GetType(); //type.tostring()
                string sql = "insert into " + type.Name;
                var entityType = t.GetType().GetProperties();
                foreach (var p in entityType)
                {
                    Ceils ceil = new Ceils {CeilName = p.Name.ToUpper()};
                    if (ceil.CeilName == (primarykeyField == null ? string.Empty : primarykeyField.ToUpper())) continue;
                    object value = entityType.First(x => x.Name == p.Name).GetValue(t, null);
                    if (value == null) continue;
                    ceil.CeilValue = value;
                    ceil.CeilType = GetDbType(p.PropertyType.Name);
                    ceilsList.Add(ceil);
                }
                StringBuilder sbName = new StringBuilder();
                StringBuilder sbValue = new StringBuilder();
                StringBuilder sbText = new StringBuilder();
                sbName.Append(IsMySqlDb(db) ? "(" : "([");
                sbValue.Append("(");
                sbText.Append("(");
                if (IsMySqlDb(db))
                {
                    for (int i = 0; i < ceilsList.Count; i++)
                    {
                        sbName.Append(ceilsList[i].CeilName + (i == ceilsList.Count - 1 ? string.Empty : ","));
                        sbValue.Append("@" + ceilsList[i].CeilName + (i == ceilsList.Count - 1 ? string.Empty : ","));
                        sbText.Append("'" + ceilsList[i].CeilValue + "'" + (i == ceilsList.Count - 1 ? string.Empty : ","));
                    }
                }
                else
                {
                    for (int i = 0; i < ceilsList.Count; i++)
                    {
                        sbName.Append(ceilsList[i].CeilName + (i == ceilsList.Count - 1 ? "]" : "],["));
                        sbValue.Append("@" + ceilsList[i].CeilName + (i == ceilsList.Count - 1 ? string.Empty : ","));
                        sbText.Append("'" + ceilsList[i].CeilValue +"'"+ (i == ceilsList.Count - 1 ? string.Empty : ","));
                    }
                }

                sbName.Append(")");
                sbValue.Append(")");
                sbText.Append(")");
                sql = sql + sbName + (IsMySqlDb(db) ? "value" : "values") +( needParameterReturn? sbValue.ToString(): sbText.ToString());
                DbCommand cmd = db.GetSqlStringCommand(sql);
                ceilsList.ForEach(m => { db.AddInParameter(cmd, m.CeilName, m.CeilType, m.CeilValue); });
                db.ExecuteNonQuery(cmd);
                message.Success = true;
                message.Message = sql;
            }
            catch(Exception exception)
            {

                message.Success = false;
                message.Message = exception.Message;
            }

            return message;
        }
        /// <summary>
        /// 根据给的实体类删除数据,删除条件不能为null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns>是否删除成功,删除成功返回语句,删除失败抛出异常</returns>
        public DbMessage Delete<T>(T t)
        {
            DbMessage message = new DbMessage();
            if (t == null)
            {
                message.Success = false;
                message.Message = "传入的参数为null,不做任何操作";
                return message;
            }
            try
            {
            Database db = new DatabaseProviderFactory().Create(_connectionString);
            List<Ceils> ceilsList = new List<Ceils>();
            var type = t.GetType();//type.tostring()
            string sql = "delete  from  " + type.Name +" where 1 = 1 ";
            var entityType = t.GetType().GetProperties();
            foreach (var p in entityType)
            {
                Ceils ceil = new Ceils { CeilName = p.Name.ToUpper() };
                object value = entityType.First(x => x.Name == p.Name).GetValue(t, null);
                if (value == null) continue;
                ceil.CeilValue = value;
                ceil.CeilType = GetDbType(p.PropertyType.Name);
                ceilsList.Add(ceil);
            }
            StringBuilder sbCondition = new StringBuilder();
            ceilsList.ForEach(x => { sbCondition.Append(" and " + x.CeilName + "=@" + x.CeilName); });
           
            sql += sbCondition;
            DbCommand cmd = db.GetSqlStringCommand(sql);
            ceilsList.ForEach(m =>
            {
                db.AddInParameter(cmd, m.CeilName, m.CeilType, m.CeilValue);
            });
            db.ExecuteNonQuery(cmd);
                message.Success = true;
                message.Message = sql;
            }
            catch (Exception exception)
            {

                message.Success = false;
                message.Message = exception.Message;
            }

            return message;
        }
        /// <summary>
        /// 根据entity选取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public List<T> Select<T>(T t) where T : class, new()
        {
            Database db = new DatabaseProviderFactory().Create(_connectionString);
           
            if (t == null)
            {
               return new List<T>();
            }

            var type = t.GetType();//type.tostring()
            string sql = "select * from " + type.Name +" where  1 = 1 ";
            List<Ceils> ceilsList = new List<Ceils>();
            var entityType = t.GetType().GetProperties();
            foreach (var p in entityType)
            {
                Ceils ceil = new Ceils { CeilName = p.Name.ToUpper() };
                object value = entityType.First(x => x.Name == p.Name).GetValue(t, null);
                if (value == null) continue;
                ceil.CeilValue = value;
                ceil.CeilType = GetDbType(p.PropertyType.Name);
                ceilsList.Add(ceil);
            }
            StringBuilder sbCondition = new StringBuilder();
            ceilsList.ForEach(x => { sbCondition.Append(" and " + x.CeilName + "=@" + x.CeilName); });

            sql += sbCondition;
            DbCommand cmd = db.GetSqlStringCommand(sql);
            ceilsList.ForEach(m =>
            {
                db.AddInParameter(cmd, m.CeilName, m.CeilType, m.CeilValue);
            });
            return db.ExecuteDataSet(cmd).Tables[0].ToList<T>();
        }


        /// <summary>
        /// 数据库字段属性和参数属性的对应关系
        /// </summary>
        /// <param name="propertyTypeName"></param>
        /// <returns></returns>
        private DbType GetDbType(string propertyTypeName)
        {
            DbType dbtype = DbType.String;
            switch (propertyTypeName)
            {
                case "Int32":
                    dbtype = DbType.Int32;
                    break;
                case "String":
                    dbtype = DbType.String;
                    break;
                case "DateTime":
                    dbtype = DbType.DateTime;
                    break;
                case "Decimal":
                    dbtype = DbType.Decimal;
                    break;
                case "Int64":
                    dbtype = DbType.Int64;
                    break;
                case "Guid":
                    dbtype = DbType.Guid;
                    break;
                case "Object":
                    dbtype = DbType.Object;
                    break;
                default:

                    break;

            }
            return dbtype;
        }


        private List<T> Get<T>()
        {
            return new List<T>();
        }





        /// <summary>
        /// 批量保存(只适用于insert语句)
        /// </summary>
        /// <param name="dt">要插入的数据源</param>
        /// <param name="list">要插入的列名集合</param>
        /// <param name="tablename">要插入的表名</param>
        public DbMessage SBulkToDb(DataTable dt, List<string> list, string tablename)
        {
            DbMessage message = new DbMessage();
            try
            {
                Database db = new DatabaseProviderFactory().Create(_connectionString);
                if (IsMySqlDb(db))
                {
                    message.Message = "批量插入暂不支持MySql数据库";
                    message.Success = false;
                    return message;
                }
                SqlConnection sqlConn = new SqlConnection(db.ConnectionString);
                SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn)
                {
                    DestinationTableName = tablename,
                    BatchSize = dt.Rows.Count
                };
                if (list != null && list.Count > 0)
                {
                    list.ForEach(x =>
                    {
                        bulkCopy.ColumnMappings.Add(x, x);
                    });
                }
                else
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                }
                try
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    sqlConn.Open();
                    if (dt.Rows.Count != 0)
                        bulkCopy.WriteToServer(dt);
                    watch.Stop();
                    message.Message ="插入耗时:"+ watch.Elapsed.Seconds;
                    message.Success = true;
                }
                finally
                {
                    sqlConn.Close();
                    bulkCopy.Close();
                }
            }
            catch(Exception exception)
            {
                message.Message = exception.Message;
                message.Success = false;
            }

            return message;
        }
        /// <summary>
        /// 判断是否是mysql数据库
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private static bool IsMySqlDb(Database db)
        {
            return (db.DbProviderFactory == null ? string.Empty : db.DbProviderFactory.ToString()) == "MySql.Data.MySqlClient.MySqlClientFactory";
        }
    }
}
