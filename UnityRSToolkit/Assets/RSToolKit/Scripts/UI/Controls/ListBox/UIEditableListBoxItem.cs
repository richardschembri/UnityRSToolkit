namespace RSToolkit.UI.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    [AddComponentMenu("RSToolKit/Controls/UIEditableListBoxItem")]
    public class UIEditableListBoxItem : Selectable, IPointerClickHandler, ISubmitHandler
    {

        int m_orderIndex;
        public int OrderIndex {get{return m_orderIndex;} set{m_orderIndex = value;}}

        public enum ListBoxItemMode
        {
            VIEW,
            SELECT,
            EDIT,
            DELETE
        }

        public ListBoxItemMode Mode { get; private set; }

        private Dictionary<ListBoxItemMode, Transform> m_modeTransforms;

        private Dictionary<ListBoxItemMode, Transform> ModeTransforms
        {
            get
            {
                if (m_modeTransforms == null)
                {
                    m_modeTransforms = new Dictionary<ListBoxItemMode, Transform>();
                    AddModeTransform(ListBoxItemMode.VIEW, "ViewTransform");
                    AddModeTransform(ListBoxItemMode.SELECT, "SelectTransform");
                    AddModeTransform(ListBoxItemMode.EDIT, "EditTransform");
                    AddModeTransform(ListBoxItemMode.DELETE, "DeleteTransform");
                }
                return m_modeTransforms;
            }
        }

        private void AddModeTransform(ListBoxItemMode mode, string transform_name)
        {
            if (!m_modeTransforms.ContainsKey(mode))
            {
                var t = this.gameObject.transform.Find(transform_name);
                if (t != null)
                {
                    m_modeTransforms.Add(mode, this.gameObject.transform.Find(transform_name));
                }
            }
        }

        public Transform GetModeTransform()
        {
            return GetModeTransform(Mode);
        }

        public Transform GetModeTransform(ListBoxItemMode mode)
        {
            if (ModeTransforms.ContainsKey(mode))
            {
                return ModeTransforms[mode];
            }
            return null;
        }

        public class OnItemClickEvent : UnityEvent<UIEditableListBoxItem> { }
        public OnItemClickEvent OnItemClick = new OnItemClickEvent();

        private Button buttonComponent;
        public Button ButtonComponent
        {
            get
            {
                if (buttonComponent == null)
                {
                    buttonComponent = this.gameObject.GetComponent<Button>();
                }
                return buttonComponent;
            }
        }

        public object Value { get; set; }
        void SetTransformActive(Transform transform, bool value)
        {
            if (transform != null)
            {
                transform.gameObject.SetActive(value);
            }
        }
        public void SetMode(ListBoxItemMode mode)
        {
            if (ModeTransforms.ContainsKey(mode))
            {
                Mode = mode;
            }
            else
            {
                Mode = ModeTransforms.First().Key;
            }

            foreach (var mt in ModeTransforms)
            {
                SetTransformActive(mt.Value, mt.Key == mode);
            }

        }

        void SetTextComponent(Transform transform, string name, string text)
        {
            if (transform != null)
            {
                transform.Find(name).GetComponent<Text>().text = text;
            }
        }

        public void SetModeTextComponent(ListBoxItemMode mode, string name, string text)
        {
            SetTextComponent(ModeTransforms[mode], name, text);
        }
        
        public void SetCommonTextComponent(string name, string text){

            var nt = new Dictionary<string, string>();
            nt.Add(name, text);
            SetCommonTextComponents(nt);
        }
        public void SetModeTextComponents(ListBoxItemMode mode, Dictionary<string, string> nametexts){
            foreach (var nt in nametexts)
            {
                SetModeTextComponent(mode, nt.Key, nt.Value);
            }
        }
        public void SetCommonTextComponents(Dictionary<string, string> nametexts)
        {
            foreach (var mtv in ModeTransforms.Values)
            {
                foreach (var nt in nametexts)
                {
                    SetTextComponent(mtv, nt.Key, nt.Value);
                }
            }
        }

        public bool IsToggleOn()
        {
            var t = GetToggle();

            return t != null ? t.isOn : false;
        }

        public Toggle GetToggle(){
            return ModeTransforms[Mode].GetComponentInChildren<Toggle>();
        }

        void SetToggle(Transform t, bool value)
        {
            if (t == null)
            {
                return;
            }

            var componentToggle = t.GetComponentInChildren<Toggle>();
            if (componentToggle == null)
            {
                return;
            }

            componentToggle.isOn = value;
        }

        public void SetToggles(bool isOn)
        {
            foreach (var mt in ModeTransforms)
            {
                SetToggle(mt.Value, isOn);
            }
        }
        public void SetToggleOnItemClick(ListBoxItemMode mode)
        {
            var mt = GetModeTransform(mode);
            if (mt == null)
            {
                return;
            }
            OnItemClick.AddListener((UIEditableListBoxItem item) => { if (Mode == mode) { SetToggles(!IsToggleOn()); } });

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button != PointerEventData.InputButton.Left)
                return;
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            OnItemClick.Invoke(this);
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;


            OnItemClick.Invoke(this);
        }
    }
}