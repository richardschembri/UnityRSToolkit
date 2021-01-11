using RSToolkit.Data.SQLite;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Data.SQLite
{
    public class DemoDataManager : DataManager
    {
        DataCountriesFactory _dataCountriesFactory = new DataCountriesFactory();
        DataUsersFactory _dataUsersFactory = new DataUsersFactory();
        


        public static DemoDataManager GetInstance()
        {
            return (DemoDataManager)Instance;
        }
       
        public override void GenerateTables()
        {
            LogInDebugMode("GenerateTables");
            _dba.CreateAndPopulateTableIfNotExists(_dataCountriesFactory);
        }
        
        public List<DataCountries> Select_Countries(int pageSize = 0, int startIndex = 0)
        {
            return _dba.ExecuteReader_Select(_dataCountriesFactory, null, pageSize, startIndex);
        }

        public List<DataUsers> Select_Users(int pageSize = 0, int startIndex = 0)
        {
            return _dba.ExecuteReader_Select(_dataUsersFactory, null, pageSize, startIndex);
        }
    }
}