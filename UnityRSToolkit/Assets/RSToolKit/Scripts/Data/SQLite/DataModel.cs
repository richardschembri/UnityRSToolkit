using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RSToolkit.Data.SQLite
{
    public class DataModel
    {

        public class DataModelColumn
        {
            //Make enum
            public static class DataType_Keys
            {
                public const string DT_INTEGER = "INTEGER";
                public const string DT_REAL = "REAL";
                public const string DT_TEXT = "TEXT";
                public const string DT_BLOB = "BLOB";
            }
            public string ColumnName { get; set; }
            public string ColumnType { get; set; }
            public bool IsUnique { get; set; }
            public object DefaultValue { get; set; }
            public bool IsPrimaryKey { get; set; }
            public bool IsNullable { get; set; }
            public bool IsAutoIncrement { get; set; }
            public string ForeignTableName { get; set; }
            private object m_value;

            public DataModelColumn(string columnName, string columnType, bool isUnique = false, bool isPrimaryKey = false, bool isNullable = true, string foreignTableName = "", bool isAutoIncrement = false, object defaultValue = null)
            {
                this.ColumnName = columnName;
                this.ColumnType = columnType;
                this.IsUnique = isUnique;
                this.IsPrimaryKey = isPrimaryKey;
                this.IsNullable = isNullable;
                this.IsAutoIncrement = isAutoIncrement;
                this.ForeignTableName = foreignTableName;
                this.DefaultValue = defaultValue;
            }

            public bool IsForeignKey()
            {
                return !string.IsNullOrEmpty(this.ForeignTableName);
            }

            public string GetColumnCodeForCreateTable()
            {
                var sbColumnCode = new StringBuilder();
                sbColumnCode.Append(string.Format("{0} {1} ", ColumnName, ColumnType));
                if (IsPrimaryKey)
                {
                    sbColumnCode.Append("PRIMARY KEY ");
                }
                if (IsAutoIncrement)
                {
                    sbColumnCode.Append("AUTOINCREMENT ");
                }
                if (!IsNullable)
                {
                    sbColumnCode.Append("NOT NULL ");
                }
                if (IsUnique)
                {
                    sbColumnCode.Append("UNIQUE ");
                }
                if (DefaultValue != null)
                {

                    if (DefaultValue is bool || DefaultValue is bool?)
                    {
                        sbColumnCode.AppendFormat("DEFAULT '{0}' ", Convert.ToInt32(DefaultValue));
                    }
                    else
                    {
                        sbColumnCode.AppendFormat("DEFAULT '{0}' ", DefaultValue);
                    }

                }

                return sbColumnCode.ToString();
            }

            public string GetParameterName()
            {
                return string.Format("@{0}", ColumnName);
            }

            public string GetForeignKeyCodeForCreateTable()
            {

                var foreignKeyCode = GetForeignKeyCode();
                if (!string.IsNullOrEmpty(foreignKeyCode))
                {
                    return string.Format(", {0}", foreignKeyCode);
                }
                return string.Empty;
            }

            public string GetForeignKeyCode()
            {

                if (!string.IsNullOrEmpty(ForeignTableName))
                {
                    return string.Format("FOREIGN KEY ({0}) REFERENCES {1}({0})", ColumnName, ForeignTableName);
                }
                return string.Empty;
            }

            public T GetValue<T>()
            {
                return (T)m_value;
            }

            public void SetValue<T>(T value)
            {
                m_value = value;
            }

        }

        public string TableName { get; set; }
        //public Dictionary<string, string> TableColumnAndType{get; internal set;}
        #region Columns
        public List<DataModelColumn> DataModelColumns { get; internal set; }

        public List<DataModelColumn> Get_ForeignKeys()
        {
            return DataModelColumns.Where(dc => dc.IsForeignKey()).ToList();
        }
        public List<DataModelColumn> Get_DataModelColumnsByName(string[] columnNames)
        {
            var enm_dmc = DataModelColumns.Where(dmc => columnNames.Contains(dmc.ColumnName));
            if (enm_dmc != null)
            {
                return enm_dmc.ToList();
            }
            return new List<DataModelColumn>();
        }

        public DataModelColumn Get_PrimaryKey()
        {
            return DataModelColumns.FirstOrDefault(dmc => dmc.IsPrimaryKey);
        }
        #endregion
        public bool IsForeignKey { get; set; }

        public DataModel()
        {
            DataModelColumns = new List<DataModelColumn>();
            IsForeignKey = false;

        }


        public string GetCommandText_Insert()
        {
            var sbQuery = new StringBuilder();

            var pkDMC = DataModelColumns[0];
            int index = 0;
            if (pkDMC.IsAutoIncrement)
            {
                index++;
            }
            sbQuery.AppendFormat("INSERT INTO {0}({1}", TableName, DataModelColumns[index].ColumnName);
            var sbQ = new StringBuilder();


            sbQ.AppendFormat("@{0}", DataModelColumns[index].ColumnName);
            for (int i = index + 1; i < DataModelColumns.Count; i++)
            {
                sbQuery.AppendFormat(", {0}", DataModelColumns[i].ColumnName);
                sbQ.AppendFormat(",@{0}", DataModelColumns[i].ColumnName);
            }
            sbQuery.Append(") VALUES (");
            sbQuery.Append(sbQ.ToString());
            sbQuery.Append(")");

            return sbQuery.ToString();
        }

        public virtual string GetCommandText_BasicSelect(bool selectAll = true, int pageSize = 0, int startIndex = 0)
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendFormat("SELECT {0}", DataModelColumns[0].ColumnName);
            for (int i = 1; i < DataModelColumns.Count; i++)
            {
                sbQuery.AppendFormat(", {0}", DataModelColumns[i].ColumnName);
            }

            var primaryKey = DataModelColumns.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat(" FROM {0}", TableName);
            if (!selectAll)
            {
                sbQuery.AppendLine(string.Format(" WHERE {0} = @{0}", primaryKey.ColumnName));
            }
            sbQuery.AppendLine(string.Format(" ORDER BY {0}", primaryKey.ColumnName));
            if (pageSize > 0)
            {
                sbQuery.AppendLine(string.Format(" LIMIT {0}, {1}", pageSize, startIndex));
            }
            return sbQuery.ToString();
        }
        public virtual string GetCommandText_BasicSelectByParameters(List<DataModelColumn> parameters, int pageSize = 0, int startIndex = 0)
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendFormat("SELECT {0}", DataModelColumns[0].ColumnName);
            for (int i = 1; i < DataModelColumns.Count; i++)
            {
                sbQuery.AppendFormat(", {0}", DataModelColumns[i].ColumnName);
            }

            var primaryKey = DataModelColumns.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat(" FROM {0} WHERE ", TableName);
            for (int pi = 0; pi < parameters.Count; pi++)
            {
                if (pi > 0)
                {
                    sbQuery.Append(" AND");
                }
                sbQuery.AppendLine(string.Format(" {0} = @{0}", parameters[pi].ColumnName));
            }
            sbQuery.AppendLine(string.Format(" ORDER BY {0}", primaryKey.ColumnName));
            if (pageSize > 0)
            {
                sbQuery.AppendLine(string.Format(" LIMIT {0}, {1}", pageSize, startIndex));
            }
            return sbQuery.ToString();
        }

        public virtual string GetCommandText_SelectByPrimaryKey()
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendFormat("SELECT {0}", DataModelColumns[0].ColumnName);
            for (int i = 1; i < DataModelColumns.Count; i++)
            {
                sbQuery.AppendFormat(", {0}", DataModelColumns[i].ColumnName);
            }

            var primaryKey = DataModelColumns.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat(" WHERE {0} = @{0}", primaryKey.ColumnName);

            sbQuery.AppendFormat(" FROM {0} ORDER BY {1}", TableName, primaryKey.ColumnName);
            return sbQuery.ToString();
        }

        public string GetCommandText_Update()
        {
            var sbQuery = new StringBuilder();
            var lstDMC = DataModelColumns.Where(dmc => !dmc.IsPrimaryKey).ToList();
            sbQuery.AppendFormat("UPDATE {0} SET {1} = @{1} ", TableName, lstDMC[0].ColumnName);

            for (int i = 1; i < lstDMC.Count; i++)
            {
                sbQuery.AppendFormat(",{0} = @{0} ", lstDMC[i].ColumnName);
            }
            var primaryKey = DataModelColumns.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat("WHERE {0} = @{0}", primaryKey.ColumnName);
            return sbQuery.ToString();
        }

        public string GetCommandText_Delete()
        {
            var sbQuery = new StringBuilder();
            var primaryKey = DataModelColumns.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat("DELETE FROM {0} WHERE {1} = @{1} ", TableName, primaryKey.ColumnName);

            return sbQuery.ToString();
        }

        public string GetCommandText_Delete(List<DataModelColumn> parameters)
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendFormat("DELETE FROM {0} WHERE {1} = @{1} ", TableName, parameters[0].ColumnName);
            for (int i = 1; i < parameters.Count; i++)
            {
                sbQuery.AppendFormat("AND {0} = @{0} ", parameters[i].ColumnName);
            }

            return sbQuery.ToString();
        }

        public string GetCommandText_Delete(SqliteParameterCollection parameters)
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendFormat("DELETE FROM {0} WHERE {1} = @{1} ", TableName, parameters[0].ParameterName);
            for (int i = 1; i < parameters.Count; i++)
            {
                sbQuery.AppendFormat("AND {0} = @{0} ", parameters[i].ParameterName);
            }

            return sbQuery.ToString();
        }

        public string GetCommandText_DeleteAll()
        {
            return string.Format("DELETE FROM {0}", TableName);
        }

        public string GetCommandText_LatestPrimaryKey()
        {
            var sbQuery = new StringBuilder();
            var primaryKey = Get_PrimaryKey();
            sbQuery.AppendFormat("SELECT {1} FROM {0} ORDER BY {1} DESC LIMIT 1 ", TableName, primaryKey.ColumnName);

            return sbQuery.ToString();
        }

        public virtual SqliteCommand AddParameters_AllColumns(SqliteCommand cmd, bool includePrimaryKey = true)
        {
            Debug.LogError("AddAllColumnsAsParameters not implemented");
            return cmd;
        }

        public virtual SqliteCommand AddParameter_PrimaryKey(SqliteCommand cmd)
        {
            Debug.LogError("AddPrimaryKeyAsParameter not implemented");
            return cmd;
        }

        public virtual CSVDataModel Get_CSVRawDataModel()
        {
            throw new NotImplementedException("GetCSVRawDataModel");
        }

        public SqliteCommand GetCommand_Insert()
        {
            var query = GetCommandText_Insert();
            var insertSQL = new SqliteCommand(query);
            var pk = DataModelColumns.FirstOrDefault(dmc => dmc.IsPrimaryKey);
            bool includePK = true;
            if (pk != null)
            {
                includePK = !pk.IsAutoIncrement;
            }
            insertSQL = AddParameters_AllColumns(insertSQL, includePK);
            return insertSQL;
        }

        public SqliteCommand GetCommand_Update()
        {
            var query = GetCommandText_Update();
            var updateSQL = new SqliteCommand(query);
            updateSQL = AddParameters_AllColumns(updateSQL);
            return updateSQL;
        }

        public SqliteCommand GetCommand_Delete()
        {
            var query = GetCommandText_Delete();
            var deleteSQL = new SqliteCommand(query);
            deleteSQL = AddParameter_PrimaryKey(deleteSQL);

            return deleteSQL;
        }

        public SqliteCommand GetCommand_DeleteAll()
        {
            var query = GetCommandText_DeleteAll();
            var cmd = new SqliteCommand(query);

            return cmd;
        }

        public SqliteCommand GetCommand_LatestPrimaryKey()
        {
            var query = GetCommandText_LatestPrimaryKey();
            var cmd = new SqliteCommand(query);

            return cmd;
        }

        public virtual int ReadFromDatabase(SqliteDataReader reader, int index = 0)
        {
            throw new NotImplementedException("ReadFromDatabase");
        }


    }

    public class CSVDataModel
    {
        public virtual string GetCSVHeaderLine()
        {
            throw new NotImplementedException("GetCSVHeaderLine");
        }
        public virtual string GetCSVLine()
        {
            throw new NotImplementedException("GetCSVLine");
        }

        public List<CSVDataModel> GetCSVFromTable<T>(List<T> lstDataModel)
        {
            var lstCSVDataModel = new List<CSVDataModel>();
            for (int i = 0; i < lstDataModel.Count; i++)
            {
                var dm = lstDataModel[i] as DataModel;
                lstCSVDataModel.Add(dm.Get_CSVRawDataModel());
            }

            return lstCSVDataModel;
        }
    }
}