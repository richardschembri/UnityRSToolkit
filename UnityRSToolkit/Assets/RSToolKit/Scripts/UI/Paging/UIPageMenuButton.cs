using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.UI.Paging
{
    [RequireComponent(typeof(Toggle))]
    public class UIPageMenuButton : RSMonoBehaviour
    {
        public UIPage TargetPage;

        public bool KeepCacheOnNavigate = false;
        public bool HideBackgroundOnToggle = false;

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

        private void onValueChanged_Listener(bool on)
        {
            if (on)
            {
                TargetPage.NavigateTo(KeepCacheOnNavigate);
            }
            if (HideBackgroundOnToggle)
            {
                ToggleComponent.targetGraphic.gameObject.SetActive(!on);
            }
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
