using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSToolkit.UI.Paging
{
    public class UIPageMenuButton : RSMonoBehaviour, IPointerClickHandler, ISubmitHandler
    {
        public UIPage TargetPage;

        public bool KeepCacheOnNavigate = false;
        public bool HideBackgroundOnToggle = false;
        public bool AllowRenavigate = false;

        [SerializeField]
        private Toggle _toggleComponent;
        public Toggle ToggleComponent { get => _toggleComponent; private set => _toggleComponent = value; }

        [SerializeField]
        private Text _textComponent;

        protected override void InitComponents()
        {
            base.InitComponents();

            if(_toggleComponent == null)
            {
                ToggleComponent = GetComponent<Toggle>();
            }
            if(_textComponent == null)
            {
                _textComponent = GetComponentInChildren<Text>(true);
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

        protected void RenavigateToTargetPage()
        {
            if (_isSamePage && AllowRenavigate)
                TargetPage.NavigateTo(KeepCacheOnNavigate);
        }
        /// <summary>
        /// React to clicks.
        /// </summary>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            RenavigateToTargetPage();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            RenavigateToTargetPage();
        }

        public bool TrySetText(string text)
        {
            if (_textComponent == null)
            {
                return false;
            }

            _textComponent.text = text;
            return true;
        }

    }
}
