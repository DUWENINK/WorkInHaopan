﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DUWENINKHopenTools.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace DUWENINKHopenTools.DB
{
    
   public class DbOp
    {
        private readonly string _connectionString = "ConnectionString";
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
        public DbMessage Insert<T>(T t, string primarykeyField = null)
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
                    if (ceil.CeilName == (primarykeyField == null ? "" : primarykeyField.ToUpper())) continue;
                    object value = entityType.First(x => x.Name == p.Name).GetValue(t, null);
                    if (value == null) continue;
                    ceil.CeilValue = value;
                    ceil.CeilType = GetDbType(p.PropertyType.Name);
                    ceilsList.Add(ceil);
                }

                StringBuilder sbName = new StringBuilder();
                StringBuilder sbValue = new StringBuilder();
                sbName.Append(IsMySqlDb(db) ? "(" : "([");
                sbValue.Append("(");
                if (IsMySqlDb(db))
                {
                    for (int i = 0; i < ceilsList.Count; i++)
                    {
                        sbName.Append(ceilsList[i].CeilName + (i == ceilsList.Count - 1 ? string.Empty : ","));
                        sbValue.Append("@" + ceilsList[i].CeilName + (i == ceilsList.Count - 1 ? string.Empty : ","));
                    }
                }
                else
                {
                    for (int i = 0; i < ceilsList.Count; i++)
                    {
                        sbName.Append(ceilsList[i].CeilName + (i == ceilsList.Count - 1 ? "]" : "],["));
                        sbValue.Append("@" + ceilsList[i].CeilName + (i == ceilsList.Count - 1 ? string.Empty : ","));
                    }
                }

                sbName.Append(")");
                sbValue.Append(")");
                sql = sql + sbName + (IsMySqlDb(db) ? "value" : "values") + sbValue;
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

        public List<T> Select<T>(T t=default(T))
        {
            Database db = new DatabaseProviderFactory().Create(_connectionString);
          
            if (t == null)
            {
                var type = t.GetType();//type.tostring()
                string sql = "insert into " + type.Name;

            }
            else
            {
                
            }
        }


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
                default:

                    break;

            }
            return dbtype;
        }


        private  bool IsMySqlDb(Database db)
        {
            //IL_0000: Unknown result type (might be due to invalid IL or missing references)
            return (db.DbProviderFactory == null ? "" : db.DbProviderFactory.ToString()) == "MySql.Data.MySqlClient.MySqlClientFactory";
        }


    }
}