using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RSToolkit.Data.SQLite
{
    // Work in progress
    public abstract class DataModelFactory<T> where T : DataModel
    {
        public List<DataModel.IDataModelColumnProperties> DataModelColumnProperties { get; protected set; } = new List<DataModel.IDataModelColumnProperties>();
        public string TableName { get; private set; }


        public List<T> DataModels { get; private set; } = new List<T>();

        public abstract T GenerateDataModel();

        public abstract void GenerateDataColumnProperties();
        
        public DataModelFactory(string tableName)
        {
            TableName = tableName;
            GenerateDataColumnProperties();
        }

        public T GenerateDataModel(SqliteDataReader reader, ref int index)
        {
            T result = GenerateDataModel();
            result.ReadFromDatabase(reader, ref index);
            return result;
        }

        public virtual void GeneratePresets()
        {
            throw new System.Exception("Not implemented");
        }

        public string GetCommandText_Insert()
        {
            var sbQuery = new StringBuilder();

            int index = 0;
            if (DataModelColumnProperties[0].IsAutoIncrement)
            {
                index++;
            }
            sbQuery.AppendFormat("INSERT INTO {0}({1}", TableName, DataModelColumnProperties[index].ColumnName);
            var sbQ = new StringBuilder();


            sbQ.AppendFormat("@{0}", DataModelColumnProperties[index].ColumnName);
            for (int i = index + 1; i < DataModelColumnProperties.Count; i++)
            {
                sbQuery.AppendFormat(", {0}", DataModelColumnProperties[i].ColumnName);
                sbQ.AppendFormat(",@{0}", DataModelColumnProperties[i].ColumnName);
            }
            sbQuery.Append(") VALUES (");
            sbQuery.Append(sbQ.ToString());
            sbQuery.Append(")");

            return sbQuery.ToString();
        }

        public virtual string GetCommandText_BasicSelect(bool selectAll = true, int pageSize = 0, int startIndex = 0)
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendFormat("SELECT {0}", DataModelColumnProperties[0].ColumnName);
            for (int i = 1; i < DataModelColumnProperties.Count; i++)
            {
                sbQuery.AppendFormat(", {0}", DataModelColumnProperties[i].ColumnName);
            }

            var primaryKey = DataModelColumnProperties.First(dmc => dmc.IsPrimaryKey);
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

        public virtual string GetCommandText_BasicSelectWithParameters(List<DataModel.IDataModelColumn> parameters, int pageSize = 0, int startIndex = 0)
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendFormat("SELECT {0}", DataModelColumnProperties[0].ColumnName);
            for (int i = 1; i < DataModelColumnProperties.Count; i++)
            {
                sbQuery.AppendFormat(", {0}", DataModelColumnProperties[i].ColumnName);
            }

            var primaryKey = DataModelColumnProperties.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat(" FROM {0} WHERE ", TableName);
            for (int pi = 0; pi < parameters.Count; pi++)
            {
                if (pi > 0)
                {
                    sbQuery.Append(" AND");
                }
                sbQuery.AppendLine(string.Format(" {0} = @{0}", parameters[pi].GetColumnProperties().ColumnName));
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
            sbQuery.AppendFormat("SELECT {0}", DataModelColumnProperties[0].ColumnName);
            for (int i = 1; i < DataModelColumnProperties.Count; i++)
            {
                sbQuery.AppendFormat(", {0}", DataModelColumnProperties[i].ColumnName);
            }

            var primaryKey = DataModelColumnProperties.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat(" WHERE {0} = @{0}", primaryKey.ColumnName);

            sbQuery.AppendFormat(" FROM {0} ORDER BY {1}", TableName, primaryKey.ColumnName);
            return sbQuery.ToString();
        }

        public string GetCommandText_Update()
        {
            var sbQuery = new StringBuilder();
            var lstDMC = DataModelColumnProperties.Where(dmc => !dmc.IsPrimaryKey).ToList();
            sbQuery.AppendFormat("UPDATE {0} SET {1} = @{1} ", TableName, lstDMC[0].ColumnName);

            for (int i = 1; i < lstDMC.Count; i++)
            {
                sbQuery.AppendFormat(",{0} = @{0} ", lstDMC[i].ColumnName);
            }
            var primaryKey = DataModelColumnProperties.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat("WHERE {0} = @{0}", primaryKey.ColumnName);
            return sbQuery.ToString();
        }

        public string GetCommandText_Delete()
        {
            var sbQuery = new StringBuilder();
            var primaryKey = DataModelColumnProperties.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat("DELETE FROM {0} WHERE {1} = @{1} ", TableName, primaryKey.ColumnName);

            return sbQuery.ToString();
        }

        public string GetCommandText_Delete(List<DataModel.IDataModelColumnProperties> parameters)
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

        public DataModel.IDataModelColumnProperties Get_PrimaryKeyProperties()
        {
            return DataModelColumnProperties.FirstOrDefault(dmc => dmc.IsPrimaryKey);
        }

        public string GetCommandText_LatestPrimaryKey()
        {
            var sbQuery = new StringBuilder();
            var primaryKey = Get_PrimaryKeyProperties();
            sbQuery.AppendFormat("SELECT {1} FROM {0} ORDER BY {1} DESC LIMIT 1 ", TableName, primaryKey.ColumnName);

            return sbQuery.ToString();
        }


        public SqliteCommand GetCommand_Insert(T dataModel)
        {
            var query = GetCommandText_Insert();
            var insertSQL = new SqliteCommand(query);
            var pk = DataModelColumnProperties.FirstOrDefault(dmc => dmc.IsPrimaryKey);
            bool includePK = true;
            if (pk != null)
            {
                includePK = !pk.IsAutoIncrement;
            }
            insertSQL = dataModel.AddParameters_AllColumns(insertSQL, includePK);
            return insertSQL;
        }

        public SqliteCommand GetCommand_Update(T dataModel)
        {
            var query = GetCommandText_Update();
            var updateSQL = new SqliteCommand(query);
            updateSQL = dataModel.AddParameters_AllColumns(updateSQL);
            return updateSQL;
        }

        public SqliteCommand GetCommand_Delete(T dataModel)
        {
            var query = GetCommandText_Delete();
            var deleteSQL = new SqliteCommand(query);
            dataModel.AddParameter_PrimaryKey(ref deleteSQL);

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
    }
}