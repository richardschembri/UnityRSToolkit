using RSToolkit.Data.SQLite;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Data.SQLite
{
    public class DemoDataManager : DataManager
    {
        DataCountriesFactory _dataCountriesFactory;
        DataUsersFactory _dataUsersFactory;

        protected override void Awake()
        {
            _dataCountriesFactory = new DataCountriesFactory();
            _dataUsersFactory = new DataUsersFactory(_dataCountriesFactory);

            base.Awake();
        }

        public static DemoDataManager GetInstance()
        {
            return (DemoDataManager)Instance;
        }
       
        public override void GenerateTables()
        {
            LogInDebugMode("GenerateTables");
            _dba.CreateAndPopulateTableIfNotExists(_dataCountriesFactory);
            _dba.CreateAndPopulateTableIfNotExists(_dataUsersFactory);
        }
        
        public List<DataCountries> Select_Countries(int pageSize = 0, int startIndex = 0)
        {
            return _dba.ExecuteReader_Select(_dataCountriesFactory, null, pageSize, startIndex);
        }

        public List<DataUsers> Select_Users(int pageSize = 0, int startIndex = 0)
        {
            return _dba.ExecuteReader_Select(_dataUsersFactory, null, pageSize, startIndex);
        }

        public bool Insert_User(string firstName, string lastName, DataCountries country)
        {
            return _dba.ExecuteCommands_Insert(_dataUsersFactory, _dataUsersFactory.GenerateDataModel(firstName, lastName, country));
        }
    }
}