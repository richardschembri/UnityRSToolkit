using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSToolkit.Data.SQLite
{ 
    public interface IDataModelFactory
    {
        List<DataModel.IDataModelColumnProperties> DataModelColumnProperties { get; }
        List<DataModel.IDataModelForeignKey> DataModelForeignKeyProperties { get; } 
        string TableName { get; }
        string GetColumnNamesAsCommandText(string tableNamePrefix = "");
        // List<IDataModel> DataModels { get; }
        void GenerateDataColumnProperties();
        // void GeneratePresets();
        // void GenerateDataModel(SqliteDataReader reader, ref int index);
        IDataModel GenerateAndGetIDataModel(SqliteDataReader reader, ref int index);
        IDataModel GenerateDataModelWithOnlyPK(SqliteDataReader reader, ref int index);

        string GetCommandText_Insert();
        // string GetCommandText_BasicSelect(bool selectAll = true, int pageSize = 0, int startIndex = 0);
        string GetCommandText_Select(List<DataModel.IDataModelColumn> parameters = null,
                                        int pageSize = 0, int startIndex = 0,
                                        List<DataModel.IDataModelColumnProperties> orderby = null);
        // string GetCommandText_SelectByPrimaryKey();
        string GetCommandText_Update();
        string GetCommandText_Delete();
        string GetCommandText_Delete(List<DataModel.IDataModelColumnProperties> parameters);
        string GetCommandText_Delete(SqliteParameterCollection parameters);
        string GetCommandText_DeleteAll();
        DataModel.IDataModelColumnProperties Get_PrimaryKeyProperties();
        string GetCommandText_LatestPrimaryKey();

        SqliteCommand GetCommand_DeleteAll();
        SqliteCommand GetCommand_LatestPrimaryKey();
    }
    // Work in progress
    public abstract class DataModelFactory<T> : IDataModelFactory where T : DataModel 
    {
        public List<DataModel.IDataModelColumnProperties> DataModelColumnProperties { get; protected set; } = new List<DataModel.IDataModelColumnProperties>();
        public List<DataModel.IDataModelForeignKey> DataModelForeignKeyProperties { get; private set; } = new List<DataModel.IDataModelForeignKey>();
        public string TableName { get; private set; }

        public string GetColumnNamesAsCommandText(string overrideTableName = "")
        {
            if (!string.IsNullOrEmpty(overrideTableName))
            {
                overrideTableName = TableName;
            }
            var sbCommandText = new StringBuilder();

            string comma = "";
            for (int i = 0; i < DataModelColumnProperties.Count(); i++)
            {
                sbCommandText .Append($"{comma}{overrideTableName}.{DataModelColumnProperties[i].ColumnName}");
                comma = " ,";
            }
            for (int i = 0; i < DataModelForeignKeyProperties.Count(); i++)
            {
                sbCommandText.Append(" ,");
                sbCommandText.Append(
                    DataModelForeignKeyProperties[i].ForeignDataModelFactory
                    .GetColumnNamesAsCommandText(DataModelForeignKeyProperties[i].GetJoinName()));
            }

            return sbCommandText.ToString();
        }

        // public List<T> DataModels { get; private set; } = new List<T>();
        // public List<IDataModel> DataModels { get; private set; } = new List<IDataModel>();

        public abstract T GenerateAndGetDataModel();

        public abstract void GenerateDataColumnProperties();
        
        public DataModelFactory(string tableName)
        {
            TableName = tableName;
            GenerateDataColumnProperties();
        }

        public IDataModel GenerateAndGetIDataModel(SqliteDataReader reader, ref int index)
        {
            return GenerateAndGetDataModel(reader, ref index);
        }

        public T GenerateAndGetDataModel(SqliteDataReader reader, ref int index)
        {
            T result = GenerateAndGetDataModel();
            result.ReadFromDatabase(reader, ref index);
            return result;
        }

        public IDataModel GenerateDataModelWithOnlyPK(SqliteDataReader reader, ref int index)
        {
            var result = GenerateAndGetDataModel();
            result.ReadPKFromDatabase(reader, ref index);
            return result;
        }

        /*
        public void GenerateDataModel(SqliteDataReader reader, ref int index)
        {
            GenerateAndGetDataModel(reader, ref index);
        }
        */

        public virtual List<T> GeneratePresets()
        {
            return null;
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
            var sbParameters = new StringBuilder();


            sbParameters.Append(DataModelColumnProperties[index].GetParameterName());
            for (int i = index + 1; i < DataModelColumnProperties.Count; i++)
            {
                sbQuery.Append($", {DataModelColumnProperties[i].ColumnName}");
                sbParameters.Append($", {DataModelColumnProperties[i].GetParameterName()}"); //.AppendFormat(",@{0}", DataModelColumnProperties[i].ColumnName);
            }

            for (int i = 0; i < DataModelForeignKeyProperties.Count; i++)
            {
                sbQuery.Append($", {DataModelForeignKeyProperties[i].ColumnName}");
                sbParameters.Append($", {DataModelForeignKeyProperties[i].GetParameterName()}");
            }

            sbQuery.Append(") VALUES (");
            sbQuery.Append(sbParameters.ToString());
            sbQuery.Append(")");

            return sbQuery.ToString();
        }
        public string GetForeignKeyCommandText(DataModel.IDataModelForeignKey fk)
        {
            return $"JOIN {fk.ForeignDataModelFactory.TableName} as {fk.GetJoinName()} ON {TableName}.{fk.ColumnName} = {fk.GetJoinName()}.{fk.ColumnName}";
        }

        /*
        protected virtual string GetCommandText_BasicSelectCommon()
        {
            var sbQuery = new StringBuilder();
            var sbQueryJoin = new StringBuilder();
            
            sbQuery.Append($"SELECT {GetColumnNamesAsCommandText()}");

            for(int i = 0; i < DataModelForeignKeyProperties.Count(); i++)
            {
                sbQueryJoin.AppendLine($" {GetForeignKeyCommandText(DataModelForeignKeyProperties[i])}");
            }

            sbQuery.AppendFormat(" FROM {0}", TableName);
            sbQuery.Append(sbQueryJoin.ToString());
            return sbQuery.ToString();
        }
        */

        public virtual string GetCommandText_Select(List<DataModel.IDataModelColumn> parameters = null,
                                                       int pageSize = 0, int startIndex = 0,
                                                       List<DataModel.IDataModelColumnProperties> orderby = null)
        {
            var sbQuery = new StringBuilder();
            var sbSelect = new StringBuilder();
            

            sbSelect.AppendFormat("SELECT {0}", DataModelColumnProperties[0].ColumnName);
            // Iterate the Factory's Columns(exluding foreign keys) to add them to select query
            for (int i = 1; i < DataModelColumnProperties.Count; i++)
            {
                sbSelect.AppendFormat(", {0}", DataModelColumnProperties[i].ColumnName);
            }

            sbQuery.AppendFormat(" FROM {0}", TableName);

            // Iterate the Factory's Foreign Keys to add them to select query and to join to their
            // respective tables
            for (int i = 0; i < DataModelForeignKeyProperties.Count(); i++)
            {
                if (DataModelForeignKeyProperties[i].PerformJoin) {
                    sbSelect.Append(DataModelForeignKeyProperties[i].GetColumnsForSelectQuery());
                    sbQuery.Append($" JOIN {DataModelForeignKeyProperties[i].GetJoinName()}");
                    sbQuery.Append($" ON {DataModelForeignKeyProperties[i].ColumnName} = {DataModelForeignKeyProperties[i].GetForeignTablePK()}");
                }
                else
                {
                    sbSelect.Append($", {DataModelForeignKeyProperties[i].ColumnName}");
                }
            }

            // If parameters have been provided include WHERE to the SELECT query
            if(parameters != null && parameters.Count > 0){
                sbQuery.AppendLine(" WHERE");
                // Iterate all parameters to add to the WHERE query
                for (int pi = 0; pi < parameters.Count; pi++)
                {
                    if (pi > 0)
                    {
                        sbQuery.Append(" AND");
                    }
                    sbQuery.AppendLine(string.Format(" {0} = @{0}", parameters[pi].GetColumnProperties().ColumnName));
                }
            }

            sbQuery.AppendLine(" ORDER BY");

            // Add Order By
            if (orderby != null)
            {
                for (int i = 0; i < orderby.Count; i++)
                {
                    if (i > 0)
                    {
                        sbQuery.Append(", ");
                    }
                    sbQuery.Append($"{orderby[i].ColumnName}");
                }
            }
            else
            {
                // If Order By not provided, order by primary key 
                sbQuery.AppendLine($"{Get_PrimaryKeyProperties().ColumnName}");
            }

            if (pageSize > 0)
            {
                sbQuery.AppendLine(string.Format(" LIMIT {0}, {1}", pageSize, startIndex));
            }

            return $"{sbSelect.ToString()} {sbQuery.ToString()}";
        }

/*
        public virtual string GetCommandText_BasicSelect(bool selectAll = true, int pageSize = 0, int startIndex = 0)
        {
            var sbQuery = new StringBuilder();
            var sbSelect = new StringBuilder();
            
            sbSelect.AppendFormat("SELECT {0}", DataModelColumnProperties[0].ColumnName);
            for (int i = 1; i < DataModelColumnProperties.Count; i++)
            {
                sbSelect.AppendFormat(", {0}", DataModelColumnProperties[i].ColumnName);
            }

            var primaryKey = DataModelColumnProperties.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendFormat(" FROM {0}", TableName);

            for (int i = 0; i < DataModelForeignKeyProperties.Count(); i++)
            {
                if (DataModelForeignKeyProperties[i].PerformJoin) {
                    sbSelect.Append(DataModelForeignKeyProperties[i].GetColumnsForSelectQuery());
                    sbQuery.Append($" JOIN {DataModelForeignKeyProperties[i].GetJoinName()}");
                    sbQuery.Append($" ON {DataModelForeignKeyProperties[i].ColumnName} = {DataModelForeignKeyProperties[i].GetForeignTablePK()}");
                }
                else
                {
                    sbSelect.Append($", {DataModelForeignKeyProperties[i].ColumnName}");
                }
            }

            if (!selectAll)
            {
                sbQuery.AppendLine(string.Format(" WHERE {0} = @{0}", primaryKey.ColumnName));
            }

            sbQuery.AppendLine(string.Format(" ORDER BY {0}", primaryKey.ColumnName));

            if (pageSize > 0)
            {
                sbQuery.AppendLine(string.Format(" LIMIT {0}, {1}", pageSize, startIndex));
            }

            return $"{sbSelect.ToString()} {sbQuery.ToString()}";
        }

        public virtual string GetCommandText_BasicSelect(List<DataModel.IDataModelColumn> parameters, int pageSize = 0, int startIndex = 0)
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendFormat("SELECT {0}", DataModelColumnProperties[0].ColumnName);
            for (int i = 1; i < DataModelColumnProperties.Count; i++)
            {
                sbQuery.AppendFormat(", {0}", DataModelColumnProperties[i].ColumnName);
            }

            var primaryKey = DataModelColumnProperties.First(dmc => dmc.IsPrimaryKey);
            sbQuery.Append($" FROM {TableName}");
            if(parameters.Count > 0){
                sbQuery.Append(" WHERE");
                for (int pi = 0; pi < parameters.Count; pi++)
                {
                    if (pi > 0)
                    {
                        sbQuery.Append(" AND");
                    }
                    sbQuery.AppendLine(string.Format(" {0} = @{0}", parameters[pi].GetColumnProperties().ColumnName));
                }
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
*/

        public string GetCommandText_Update()
        {
            var sbQuery = new StringBuilder();
            var lstDMC = DataModelColumnProperties.Where(dmc => !dmc.IsPrimaryKey).ToList();
            sbQuery.AppendFormat("UPDATE {0} SET {1} = @{1} ", TableName, lstDMC[0].ColumnName);

            for (int i = 1; i < lstDMC.Count; i++)
            {
                sbQuery.AppendFormat(",{0} = @{0} ", lstDMC[i].ColumnName);
            }
            sbQuery.AppendFormat("WHERE {0} = @{0}", Get_PrimaryKeyProperties().ColumnName);
            return sbQuery.ToString();
        }

        public string GetCommandText_Delete()
        {
            var sbQuery = new StringBuilder();
            var primaryKey = DataModelColumnProperties.First(dmc => dmc.IsPrimaryKey);
            sbQuery.AppendLine($"DELETE FROM {TableName} WHERE {Get_PrimaryKeyProperties().ColumnName} = @{Get_PrimaryKeyProperties().ColumnName} ");

            return sbQuery.ToString();
        }

        public string GetCommandText_Delete(List<DataModel.IDataModelColumnProperties> parameters)
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendFormat($"DELETE FROM {TableName}");
            sbQuery.AppendLine($"WHERE {parameters[0].ColumnName} = @{parameters[0].ColumnName}");
            for (int i = 1; i < parameters.Count; i++)
            {
                sbQuery.Append($" AND {parameters[i].ColumnName} = @{parameters[i].ColumnName}");
            }

            return sbQuery.ToString();
        }

        public string GetCommandText_Delete(SqliteParameterCollection parameters)
        {
            var sbQuery = new StringBuilder();
            sbQuery.AppendLine($"DELETE FROM {TableName}");
            sbQuery.AppendLine($"WHERE {parameters[0].ParameterName} = @{parameters[0].ParameterName} ");
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
        DataModel.IDataModelColumnProperties _primaryKeyProperties = null;
        public DataModel.IDataModelColumnProperties Get_PrimaryKeyProperties()
        {
            if(_primaryKeyProperties == null){
                _primaryKeyProperties = DataModelColumnProperties.FirstOrDefault(dmc => dmc.IsPrimaryKey);
            }

            return _primaryKeyProperties;
        }

        public string GetCommandText_LatestPrimaryKey()
        {
            var sbQuery = new StringBuilder();
            var primaryKey = Get_PrimaryKeyProperties();
            sbQuery.AppendFormat("SELECT {1} FROM {0} ORDER BY {1} DESC LIMIT 1 ", TableName, primaryKey.ColumnName);

            return sbQuery.ToString();
        }

        // Check ownership of datamodel of the following
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
            dataModel.AddParameters_AllColumns(insertSQL, includePK);
            return insertSQL;
        }

        public SqliteCommand GetCommand_Update(T dataModel)
        {
            var query = GetCommandText_Update();
            var updateSQL = new SqliteCommand(query);
            dataModel.AddParameters_AllColumns(updateSQL);
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