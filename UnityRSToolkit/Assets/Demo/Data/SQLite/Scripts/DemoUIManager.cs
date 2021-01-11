using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Demo.Data.SQLite
{
    public class DemoUIManager : MonoBehaviour
    {
        [SerializeField]
        private Dropdown DropdownCountries;

        [SerializeField]
        private Dropdown DropdownUsers;

        List<DataCountries> _countries;
        private void PopulateDropdownCountries()
        {
            DropdownCountries.ClearOptions();
            _countries = DemoDataManager.GetInstance().Select_Countries();
            for(int i = 0; i < countries.Count; i++)
            {
                DropdownCountries.options.Add(new Dropdown.OptionData(countries[i].ColumnCountryName.ColumnValue));
            }
        }

        public DataUsers SelectedCountry{
            get{
                if(DropdownCountries.value <= 0){
                    return null;
                }
                return _countries[DropdownCountries.value - 1];
            }
        }
        
        List<DataUsers> _users;
        private void PopulateDropdownUsers(){
            DropdownUsers.ClearOptions();
            _users = DemoDataManager.GetInstance().Select_Users();

            DropdownCountries.options.Add(new Dropdown.OptionData(-1, "Please Select"));
            for(int i = 0; i < users.Count; i++)
            {
                DropdownCountries.options.Add(new Dropdown.OptionData($"{users[i].ColumnUserID.ColumnValue, users[i].ColumnFirstName.ColumnValue}/{users[i].ColumnLastName.ColumnValue}/{users[i].FKCountry.ColumnCountryName.ColumnValue}"));
            }
        }
        public DataUsers SelectedUser{
            get{
                if(DropdownUsers.value <= 0){
                    return null;
                }
                return _users[DropdownUsers.value - 1];
            }
        }

        private void RefreshUI()
        {
            PopulateDropdownCountries();
            PopulateDropdownUsers();
        }

        public void OnInitDatabase_Listener()
        {
            RefreshUI();
        }


        #region Mono Functions
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion Mono Functions
    }
}