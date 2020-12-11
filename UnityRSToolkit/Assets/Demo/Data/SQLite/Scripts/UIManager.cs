using UnityEngine;
using UnityEngine.UI;

namespace Demo.Data.SQLite
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Dropdown DropdownCountries;

        private void PopulateDropdownCountries()
        {
            DropdownCountries.ClearOptions();
            var countries = DemoDataManager.GetInstance().BasicSelect_Countries();
            for(int i = 0; i < countries.Count; i++)
            {
                DropdownCountries.options.Add(new Dropdown.OptionData(countries[i].ColumnCountryName.ColumnValue));
            }
        }

        private void RefreshUI()
        {
            PopulateDropdownCountries();
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