namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RSToolkit.Helpers;
    using System.Linq;

    public class UIEditableListBox : UIListBox
    {
        private UIEditableListBoxItem.ListBoxItemMode m_ListMode = UIEditableListBoxItem.ListBoxItemMode.VIEW; 
        public UIEditableListBoxItem[] GetListBoxItems(){
            return ListItemSpawner.SpawnedGameObjects
                    .Select(li => li.GetComponent<UIEditableListBoxItem>()).ToArray();
        }

        public UIEditableListBoxItem[] GetToggledListItems(bool on = true){
            return GetListBoxItems().Where(t => t.IsToggleOn() == on).ToArray();
        }

        public bool HasAllToggledListItems(bool on = true){
            if(ListItemSpawner.SpawnedGameObjects.Any()){
                return ListItemSpawner.SpawnedGameObjects.Count == GetToggledListItems(on).Length;
            }
            return false;
        }

        public UIEditableListBoxItem AddEditableListItem(){
            var li = AddListItem();
            if(li != null){
                Refresh();
                return li.GetComponent<UIEditableListBoxItem>();
            }
            return null;
        }

        public UIEditableListBoxItem AddEditableListItem(string name, string text, object value = null){
            var bl = new Dictionary<string, string>();
            bl.Add("Text", text);
            return AddEditableListItem(name, bl, value);
        }

        public UIEditableListBoxItem AddEditableListItem(string name, Dictionary<string, string> nametexts, object value = null){
            var li = AddEditableListItem();
            if(li != null){
                li.name = name;
                li.SetMode(m_ListMode);
                li.SetCommonTextComponents(nametexts);
                li.Value = value;
                return li;
            }
            return null;
        }

        public void SetMode(UIEditableListBoxItem.ListBoxItemMode mode){
            m_ListMode = mode;
            var listitems = GetListBoxItems();
            for(int i = 0; i < listitems.Length; i++){
                listitems[i].SetMode(mode);
            }
        }

        public UIEditableListBoxItem.ListBoxItemMode GetMode(){
            return m_ListMode;
        }

    }
}
