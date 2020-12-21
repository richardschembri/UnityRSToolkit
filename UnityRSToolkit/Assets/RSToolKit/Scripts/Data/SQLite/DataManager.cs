using RSToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.Data.SQLite
{
    [DisallowMultipleComponent]
    public abstract class DataManager : RSSingletonMonoBehaviour<DataManager>
    {
        protected DatabaseAccess _dba { get; set; }

        [SerializeField]
        private string _databaseName = "SQLiteDB";

        public override string GetDebugTag()
        {
            return $"DataManger[{_databaseName}]";
        }

        public UnityEvent OnInitDatabase;       

        public abstract void GenerateTables();

        protected virtual void ModifyTables()
        {
            LogInDebugMode("ModifyTables");
        }

        protected virtual void GenerateData()
        {
            LogInDebugMode("GenerateData");
        }

        protected virtual void CleanData()
        {
            LogInDebugMode("CleanData");
        }

        protected virtual void InitDatabase()
        {
            _dba = new DatabaseAccess(_databaseName, DebugMode);
            GenerateTables();
            ModifyTables();
            GenerateData();
            CleanData();
            OnInitDatabase?.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();
            InitDatabase();
        }
    }
}