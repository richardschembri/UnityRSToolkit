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

        public bool DebugMode { get; set; } = false;
        private string DEBUG_TAG = "SQLite";

        public static string GetDBStreamingFilePath(string databaseName)
        {
            return string.Format("{0}/{1}.s3db", Application.streamingAssetsPath, databaseName);
        }

        public static string GetConnectionPath(string filepath)
        {
            return string.Format("URI=file:{0}", filepath);
        }

        public DatabaseAccess(string databaseName)
        {
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

        public static bool GenerateDBFile(string databaseName)
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
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error generated DB");
                sbError.AppendLine(ex.Message);
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
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error with script 【{0}】", query);
                sbError.AppendLine(ex.Message);
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

                            dbConnection.Close();
                            reader.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error with script 【{0}】", query);
                sbError.AppendLine(ex.Message);
            }
            return tableExists;
        }

        public void AddColumnToTable(string columnName, DataModel dataModel)
        {

            var sbQuery = new StringBuilder();
            var columnToAdd = dataModel.DataModelColumns.First(c => c.ColumnName == columnName);

            sbQuery.AppendLine(string.Format("alter table {0} add column {1}", dataModel.TableName, columnToAdd.GetColumnCodeForCreateTable()));
            if (dataModel.IsForeignKey)
            {
                sbQuery.AppendLine(columnToAdd.GetForeignKeyCodeForCreateTable());
                sbQuery.AppendLine("GO");
            }

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
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error with script 【{0}】", query);
                sbError.AppendLine(ex.Message);
                Debug.LogError(sbError.ToString());
            }
        }
        public void DropTable(DataModel dataModel)
        {
            DropTable(dataModel.TableName);
        }
        public void DropTable(string tableName)
        {
            var sbQuery = new StringBuilder();

            sbQuery.Append(string.Format("drop table if exists {0}", tableName));

            var query = sbQuery.ToString();
            DebugHelpers.LogInDebugMode(DebugMode, DEBUG_TAG, query);
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
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error with script 【{0}】", query);
                sbError.AppendLine(ex.Message);
                Debug.LogError(sbError.ToString());
            }
        }
        public void CreateTableAndPopulateTableIfNotExists(string tableName, DataModel dataModel)
        {
            var sbQuery = new StringBuilder();
            var primaryKeyColumn = dataModel.DataModelColumns.First(pk => pk.IsPrimaryKey);

            sbQuery.Append(string.Format("create table if not exists {0}(", tableName));
            sbQuery.AppendLine(primaryKeyColumn.GetColumnCodeForCreateTable());

            for (int i = 0; i < dataModel.DataModelColumns.Count; i++)
            {
                var dmc = dataModel.DataModelColumns[i];
                if (!dmc.IsPrimaryKey)
                {
                    sbQuery.Append(string.Format(", {0}", dmc.GetColumnCodeForCreateTable()));
                }
            }

            var foreignKeys = dataModel.DataModelColumns.Where(dmc => !string.IsNullOrEmpty(dmc.ForeignTableName)).ToList();
            for (int i = 0; i < foreignKeys.Count; i++)
            {
                sbQuery.AppendLine(foreignKeys[i].GetForeignKeyCodeForCreateTable());
            }
            sbQuery.Append(")");

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
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error with script 【{0}】", query);
                sbError.AppendLine(ex.Message);
                Debug.LogError(sbError.ToString());
            }
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
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error with script 【{0}】", query);
                sbError.AppendLine(ex.Message);
                Debug.LogError(sbError.ToString());
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
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error with script 【{0}】", query);
                sbError.AppendLine(ex.Message);
                Debug.LogError(sbError.ToString());
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
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error with script 【{0}】", query);
                sbError.AppendLine(ex.Message);
                Debug.LogError(sbError.ToString());
            }

            return rowID;
        }


        public void ExecuteCommand_Insert(SqliteCommand insertCommand)
        {
            var lstInsertCommands = new List<SqliteCommand>();
            lstInsertCommands.Add(insertCommand);
            ExecuteCommands_Insert(lstInsertCommands);
        }

        public bool ExecuteCommands_Insert<T>(List<T> lstDataModel)
        {
            if (!lstDataModel.Any())
            {
                return false;
            }

            var sqliteCommands = new List<SqliteCommand>();
            for (int i = 0; i < lstDataModel.Count; i++)
            {
                var dm = lstDataModel[i] as DataModel;
                sqliteCommands.Add(dm.GetCommand_Insert());
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
                        var sbError = new StringBuilder();
                        sbError.AppendFormat("Error with script 【{0}】", query_log);
                        sbError.AppendLine(ex.Message);
                        Debug.LogError(sbError.ToString());
                    }
                }

                dbConnection.Close();
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
                var sbError = new StringBuilder();
                sbError.AppendFormat("Error with script 【{0}】", query);
                sbError.AppendLine(ex.Message);
                Debug.LogError(sbError.ToString());
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
    }
}