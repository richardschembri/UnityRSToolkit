namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;
    using RSToolkit.Helpers;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(UIDropdownSettings))]
    public class UIDropdown : Dropdown 
    {
        private UIDropdownSettings m_settings;
        public UIDropdownSettings Settings { 
            get { 
                if(m_settings == null){
                   m_settings = GetComponent<UIDropdownSettings>(); 
                }
                return m_settings;
            }
        }
        public UnityEvent onShow = new UnityEvent();

        public GameObject GetDropdownList(){
            return gameObject.GetChild("Dropdown List");
        }

        public GameObject GetDropdownListContent(){
            return GetDropdownList().GetChild("Content");
        }
        public override void OnPointerClick(PointerEventData eventData){
            AdvancedShow();
        }

        public override void OnSubmit(BaseEventData eventData){
            AdvancedShow();
        }

        private void AdvancedShow(){
            if(GetDropdownList() != null){
                return;
            }
            Show();
            var dropdownRect = GetDropdownList().GetComponent<RectTransform>();
            dropdownRect.sizeDelta = dropdownRect.sizeDelta + Settings.SizeOffset;
            var item = template.GetComponentInChildren<DropdownItem>().GetComponent<RectTransform>();
            float newY = item.sizeDelta.y * value; 
            if (newY < 0f){
                newY = 0f;
            }
            var ap = GetDropdownListContent().GetComponent<RectTransform>().anchoredPosition;
            GetDropdownListContent().GetComponent<RectTransform>().anchoredPosition = new Vector2(ap.x, newY);
            onShow.Invoke();

        }
    }
}
