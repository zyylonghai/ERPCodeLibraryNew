using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace LibraryBaseDal
{
    public static class EntityFrameworkCoreExtension
    {
        private static DbCommand CreateCommand(DatabaseFacade facade, string sql, out DbConnection connection)
        {
            var conn = facade.GetDbConnection();
            connection = conn;
            conn.Open();
            var cmd = conn.CreateCommand();
            if (facade.IsSqlServer())
            {
                cmd.CommandText = sql;
                //cmd.Parameters.AddRange(parameters);
            }
            return cmd;
        }

        public static DataTable SqlQuery(this DatabaseFacade facade, string sql)
        {
            var dt = new DataTable();
            var command = CreateCommand(facade, sql, out DbConnection conn);
            try
            {
                var reader = command.ExecuteReader();
                dt.Load(reader);
                reader.Close();
            }
            catch(Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }
            return dt;
        }

        public static void ExecuteSql(this DatabaseFacade facade, string sql)
        {
            var command = CreateCommand(facade, sql, out DbConnection conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public static void ExeStoredProcedure(this DatabaseFacade facade, string storeProcedureNm, LibDbParameter[] parameters)
        {
            try
            {
                var command = CreateCommand(facade, string.Empty, out DbConnection conn);
                command.CommandText = storeProcedureNm;
                command.CommandType = CommandType.StoredProcedure;
                DbParameter parameter = null;
                command.Parameters.Clear();
                foreach (LibDbParameter item in parameters)
                {
                    parameter = command.CreateParameter();
                    parameter.ParameterName = item.ParameterNm;
                    parameter.DbType = item.DbType;
                    parameter.Direction = item.Direction;
                    parameter.Value = item.Value;
                    parameter.Size = item.Size;
                    command.Parameters.Add(parameter);
                }
                command.ExecuteNonQuery();
                foreach (LibDbParameter p in parameters)
                {
                    if (p.Direction == ParameterDirection.ReturnValue || p.Direction == ParameterDirection.Output)
                    {
                        p.Value = command.Parameters[p.ParameterNm].Value;
                    }
                }
                conn.Close();
            }
            catch (Exception ex) {
               
            }
        }

        public static DataTable ExeStoredProcedureToData(this DatabaseFacade facade, string storeProcedureNm, LibDbParameter[] parameters) {
            var command = CreateCommand(facade, string.Empty, out DbConnection conn);
            command.CommandText = storeProcedureNm;
            command.CommandType = CommandType.StoredProcedure;
            DbParameter parameter = null;
            command.Parameters.Clear();
            foreach (LibDbParameter item in parameters)
            {
                parameter = command.CreateParameter();
                parameter.ParameterName = item.ParameterNm;
                parameter.DbType = item.DbType;
                parameter.Direction = item.Direction;
                parameter.Value = item.Value == null ? DBNull.Value : item.Value;
                parameter.Size = item.Size;
                command.Parameters.Add(parameter);
            }
            var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            reader.Close();
            foreach (LibDbParameter p in parameters)
            {
                if (p.Direction == ParameterDirection.ReturnValue || p.Direction == ParameterDirection.Output)
                {
                    p.Value = command.Parameters[p.ParameterNm].Value;
                }
            }
            conn.Close();
            return dt;
        }

        public static List<T> SqlQuery<T>(this DatabaseFacade facade, string sql) where T : class, new()
        {
            var dt = SqlQuery(facade, sql);
            return dt.ToList<T>();
        }

        public static List<T> ToList<T>(this DataTable dt) where T : class, new()
        {
            //string ff = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            var propertyInfos = typeof(T).GetProperties();
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var t = new T();
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        p.SetValue(t, row[p.Name], null);
                }
                list.Add(t);
            }
            //string ff1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            return list;
        }
        public static List<object> ToList(this DataTable dt,Type targettype)
        {
            var propertyInfos = targettype.GetProperties();
            var list = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                var t = Activator.CreateInstance(targettype);
                foreach (PropertyInfo p in propertyInfos)
                {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                    {
                        if (p.PropertyType.Equals(typeof(int)))
                        {
                            p.SetValue(t, Convert.ToInt32(row[p.Name]), null);
                        }
                        else if (p.PropertyType.Equals(typeof(decimal)))
                        {
                            p.SetValue(t, Convert.ToDecimal(row[p.Name]), null);
                        }
                        else if (p.PropertyType.BaseType.Equals(typeof(Enum)))
                        {
                            p.SetValue(t,Convert.ToInt32(row[p.Name]), null);
                        }
                        else
                            p.SetValue(t, row[p.Name], null);
                    }
                }
                list.Add(t);
            }
            return list;
        }
    }
}
