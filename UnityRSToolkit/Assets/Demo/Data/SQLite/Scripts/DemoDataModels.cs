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
        public DataModel.DataModelColumnProperties<int> ColumnPropertiesCountryID { get; private set; } = new DataModel.DataModelColumnProperties<int>("CountryID", true, true, false);
        public DataModel.DataModelColumnProperties<string> ColumnPropertiesCountryName { get; set; } = new DataModel.DataModelColumnProperties<string>("CountryName", true, false, false);

        public override void GenerateDataColumnProperties()
        {
            DataModelColumnProperties.Clear();
            DataModelColumnProperties.Add(ColumnPropertiesCountryID);
            DataModelColumnProperties.Add(ColumnPropertiesCountryName);
        }

        public DataCountries GenerateAndGetDataModel(int countryID, string countryName)
        {
            var result = new DataCountries(ColumnPropertiesCountryID, countryID,
                                               ColumnPropertiesCountryName, countryName);
            //DataModels.Add(result);

            return result;
        }

        public override DataCountries GenerateAndGetDataModel()
        {
            var result = new DataCountries(ColumnPropertiesCountryID,
                                               ColumnPropertiesCountryName);
            //DataModels.Add(result);
            return result;
        }

        public override List<DataCountries> GeneratePresets()
        {     
            var result = new List<DataCountries>();
            result.Add(GenerateAndGetDataModel(0, "Japan"));
            result.Add(GenerateAndGetDataModel(1, "Korea"));
            result.Add(GenerateAndGetDataModel(2, "Thailand"));
            result.Add(GenerateAndGetDataModel(3, "Vietnam"));
            result.Add(GenerateAndGetDataModel(4, "Napal"));

            return result;
        }
        public override string GetCommandText_Select(List<DataModel.IDataModelColumn> parameters = null, int pageSize = 0, int startIndex = 0, List<DataModel.IDataModelColumnProperties> orderby = null)
        {

            return base.GetCommandText_Select(parameters, pageSize, startIndex, orderby);
        }

        public DataCountriesFactory() : base(DataCountries.TABLE_NAME)
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
        private DataModeForeignKey FKCountry { get; set; }
        public DataCountries Country {
            get { return (DataCountries)FKCountry.ColumnValue; }
            private set { FKCountry.ColumnValue = value; }
        }

        private void Init(DataModelColumnProperties<int> columnUserIDProperties,
                                DataModelColumnProperties<string> columnFirstNameProperties,
                                DataModelColumnProperties<string> columnLastNameProperties,
                                DataModeForeignKey fkCountry)
        {
            ColumnUserID  = new DataModelColumn<int>(columnUserIDProperties);
            DataModelColumns.Add(ColumnUserID);
            ColumnFirstName  = new DataModelColumn<string>(columnFirstNameProperties);
            DataModelColumns.Add(ColumnFirstName);
            ColumnLastName = new DataModelColumn<string>(columnLastNameProperties);
            DataModelColumns.Add(ColumnLastName);
            FKCountry  = fkCountry;
            DataModelForeignKeys.Add(FKCountry);
        }

        public DataUsers (DataModelColumnProperties<int> columnUserIDProperties,
                                DataModelColumnProperties<string> columnFirstNameProperties,
                                DataModelColumnProperties<string> columnLastNameProperties,
                                DataModeForeignKey fkCountryProperties)
        {
            Init(columnUserIDProperties, columnFirstNameProperties, columnLastNameProperties, fkCountryProperties);

        }

        public DataUsers (DataModelColumnProperties<int> columnUserIDProperties,
                                DataModelColumnProperties<string> columnFirstNameProperties, string firstName,
                                DataModelColumnProperties<string> columnLastNameProperties, string lastName,
                                DataModeForeignKey fkCountry, DataCountries country)
        {
            Init(columnUserIDProperties, columnFirstNameProperties, columnLastNameProperties, fkCountry);
            ColumnFirstName.ColumnValue = firstName;
            ColumnLastName.ColumnValue = lastName;
            FKCountry = fkCountry;
            Country = country;
        }
    }

    public class DataUsersFactory : DataModelFactory<DataUsers>
    {

        public DataModel.DataModelColumnProperties<int> ColumnPropertiesUserID { get; private set;} = new DataModel.DataModelColumnProperties<int>("UserID", true, true, false, null, true);
        public DataModel.DataModelColumnProperties<string> ColumnPropertiesFirstName { get; set; } = new DataModel.DataModelColumnProperties<string>("FirstName", true, false, false);
        public DataModel.DataModelColumnProperties<string> ColumnPropertiesLastName { get; set; } = new DataModel.DataModelColumnProperties<string>("LastName", true, false, false);
        public DataModel.DataModeForeignKey FKCountryProperties;
        public override void GenerateDataColumnProperties()
        {
            DataModelColumnProperties.Clear();
            DataModelColumnProperties.Add(ColumnPropertiesUserID);
            DataModelColumnProperties.Add(ColumnPropertiesFirstName);
            DataModelColumnProperties.Add(ColumnPropertiesLastName);
        }

        public DataUsers GenerateDataModel(string firstName, string lastName, DataCountries country)
        {
            var result = new DataUsers(ColumnPropertiesUserID,
                                        ColumnPropertiesFirstName, firstName,
                                        ColumnPropertiesLastName, lastName,
                                        FKCountryProperties, country);
            // DataModels.Add(result);

            return result;
        }

        public override DataUsers GenerateAndGetDataModel()
        {
            var result = new DataUsers(ColumnPropertiesUserID,
                                        ColumnPropertiesFirstName,
                                        ColumnPropertiesLastName,
                                        FKCountryProperties);
            // DataModels.Add(result);
            return result;
        }

        public override string GetCommandText_Select(List<DataModel.IDataModelColumn> parameters = null, int pageSize = 0, int startIndex = 0, List<DataModel.IDataModelColumnProperties> orderby = null)
        {
            return base.GetCommandText_Select(parameters, pageSize, startIndex, orderby);
        }

        public DataUsersFactory (DataCountriesFactory dataCountriesFactory) : base(DataUsers.TABLE_NAME)
        {
            FKCountryProperties = new DataModel.DataModeForeignKey(dataCountriesFactory, true);

            DataModelForeignKeyProperties.Clear();
            DataModelForeignKeyProperties.Add(FKCountryProperties);
        }
    }
    #endregion Users
}