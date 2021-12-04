using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSToolkit.UI.Paging
{
    [RequireComponent(typeof(Toggle))]
    public class UIPageMenuButton : RSMonoBehaviour, IPointerClickHandler, ISubmitHandler
    {
        public UIPage TargetPage;

        public bool KeepCacheOnNavigate = false;
        public bool HideBackgroundOnToggle = false;
        public bool AllowRenavigate = false;

        private Toggle _toggleComponent;

        public Toggle ToggleComponent
        {
            get
            {
                if(_toggleComponent == null)
                {
                    _toggleComponent = GetComponent<Toggle>();
                }
                return _toggleComponent;
            }
        }

        private Text _textComponent;

        private Text _TextComponent
        {
            get
            {
                if(_textComponent == null)
                {
                    _textComponent = GetComponentInChildren<Text>(true);
                }
                return _textComponent;
            }
        }

        protected override void InitEvents()
        {
            base.InitEvents();
            ToggleComponent.onValueChanged.AddListener(onValueChanged_Listener);
        }
        bool _isSamePage;

        private void onValueChanged_Listener(bool on)
        {
            _isSamePage = true;
            if (on)
            {
                TargetPage.NavigateTo(KeepCacheOnNavigate);
                _isSamePage = false;
            }
            if (HideBackgroundOnToggle)
            {
                ToggleComponent.targetGraphic.gameObject.SetActive(!on);
            }
        }

        /// <summary>
        /// React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (_isSamePage && AllowRenavigate)
                TargetPage.NavigateTo(KeepCacheOnNavigate);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (_isSamePage && AllowRenavigate)
                TargetPage.NavigateTo(KeepCacheOnNavigate);
        }

        public bool TrySetText(string text)
        {
            if (_TextComponent == null)
            {
                return false;
            }

            _TextComponent.text = text;
            return true;
        }

    }
}
