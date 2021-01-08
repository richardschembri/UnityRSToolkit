using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RSToolkit.Data.SQLite
{
    public interface IDataModel
    {

        #region Columns
        List<DataModel.IDataModelColumn> DataModelColumns { get; }
        List<DataModel.IDataModelForeignKeyProperties> DataModelForeignKeyProperties { get; }
        // List<DataModel.IDataModelColumn> Get_ForeignKeys();
        List<DataModel.IDataModelColumn> Get_DataModelColumnsByName(string[] columnNames);

        DataModel.IDataModelColumn Get_PrimaryKey();
        SqliteCommand AddParameters_AllColumns(SqliteCommand cmd, bool includePrimaryKey = true);
        #endregion Columns
        void AddParameter_PrimaryKey(ref SqliteCommand cmd);
        CSVDataModel Get_CSVRawDataModel();
        void ReadFromDatabase(SqliteDataReader reader, ref int index);
        void ReadPKFromDatabase(SqliteDataReader reader, ref int index);
    }
    public class DataModel : IDataModel
    {
        /*
        public static Dictionary<Type, DbType> _typeMap;

        public static Dictionary<Type, DbType> TypeMap
        {
            get
            {
                if (_typeMap == null)
                {
                    InitTypeMap();
                }

                return _typeMap;
            }
        }

        private static void InitTypeMap()
        {
            _typeMap = new Dictionary<Type, DbType>();
            _typeMap[typeof(byte)] = DbType.Byte;
            _typeMap[typeof(sbyte)] = DbType.SByte;
            _typeMap[typeof(short)] = DbType.Int16;
            _typeMap[typeof(ushort)] = DbType.UInt16;
            _typeMap[typeof(int)] = DbType.Int32;
            _typeMap[typeof(uint)] = DbType.UInt32;
            _typeMap[typeof(long)] = DbType.Int64;
            _typeMap[typeof(ulong)] = DbType.UInt64;
            _typeMap[typeof(float)] = DbType.Single;
            _typeMap[typeof(double)] = DbType.Double;
            _typeMap[typeof(decimal)] = DbType.Decimal;
            _typeMap[typeof(bool)] = DbType.Boolean;
            _typeMap[typeof(string)] = DbType.String;
            _typeMap[typeof(char)] = DbType.StringFixedLength;
            _typeMap[typeof(Guid)] = DbType.Guid;
            _typeMap[typeof(DateTime)] = DbType.DateTime;
            _typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            _typeMap[typeof(byte[])] = DbType.Binary;
            _typeMap[typeof(byte?)] = DbType.Byte;
            _typeMap[typeof(sbyte?)] = DbType.SByte;
            _typeMap[typeof(short?)] = DbType.Int16;
            _typeMap[typeof(ushort?)] = DbType.UInt16;
            _typeMap[typeof(int?)] = DbType.Int32;
            _typeMap[typeof(uint?)] = DbType.UInt32;
            _typeMap[typeof(long?)] = DbType.Int64;
            _typeMap[typeof(ulong?)] = DbType.UInt64;
            _typeMap[typeof(float?)] = DbType.Single;
            _typeMap[typeof(double?)] = DbType.Double;
            _typeMap[typeof(decimal?)] = DbType.Decimal;
            _typeMap[typeof(bool?)] = DbType.Boolean;
            _typeMap[typeof(char?)] = DbType.StringFixedLength;
            _typeMap[typeof(Guid?)] = DbType.Guid;
            _typeMap[typeof(DateTime?)] = DbType.DateTime;
            _typeMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            // _typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
        }
        */

        public interface IDataModelColumnProperties
        {
            string ColumnName { get; set; }
            string ColumnType { get; }
            bool IsUnique { get; set; }
            object DefaultValue { get; set; }
            bool IsPrimaryKey { get; set; }
            bool IsNullable { get; set; }
            bool IsAutoIncrement { get; set; }
            // string ForeignTableName { get; set; }
            // IDataModelFactory ForeignTable { get; set; }

            // string GetForeignKeyAsTableName();
            // bool IsForeignKey();
            string GetColumnCodeForCreateTable();
            string GetParameterName();
            // string GetForeignKeyCodeForCreateTable();
            // string GetForeignKeyCode();
        }
        
        public class DataModelColumnProperties<T> : IDataModelColumnProperties
        {
            private const string DT_INTEGER = "INTEGER";
            private const string DT_REAL = "REAL";
            private const string DT_TEXT = "TEXT";
            private const string DT_BLOB = "BLOB";

            public static Dictionary<Type, string> _typeMap;

            public static Dictionary<Type, string> TypeMap
            {
                get
                {
                    if (_typeMap == null)
                    {
                        InitTypeMap();
                    }

                    return _typeMap;
                }
            }

            private static void InitTypeMap()
            {
                _typeMap = new Dictionary<Type, string>();
                _typeMap[typeof(byte)] = DT_INTEGER;
                _typeMap[typeof(sbyte)] = DT_INTEGER;
                _typeMap[typeof(short)] = DT_INTEGER;
                _typeMap[typeof(ushort)] = DT_INTEGER;
                _typeMap[typeof(int)] = DT_INTEGER;
                _typeMap[typeof(uint)] = DT_INTEGER;
                _typeMap[typeof(long)] = DT_INTEGER;
                _typeMap[typeof(ulong)] = DT_INTEGER;
                _typeMap[typeof(float)] = DT_REAL;
                _typeMap[typeof(double)] = DT_REAL;
                _typeMap[typeof(decimal)] = DT_REAL;
                _typeMap[typeof(bool)] = DT_INTEGER;
                _typeMap[typeof(string)] = DT_TEXT;
                _typeMap[typeof(char)] = DT_TEXT;
                _typeMap[typeof(Guid)] = DT_TEXT;
                _typeMap[typeof(DateTime)] = DT_TEXT;
                _typeMap[typeof(DateTimeOffset)] = DT_TEXT;
                _typeMap[typeof(byte[])] = DT_BLOB;
                _typeMap[typeof(byte?)] = DT_INTEGER;
                _typeMap[typeof(sbyte?)] = DT_INTEGER;
                _typeMap[typeof(short?)] = DT_INTEGER;
                _typeMap[typeof(ushort?)] = DT_INTEGER;
                _typeMap[typeof(int?)] = DT_INTEGER;
                _typeMap[typeof(uint?)] = DT_INTEGER;
                _typeMap[typeof(long?)] = DT_INTEGER;
                _typeMap[typeof(ulong?)] = DT_INTEGER;
                _typeMap[typeof(float?)] = DT_INTEGER;
                _typeMap[typeof(double?)] = DT_REAL;
                _typeMap[typeof(decimal?)] = DT_REAL;
                _typeMap[typeof(bool?)] = DT_REAL;
                _typeMap[typeof(char?)] = DT_TEXT;
                _typeMap[typeof(Guid?)] = DT_TEXT;
                _typeMap[typeof(DateTime?)] = DT_TEXT;
                _typeMap[typeof(DateTimeOffset?)] = DT_TEXT;
                // _typeMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
            }

            public string ColumnName { get; set; }
            public string ColumnType { get; private set; }
            public string ColumnSqliteType { get; private set; }
            public bool IsUnique { get; set; }
            public object DefaultValue { get; set; }
            public bool IsPrimaryKey { get; set; }
            public bool IsNullable { get; set; }
            public bool IsAutoIncrement { get; set; }
            // public string ForeignTableName { get; set; }
            public IDataModelFactory ForeignTable { get; set; }

            public string GetForeignKeyAsTableName()
            {
                if (IsForeignKey())
                {
                    
                    return $"{ColumnName}_TABLE";
                }
                return null;
            }

            public DataModelColumnProperties(string columnName, bool isUnique = false, bool isPrimaryKey = false, bool isNullable = true, 
                                                IDataModelFactory foreignTable = null, bool isAutoIncrement = false, object defaultValue = null)
            {
                this.ColumnName = columnName;
                this.ColumnType = TypeMap[typeof(T)];
                this.IsUnique = isUnique;
                this.IsPrimaryKey = isPrimaryKey;
                this.IsNullable = isNullable;
                this.IsAutoIncrement = isAutoIncrement;
                // this.ForeignTableName = foreignTableName;
                this.ForeignTable = foreignTable;
                this.DefaultValue = defaultValue;
            }

            public bool IsForeignKey()
            {
                // return !string.IsNullOrEmpty(this.ForeignTableName);

                return this.ForeignTable != null;
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


        }

        public interface IDataModelForeignKeyProperties
        {
            string ColumnName { get; }
            IDataModelFactory ForeignDataModelFactory { get; }
            IDataModelForeignKeyProperties ParentForeignKeyProperties { get; }
            
            bool PerformJoin { get; }

            string GetParameterName();
            string GetForeignTablePK();
            string GetColumnsForSelectQuery();
            string GetForeignKeyCodeForCreateTable();
            string GetCreateForeignKeyCode();
            string GetJoinName();
        }

        public class DataModeForeignKeyProperties : IDataModelForeignKeyProperties
        {
            public string ColumnName { get; private set; }
            public IDataModelFactory ForeignDataModelFactory { get; private set; }
            public IDataModelForeignKeyProperties ParentForeignKeyProperties { get; private set; }
            public bool PerformJoin { get; private set; }

            public DataModeForeignKeyProperties( IDataModelFactory foreignDataModelFactory, bool performJoin,
                                                    IDataModelForeignKeyProperties parentForeignKeyProperties = null,
                                                    string columnName = "")
            {
                if (string.IsNullOrEmpty(columnName))
                {
                    ColumnName = $"{foreignDataModelFactory.TableName}_FK";
                }
                else
                {
                    ColumnName = columnName;
                }
                ForeignDataModelFactory = foreignDataModelFactory;
                ParentForeignKeyProperties = parentForeignKeyProperties  ;
                PerformJoin = performJoin;
            }

            public string GetParameterName()
            {
                return string.Format("@{0}", ColumnName);
            }

            public string GetColumnsForSelectQuery()
            {
                var result = new StringBuilder();
                for(int i = 0; i < ForeignDataModelFactory.DataModelColumnProperties.Count; i++)
                {
                    result.Append($", {GetJoinName()}.{ForeignDataModelFactory.DataModelColumnProperties[i].ColumnName}"); 
                }

                return result.ToString();
            }

            public string GetForeignTablePK()
            {
                return $"{GetJoinName()}.{ForeignDataModelFactory.Get_PrimaryKeyProperties().ColumnName}";
            }
            public string GetForeignKeyCodeForCreateTable()
            {
                return $", {GetCreateForeignKeyCode()}";
            }

            public string GetCreateForeignKeyCode()
            {
                return $"FOREIGN KEY ({ColumnName}) REFERENCES {ForeignDataModelFactory.TableName}({ColumnName})";
            }

            public string GetJoinName()
            {
                if(ParentForeignKeyProperties  != null)
                {
                    return $"{ParentForeignKeyProperties.GetJoinName()}_{ForeignDataModelFactory.TableName}";
                }
                return $"{ForeignDataModelFactory.TableName}";
            }

        }
        public interface IDataModelColumn
        {
            void ReadFromDatabase(SqliteDataReader reader, ref int index);
            object GetValue();
            SqliteParameter ToParameter();
            IDataModelColumnProperties GetColumnProperties();
            object GetColumnValue();
        }

        public class DataModelColumn<T> : IDataModelColumn
        {
            public DataModelColumnProperties<T> ColumnProperties { get; private set; }

            public T ColumnValue { get; set; }

            public DataModelColumn(DataModelColumnProperties<T> columnProperties)
            {
                ColumnProperties = columnProperties;
            }

            public DataModelColumn(DataModelColumnProperties<T> columnProperties, T columnValue)
            {
                ColumnProperties = columnProperties;
                ColumnValue = columnValue;
            }

            public void ReadFromDatabase(SqliteDataReader reader, ref int index)
            {
                // ColumnValue = reader.GetValue(index);
                ColumnValue = (T)Convert.ChangeType(reader.GetValue(index), typeof(T));
                index++;
            }

            public IDataModelColumnProperties GetColumnProperties()
            {
                return ColumnProperties;
            }

            public object GetColumnValue()
            {
                return ColumnValue;
            }

            public SqliteParameter ToParameter()
            {
                return new SqliteParameter(ColumnProperties.GetParameterName(), ColumnValue);
            }

            public object GetValue()
            {
                return ColumnValue;
            }
        }

        
        //public Dictionary<string, string> TableColumnAndType{get; internal set;}
        #region Columns
        public List<IDataModelColumn> DataModelColumns { get; protected set; } = new List<IDataModelColumn>();
        // public List<IDataModelFactory> ForeignKeys { get; protected set; } = new List<IDataModelFactory>();

        public List<IDataModelForeignKeyProperties> DataModelForeignKeyProperties => throw new NotImplementedException();

        /*
        public List<IDataModelColumn> Get_ForeignKeys()
        {
            return DataModelColumns.Where(dc => dc.GetColumnProperties().IsForeignKey()).ToList();
        }
        */
        public List<IDataModelColumn> Get_DataModelColumnsByName(string[] columnNames)
        {
            var enm_dmc = DataModelColumns.Where(dmc => columnNames.Contains(dmc.GetColumnProperties().ColumnName));
            if (enm_dmc != null)
            {
                return enm_dmc.ToList();
            }
            return new List<IDataModelColumn>();
        }

        public IDataModelColumn Get_PrimaryKey()
        {
            return DataModelColumns.FirstOrDefault(dmc => dmc.GetColumnProperties().IsPrimaryKey);
        }
        #endregion Columns
        // public bool IsForeignKey { get; set; }

        public DataModel()
        {
            // IsForeignKey = false;
            
        }

        public virtual SqliteCommand AddParameters_AllColumns(SqliteCommand cmd, bool includePrimaryKey = true)
        {
            for(int i = 0; i < DataModelColumns.Count; i++)
            {
                if(!DataModelColumns[i].GetColumnProperties().IsPrimaryKey || (DataModelColumns[i].GetColumnProperties().IsPrimaryKey && includePrimaryKey))
                {
                    // cmd.Parameters.AddWithValue($"@{DataModelColumns[i].ColumnName}", DataModelColumns[i].GetValue());
                    cmd.Parameters.Add(DataModelColumns[i].ToParameter());
                }                
            }

            return cmd;
        }

        public virtual void AddParameter_PrimaryKey(ref SqliteCommand cmd)
        {
            for(int i = 0; i < DataModelColumns.Count; i++)
            {
                if (DataModelColumns[i].GetColumnProperties().IsPrimaryKey)
                {
                    cmd.Parameters.Add(DataModelColumns[i].ToParameter());
                    return; 
                }
            }
        }

        public virtual CSVDataModel Get_CSVRawDataModel()
        {
            throw new NotImplementedException("GetCSVRawDataModel");
        }

        public virtual void ReadFromDatabase(SqliteDataReader reader, ref int index)
        {            
            for(int i = 0; i < DataModelColumns.Count; i++)
            {
                DataModelColumns[i].ReadFromDatabase(reader, ref index);
            }

            for(int i = 0; i < DataModelForeignKeyProperties.Count; i++)
            {
                if (DataModelForeignKeyProperties[i].PerformJoin)
                {
                    DataModelForeignKeyProperties[i].ForeignDataModelFactory.GenerateDataModel(reader, ref index);
                }
            }
        }

        public void ReadPKFromDatabase(SqliteDataReader reader, ref int index)
        {            
            for(int i = 0; i < DataModelColumns.Count; i++)
            {
                if (DataModelColumns[i].GetColumnProperties().IsPrimaryKey)
                {
                    DataModelColumns[i].ReadFromDatabase(reader, ref index);
                    return;
                }
            }
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