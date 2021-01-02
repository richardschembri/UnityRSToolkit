using Mono.Data.Sqlite;
using RSToolkit.Data.SQLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.Data.SQLite
{

    #region Countries

    public class DataCountries : DataModel
    {
        
        public const string TABLE_NAME = "Countries";

        public DataModelColumn<int> ColumnCountryID { get; private set;}
        public DataModelColumn<string> ColumnCountryName { get; set; }

        public DataCountries() : base()
        {

        }

        public DataCountries(DataModelColumnProperties<int> countryIDProperties,
                                DataModelColumnProperties<string> countryNameProperties)
        {
            ColumnCountryID = new DataModelColumn<int>(countryIDProperties);
            DataModelColumns.Add(ColumnCountryID);
            ColumnCountryName = new DataModelColumn<string>(countryNameProperties);
            DataModelColumns.Add(ColumnCountryName);

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
            var result = new DataCountries(ColumnPropertiesColumnCountryID, countryID,
                                               ColumnPropertiesColumnCountryName, countryName);
            DataModels.Add(result);

            return result;
        }

        public override DataCountries GenerateDataModel()
        {
            var result = new DataCountries(ColumnPropertiesColumnCountryID,
                                               ColumnPropertiesColumnCountryName);
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
    #endregion Countries


    #region Users

    public class DataUsers : DataModel
    {
        
        public const string TABLE_NAME = "Users";

        public DataModelColumn<int> ColumnUserID { get; private set;}
        public DataModelColumn<string> ColumnFirstName { get; set; }
        public DataModelColumn<string> ColumnLastName { get; set; }
        public DataModelColumn<int> ColumnCountry { get; set; }

        private void Init(DataModelColumnProperties<int> columnUserIDProperties,
                                DataModelColumnProperties<string> columnFirstNameProperties,
                                DataModelColumnProperties<string> columnLastNameProperties,
                                DataModelColumnProperties<int> columnCountryProperties)
        {
            ColumnUserID  = new DataModelColumn<int>(columnUserIDProperties);
            DataModelColumns.Add(ColumnUserID);
            ColumnFirstName  = new DataModelColumn<string>(columnFirstNameProperties);
            DataModelColumns.Add(ColumnFirstName);
            ColumnLastName = new DataModelColumn<string>(columnLastNameProperties);
            DataModelColumns.Add(ColumnFirstName);
            ColumnCountry = new DataModelColumn<int>(columnCountryProperties);
            DataModelColumns.Add(ColumnCountry);
        }

        public DataUsers (DataModelColumnProperties<int> columnUserIDProperties,
                                DataModelColumnProperties<string> columnFirstNameProperties,
                                DataModelColumnProperties<string> columnLastNameProperties,
                                DataModelColumnProperties<int> columnCountryProperties)
        {
            Init(columnUserIDProperties, columnFirstNameProperties, columnLastNameProperties, columnCountryProperties);

        }

        /*
        public DataCountries(DataModelColumnProperties<int> columnUserIDProperties, int countryID,
                                DataModelColumnProperties<string> columnFirstNameProperties, string firstName,
                                DataModelColumnProperties<string> columnLastNameProperties, string lastName,
                                DataModelColumnProperties<int> columnCountryProperties, DataCountriesFactory countriesFactory)
        {

            Init(columnUserIDProperties, columnFirstNameProperties, columnLastNameProperties, columnCountryProperties);
            ColumnFirstName.ColumnValue = firstName;
            ColumnLastName.ColumnValue = lastName;
            ColumnCountry.ColumnValue = countriesFactory.DataModels[0].ColumnCountryID.;
        }
        */
    }

    #endregion Users
}