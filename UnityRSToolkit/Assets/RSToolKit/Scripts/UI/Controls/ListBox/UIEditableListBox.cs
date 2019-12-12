namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RSToolkit.Helpers;
    using System.Linq;

    public class UIEditableListBox : UIListBox
    {
        [SerializeField]
        private bool m_OrderAscending = true;
        public bool orderAscending { get { return m_OrderAscending; } set { m_OrderAscending = value; Refresh(); }}
        private UIEditableListBoxItem.ListBoxItemMode m_ListMode = UIEditableListBoxItem.ListBoxItemMode.VIEW; 


       protected override RectTransform[] m_ContentChildren{
           get{
            if (m_contentChildren == null)
                if(orderAscending){
                    m_contentChildren = ContentRectTransform.GetTopLevelChildren<RectTransform>()
                                         .OrderBy(rt => rt.GetComponent<UIEditableListBoxItem>().OrderIndex).ToArray();
                }else{
                    m_contentChildren = ContentRectTransform.GetTopLevelChildren<RectTransform>()
                                         .OrderByDescending(rt => rt.GetComponent<UIEditableListBoxItem>().OrderIndex).ToArray();
                }
            return m_contentChildren;
           }
       } 
        public UIEditableListBoxItem[] GetListBoxItems(){
            return listItemSpawner.SpawnedGameObjects
                    .Select(li => li.GetComponent<UIEditableListBoxItem>()).ToArray();
        }

        public UIEditableListBoxItem[] GetToggledListItems(bool on = true){
            return GetListBoxItems().Where(t => t.IsToggleOn() == on).ToArray();
        }

        public bool HasAllToggledListItems(bool on = true){
            if(listItemSpawner.SpawnedGameObjects.Any()){
                return listItemSpawner.SpawnedGameObjects.Count == GetToggledListItems(on).Length;
            }
            return false;
        }

       public override GameObject AddListItem(){
           return AddListItem(null);
       }

       public GameObject AddListItem(int? orderIndex){
           return AddEditableListItem(orderIndex)?.gameObject ?? null;
       }
        public UIEditableListBoxItem AddEditableListItem(int? orderIndex = null){
            if(listItemSpawner == null){
                return null;
            }
            var li = listItemSpawner.SpawnAndGetGameObject();

            UIEditableListBoxItem result = null;
            if(li != null){
                // Refresh();
                result = li.GetComponent<UIEditableListBoxItem>();
                result.OrderIndex = orderIndex ?? listItemSpawner.SpawnedGameObjects.Count;
            }
            Refresh();
            return result;
        }

        public UIEditableListBoxItem AddEditableListItem(string name, string text, object value = null, int? orderIndex = null){
            var bl = new Dictionary<string, string>();
            bl.Add("Text", text);
            return AddEditableListItem(name, bl, value, orderIndex);
        }

        public UIEditableListBoxItem AddEditableListItem(string name, Dictionary<string, string> nametexts, object value = null, int? orderIndex = null){
            var li = AddEditableListItem(orderIndex);
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
