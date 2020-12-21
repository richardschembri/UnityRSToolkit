using RSToolkit.Data.SQLite;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Data.SQLite
{
    public class DemoDataManager : DataManager
    {
        DataCountriesFactory _dataCountriesFactory = new DataCountriesFactory();


        public static DemoDataManager GetInstance()
        {
            return (DemoDataManager)Instance;
        }
       
        public override void GenerateTables()
        {
            LogInDebugMode("GenerateTables");
            _dba.CreateAndPopulateTableIfNotExists(_dataCountriesFactory);
        }
        
        public List<DataCountries> BasicSelect_Countries(bool selectAll = true, int pageSize = 0, int startIndex = 0)
        {
            _dba.ExecuteReader_BasicSelect(_dataCountriesFactory, selectAll, pageSize, startIndex);
            return _dataCountriesFactory.DataModels;
        }

    }
}