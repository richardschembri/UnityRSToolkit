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

        public DataCountries GenerateAndGetDataModel(int countryID, string countryName)
        {
            var result = new DataCountries(ColumnPropertiesColumnCountryID, countryID,
                                               ColumnPropertiesColumnCountryName, countryName);
            //DataModels.Add(result);

            return result;
        }

        public override DataCountries GenerateAndGetDataModel()
        {
            var result = new DataCountries(ColumnPropertiesColumnCountryID,
                                               ColumnPropertiesColumnCountryName);
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
        DataModeForeignKeyProperties _fkCountryProperties;
        public DataCountries FKCountry { 
            get{
                return (DataCountries)DataModelForeignKeys[_fkCountryProperties][0];
            }
            set{
                DataModelForeignKeys[_fkCountryProperties][0] = value;
            }
        } 

        private void Init(DataModelColumnProperties<int> columnUserIDProperties,
                                DataModelColumnProperties<string> columnFirstNameProperties,
                                DataModelColumnProperties<string> columnLastNameProperties,
                                DataModeForeignKeyProperties fkCountryProperties)
        {
            ColumnUserID  = new DataModelColumn<int>(columnUserIDProperties);
            DataModelColumns.Add(ColumnUserID);
            ColumnFirstName  = new DataModelColumn<string>(columnFirstNameProperties);
            DataModelColumns.Add(ColumnFirstName);
            ColumnLastName = new DataModelColumn<string>(columnLastNameProperties);
            DataModelColumns.Add(ColumnFirstName);

            DataModelForeignKeys.Add(fkCountryProperties, new List<IDataModel>());
        }

        public DataUsers (DataModelColumnProperties<int> columnUserIDProperties,
                                DataModelColumnProperties<string> columnFirstNameProperties,
                                DataModelColumnProperties<string> columnLastNameProperties,
                                DataModeForeignKeyProperties fkCountryProperties)
        {
            Init(columnUserIDProperties, columnFirstNameProperties, columnLastNameProperties, fkCountryProperties);

        }

        public DataUsers (DataModelColumnProperties<int> columnUserIDProperties,
                                DataModelColumnProperties<string> columnFirstNameProperties, string firstName,
                                DataModelColumnProperties<string> columnLastNameProperties, string lastName,
                                DataModeForeignKeyProperties fkCountryProperties, DataCountries country)
        {
            Init(columnUserIDProperties, columnFirstNameProperties, columnLastNameProperties, fkCountryProperties);
            ColumnFirstName.ColumnValue = firstName;
            ColumnLastName.ColumnValue = lastName;
            FKCountry = country;
        }
    }

    public class DataUsersFactory : DataModelFactory<DataUsers>
    {

        public DataModel.DataModelColumnProperties<int> ColumnPropertiesUserID { get; private set;} = new DataModel.DataModelColumnProperties<int>("UserID", true, true, false);
        public DataModel.DataModelColumnProperties<string> ColumnPropertiesFirstName { get; set; } = new DataModel.DataModelColumnProperties<string>("FirstName", true, false, false);
        public DataModel.DataModelColumnProperties<string> ColumnPropertiesLastName { get; set; } = new DataModel.DataModelColumnProperties<string>("LastName", true, false, false);
        public DataModel.DataModeForeignKeyProperties FKCountryProperties;
        public override void GenerateDataColumnProperties()
        {
            DataModelColumnProperties.Clear();
            DataModelColumnProperties.Add(ColumnPropertiesUserID);
            DataModelColumnProperties.Add(ColumnPropertiesFirstName);
            DataModelColumnProperties.Add(ColumnPropertiesLastName);

            DataModelForeignKeyProperties.Clear();
            DataModelForeignKeyProperties.Add(FKCountryProperties);
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

        public DataUsersFactory () : base(DataUsers.TABLE_NAME)
        {

        }
    }
    #endregion Users
}