using System;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.Data.SQLite
{
    public class DatabaseAccess
    {
        public const string SQLITE_DATEFORMAT = "YYYY-MM-DD HH:MM:SS.SSS";

        public string Connection { get; private set; }
        public string DatabaseName { get; private set; }

        public bool DebugMode { get; set; } = false;
        private string DEBUG_TAG = "SQLite";
        public virtual string GetDebugTag()
        {
            return $"{DEBUG_TAG}[{DatabaseName}]";
        }

        public static string GetDBStreamingFilePath(string databaseName)
        {
            return string.Format("{0}/{1}.s3db", Application.streamingAssetsPath, databaseName);
        }

        public static string GetConnectionPath(string filepath)
        {
            return string.Format("URI=file:{0}", filepath);
        }

        public DatabaseAccess(string databaseName, bool debugMode = false)
        {
            DebugMode = debugMode;
            DatabaseName = databaseName;
            //Connection = string.Format("URI=file:{0}/{1}.s3db", Application.dataPath,  databaseName);
            Connection = GetConnectionPath(GetDBStreamingFilePath(databaseName));
#if UNITY_EDITOR
            return;
#endif
#if UNITY_IOS
        var filepath = string.Format("{0}/{1}.s3db", Application.persistentDataPath, databaseName);

        if (!File.Exists(filepath))
        {
            var loadDb  = GetDBStreamingFilePath(databaseName);
            // then save to Application.persistentDataPath
            File.Copy(loadDb, filepath);
        }

        Connection = string.Format("URI=file:{0}", filepath);
#endif

        }

        protected bool GenerateDBFile(string databaseName)
        {
            try
            {
                var Connection = GetConnectionPath(GetDBStreamingFilePath(databaseName));
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();
                    dbConnection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                LogErrorInDebugMode($"Failed to generate Database File for {databaseName}", ex);               
            }
            return false;
        }


        public bool DoesExist_Column(string columnName, string tableName)
        {

            bool columnExists = false;
            var query = string.Format("SELECT {0} FROM {1} LIMIT 1", columnName, tableName);
            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();

                    using (var dbCmd = dbConnection.CreateCommand())
                    {
                        dbCmd.CommandText = query;
                        using (var reader = dbCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //columnExists = true;
                            }

                            columnExists = true;
                            dbConnection.Close();
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }
            return columnExists;
        }

        public bool DoesExist_Table(string tableName)
        {
            bool tableExists = false;
            var query = string.Format("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{0}'", tableName);
            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();

                    using (var dbCmd = dbConnection.CreateCommand())
                    {
                        dbCmd.CommandText = query;
                        using (var reader = dbCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tableExists = reader.GetInt32(0) > 0;
                            }
                            
                            reader.Close();
                        }                        
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }
            return tableExists;
        }

        // public void AddColumnToTable<T>(string columnName, DataModelFactory<T> dataModelFactory, DataModel dataModel) where T : DataModel
        public bool AddColumnToTable(string columnName, IDataModelFactory dataModelFactory)
        {

            var sbQuery = new StringBuilder();
            var columnToAdd = dataModelFactory.DataModelColumnProperties.First(c => c.ColumnName == columnName);
            if(columnToAdd != null)
            {
                sbQuery.AppendLine(string.Format("alter table {0} add column {1}",
                dataModelFactory.TableName, columnToAdd.GetColumnCodeForCreateTable()));
            }
            else
            {
                //CONTINUE FROM HERE
                //var fkToAdd = dataModelFactory..DataModelColumnProperties.First(c => c.ColumnName == columnName);
            }
            // if (dataModel.IsForeignKey)
            /*
            if (columnToAdd.IsForeignKey())
            {
                sbQuery.AppendLine(columnToAdd.GetForeignKeyCodeForCreateTable());
                sbQuery.AppendLine("GO");
            }
            */

            var query = sbQuery.ToString();
            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();
                    using (var dbCmd = dbConnection.CreateCommand())
                    {
                        dbCmd.CommandText = query;
                        dbCmd.ExecuteScalar();
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }
            return true;
        }

        // public void DropTable<T>(DataModelFactory<T> dataModel) where T : DataModel
        public void DropTable(IDataModelFactory dataModel)
        {
            DropTable(dataModel.TableName);
        }

        public void DropTable(string tableName)
        {
            var sbQuery = new StringBuilder();

            sbQuery.Append(string.Format("drop table if exists {0}", tableName));

            var query = sbQuery.ToString();
            LogInDebugMode(query);
            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();
                    using (var dbCmd = dbConnection.CreateCommand())
                    {
                        dbCmd.CommandText = query;
                        dbCmd.ExecuteScalar();
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }
        }
        // public void CreateTableIfNotExists<T>(DataModelFactory<T> dataModelFactory) where T : DataModel
        public void CreateTableIfNotExists(IDataModelFactory dataModelFactory)
        {
            var sbQuery = new StringBuilder();
            var primaryKeyColumn = dataModelFactory.DataModelColumnProperties.First(pk => pk.IsPrimaryKey);

            sbQuery.Append(string.Format("create table if not exists {0}(", dataModelFactory.TableName));
            sbQuery.AppendLine(primaryKeyColumn.GetColumnCodeForCreateTable());

            for (int i = 0; i < dataModelFactory.DataModelColumnProperties.Count; i++)
            {
                var dmc = dataModelFactory.DataModelColumnProperties[i];
                if (!dmc.IsPrimaryKey)
                {
                    sbQuery.Append(string.Format(", {0}", dmc.GetColumnCodeForCreateTable()));
                }
            }

            //CONTINUE FROM HERE
            /*
            // var foreignKeys = dataModelFactory.DataModelColumnProperties.Where(dmc => !string.IsNullOrEmpty(dmc.ForeignTableName)).ToList();
            var foreignKeys = dataModelFactory.DataModelColumnProperties.Where(dmc => dmc.IsForeignKey()).ToList();
            for (int i = 0; i < foreignKeys.Count; i++)
            {
                sbQuery.AppendLine(foreignKeys[i].GetForeignKeyCodeForCreateTable());
            }
            sbQuery.Append(")");
*/
            var query = sbQuery.ToString();
            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();
                    using (var dbCmd = dbConnection.CreateCommand())
                    {
                        dbCmd.CommandText = query;
                        dbCmd.ExecuteScalar();
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }
        }

        public bool CreateAndPopulateTableIfNotExists<T>(DataModelFactory<T> dataModelFactory, List<T> dataModels) where T : DataModel
        // public bool CreateAndPopulateTableIfNotExists(IDataModelFactory dataModelFactory, List<IDataModel> dataModels)
        {
            if (DoesExist_Table(dataModelFactory.TableName))
            {
                return false;
            }

            CreateTableIfNotExists(dataModelFactory);

            return ExecuteCommands_Insert(dataModelFactory, dataModels);
        }

        public bool CreateAndPopulateTableIfNotExists<T>(DataModelFactory<T> dataModelFactory) where T : DataModel
        {
            dataModelFactory.GeneratePresets();
            LogInDebugMode($"Generated {dataModelFactory.DataModels.Count} Presets");
            return CreateAndPopulateTableIfNotExists(dataModelFactory, dataModelFactory.DataModels);
        }

        public void ExecuteCommand_Update(SqliteCommand updateCommand)
        {
            var query = string.Empty;
            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();
                    using (updateCommand)
                    {
                        query = updateCommand.CommandText;
                        updateCommand.Connection = dbConnection;
                        updateCommand.ExecuteScalar();
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }
        }

        public void ExecuteCommand_Delete(SqliteCommand deleteCommand)
        {
            var query = string.Empty;
            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();
                    using (deleteCommand)
                    {
                        query = deleteCommand.CommandText;
                        DebugHelpers.LogInDebugMode(DebugMode, DEBUG_TAG, query);
                        deleteCommand.Connection = dbConnection;
                        deleteCommand.ExecuteNonQuery();//ExecuteScalar();
                    }
                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }
        }

        public object ExecuteCommand_GetLatestPrimaryKey(SqliteCommand cmd)
        {

            var query = string.Empty;
            object rowID = null;
            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();
                    using (cmd)
                    {
                        query = cmd.CommandText;
                        cmd.Connection = dbConnection;
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rowID = reader.GetValue(0);
                            }
                            reader.Close();
                        }
                        dbConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }

            return rowID;
        }


        public void ExecuteCommand_Insert(SqliteCommand insertCommand)
        {
            var lstInsertCommands = new List<SqliteCommand>();
            lstInsertCommands.Add(insertCommand);
            ExecuteCommands_Insert(lstInsertCommands);
        }

        public void ExecuteCommand_Insert<T>(DataModelFactory<T> dataModelFactory, T dataModel) where T : DataModel
        {
            ExecuteCommand_Insert(dataModelFactory.GetCommand_Insert(dataModel));
        }

        public bool ExecuteCommands_Insert<T>(DataModelFactory<T> dataModelFactory, List<T> dataModels) where T : DataModel
        {
            if (!dataModels.Any())
            {
                return false;
            }

            var sqliteCommands = new List<SqliteCommand>();
            for (int i = 0; i < dataModels.Count; i++)
            {
                sqliteCommands.Add(dataModelFactory.GetCommand_Insert(dataModels[i] as T));
            }

            ExecuteCommands_Insert(sqliteCommands);
            return true;
        }

        public void ExecuteCommands_Insert(List<SqliteCommand> lstInsertCommands)
        {
            var query_log = string.Empty;
            using (var dbConnection = new SqliteConnection(Connection))
            {
                dbConnection.Open();
                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < lstInsertCommands.Count; i++)
                        {
                            using (lstInsertCommands[i])
                            {
                                if (i == 0)
                                {
                                    query_log = string.Format("[{0} x Rows] {1}", lstInsertCommands.Count, lstInsertCommands[i].CommandText);
                                    DebugHelpers.LogInDebugMode(DebugMode, DEBUG_TAG, query_log);
                                }
                                lstInsertCommands[i].Connection = dbConnection;
                                lstInsertCommands[i].ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogSQLQueryErrorInDebugMode(query_log, ex);
                    }
                }

                dbConnection.Close();
            }
        }

        public void ExecuteReader_BasicSelect<T>(DataModelFactory<T> modelFactory, bool selectAll = true, int pageSize = 0, int startIndex = 0) where T : DataModel
        {
            modelFactory.DataModels.Clear();

            var query = modelFactory.GetCommandText_BasicSelect(selectAll, pageSize, startIndex);

            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();

                    using (var dbCmd = dbConnection.CreateCommand())
                    {
                        dbCmd.CommandText = query;

                        using (var reader = dbCmd.ExecuteReader())
                        {
                            int index; 
                            while (reader.Read())
                            {
                                index = 0;
                                modelFactory.GenerateAndGetDataModel(reader, ref index);
                            }
                            reader.Close();
                        }
                    }

                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }
        }

        public virtual void GetCommandText_BasicSelectWithParameters<T>(DataModelFactory<T> modelFactory, List<DataModel.IDataModelColumn> parameters, int pageSize = 0, int startIndex = 0) where T : DataModel
        {

            var query = modelFactory.GetCommandText_BasicSelectWithParameters(parameters, pageSize, startIndex);

            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    dbConnection.Open();

                    using (var dbCmd = dbConnection.CreateCommand())
                    {
                        dbCmd.CommandText = query;
                        for(int i = 0; i < parameters.Count; i++)
                        {
                            dbCmd.Parameters.Add(parameters[i].ToParameter());
                        }

                        using (var reader = dbCmd.ExecuteReader())
                        {
                            int index = 0;
                            while (reader.Read())
                            {
                                modelFactory.GenerateAndGetDataModel(reader, ref index);
                            }
                            reader.Close();
                        }
                    }

                    dbConnection.Close();
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }
        }

        public int Get_RowCount(string tableName)
        {

            int rowCount = 0;
            var query = string.Format("SELECT COUNT(*) FROM {0}", tableName);
            try
            {
                using (var dbConnection = new SqliteConnection(Connection))
                {
                    using (var dbCmd = dbConnection.CreateCommand())
                    {
                        dbConnection.Open();
                        dbCmd.CommandText = query;
                        using (var reader = dbCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rowCount = reader.GetInt32(0);
                            }

                            dbConnection.Close();
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSQLQueryErrorInDebugMode(query, ex);
            }

            return rowCount;
        }

        public bool IsTableEmpty(string tableName)
        {
            return !(Get_RowCount(tableName) > 0);
        }

        public bool DoesTableNotExistOrIsEmpty(string tableName)
        {
            return !DoesExist_Table(tableName) || IsTableEmpty(tableName);
        }

        protected void LogInDebugMode(string message, bool includeTimestamp = false)
        {
            DebugHelpers.LogInDebugMode(DebugMode, GetDebugTag(), message, includeTimestamp);
        }

        protected void LogErrorInDebugMode(string message, bool includeTimestamp = false)
        {
            DebugHelpers.LogErrorInDebugMode(DebugMode, GetDebugTag(), message, includeTimestamp);
        }

        protected void LogErrorInDebugMode(object message, Exception ex, bool includeTimestamp = false)
        {
            DebugHelpers.LogErrorInDebugMode(DebugMode, GetDebugTag(), message, ex, includeTimestamp);
        }


        protected void LogSQLQueryErrorInDebugMode(string query, Exception ex, bool includeTimestamp = false)
        {
            LogErrorInDebugMode($"Error with script 【{query}】", ex, includeTimestamp);
        }

    }
}