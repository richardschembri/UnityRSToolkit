namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RSToolkit.Helpers;

    [RequireComponent(typeof(UIListBox))]
    public class UIEditableListBox : MonoBehaviour
    {
        private UIListBox listBox{
            get{
                return this.GetComponent<UIListBox>();
            }
        }
        public UIEditableListBoxItem ListBoxItemTemplate;  

        private UIEditableListBox[] ListBoxItems{
            get{
                return null; //listBox.content.GetComponent<RectTransform>().;
            }
        }
        /* 
        public UIEditableListBoxItem GetListItemByName(string name){
            return listItems.FirstOrDefault(li => li.gameObject.name == id);
        }*/



        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
