using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Demo.Data.SQLite
{
    public class DemoUIManager : MonoBehaviour
    {

        [SerializeField]
        private Text FieldUserID;

        [SerializeField]
        private InputField FieldFirstName;

        [SerializeField]
        private InputField FieldLastName;

        [SerializeField]
        private Dropdown DropdownCountries;

        [SerializeField]
        private Dropdown DropdownUsers;

        List<DataCountries> _countries;
        private void PopulateDropdownCountries()
        {
            DropdownCountries.ClearOptions();
            _countries = DemoDataManager.GetInstance().Select_Countries();
            for(int i = 0; i < _countries.Count; i++)
            {
                DropdownCountries.options.Add(new Dropdown.OptionData(_countries[i].ColumnCountryName.ColumnValue));
            }
        }

        public DataCountries SelectedCountry{
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

            DropdownUsers.options.Add(new Dropdown.OptionData("Please Select"));
            for(int i = 0; i < _users.Count; i++)
            {
                DropdownCountries.options.Add(new Dropdown.OptionData($"{_users[i].ColumnFirstName.ColumnValue}/{_users[i].ColumnLastName.ColumnValue}/{_users[i].FKCountry.ColumnCountryName.ColumnValue}"));
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

        public void SaveUserData()
        {
           if(!string.IsNullOrEmpty(FieldFirstName.text)
                && !string.IsNullOrEmpty(FieldLastName.text)
                && SelectedCountry != null)
            {
                if(DemoDataManager.GetInstance().Insert_User(FieldFirstName.text,
                    FieldLastName.text, SelectedCountry))
                {
                    PopulateDropdownUsers();
                }
            }
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