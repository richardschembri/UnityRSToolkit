using RSToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.Data.SQLite
{
    [DisallowMultipleComponent]
    public abstract class DataManager : SingletonMonoBehaviour<DataManager>
    {
        protected DatabaseAccess _dba { get; set; }

        [SerializeField]
        private string _databaseName = "SQLiteDB";

        public UnityEvent OnInitDatabase;       

        public abstract void GenerateTables();

        protected virtual void ModifyTables()
        {

        }

        protected virtual void GenerateData()
        {

        }

        protected virtual void CleanData()
        {

        }

        protected virtual void InitDatabase()
        {
            _dba = new DatabaseAccess(_databaseName);
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