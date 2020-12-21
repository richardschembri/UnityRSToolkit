using Mono.Data.Sqlite;
using RSToolkit.Data.SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Data.SQLite
{
    public class DataCountriesFactory : DataModelFactory<DataCountries>
    {
        public DataModel.DataModelColumnProperties<int> ColumnPropertiesColumnCountryID { get; private set; } = new DataModel.DataModelColumnProperties<int>("CountryID", true, true, false);
        public DataModel.DataModelColumnProperties<string> ColumnPropertiesColumnCountryName { get; set; } = new DataModel.DataModelColumnProperties<string>("CountryName", true, false, false);

        public override void GenerateDataColumnProperties()
        {
            DataModelColumnProperties.Clear();
            DataModelColumnProperties.Add(ColumnPropertiesColumnCountryID);
            DataModelColumnProperties.Add(ColumnPropertiesColumnCountryName);
        }

        public DataCountries GenerateDataModel(int countryID, string countryName)
        {
            var newModel = new DataCountries(ColumnPropertiesColumnCountryID, countryID,
                                               ColumnPropertiesColumnCountryName, countryName);
            DataModels.Add(newModel);

            return newModel;
        }

        public override DataCountries GenerateDataModel()
        {
            var result = new DataCountries();
            DataModels.Add(result);
            return result;
        }

        public override void GeneratePresets()
        {     
            GenerateDataModel(0, "Japan");
            GenerateDataModel(1, "Korea");
            GenerateDataModel(2, "Thailand");
            GenerateDataModel(3, "Vietnam");
            GenerateDataModel(4, "Napal");

        }

        public DataCountriesFactory() : base("Countries")
        {

        }
    }

    public class DataCountries : DataModel
    {
        
        public const string TABLE_NAME = "Countries";

        public DataModelColumn<int> ColumnCountryID { get; private set;}
        public DataModelColumn<string> ColumnCountryName { get; set; }

        public DataCountries() : base()
        {

        }

        public DataCountries(DataModelColumnProperties<int> countryIDProperties, int countryID,
                                DataModelColumnProperties<string> countryNameProperties, string countryName)
        {
            ColumnCountryID = new DataModelColumn<int>(countryIDProperties, countryID);
            DataModelColumns.Add(ColumnCountryID);
            ColumnCountryName = new DataModelColumn<string>(countryNameProperties, countryName);
            DataModelColumns.Add(ColumnCountryName);

        }
    }
}