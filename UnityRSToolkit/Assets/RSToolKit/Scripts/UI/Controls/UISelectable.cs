using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSToolkit.UI.Controls
{
    public class UISelectable : RSMonoBehaviour, ISelectHandler, IDeselectHandler,
                                                    IPointerEnterHandler, IPointerExitHandler
    {
        //public bool IsSelected { get; private set; } = false;
        public bool IsSelected { get {return EventSystem.current.currentSelectedGameObject == SelectableComponent.gameObject; } } // = false;
        public bool IsPointerInside { get; private set; } = false;

        public class OnSelectedEvent : UnityEvent<UISelectable> { }
        public OnSelectedEvent OnSelected = new OnSelectedEvent();
        [SerializeField]
        private Selectable _selectableComponent;
        public Selectable SelectableComponent {
            get
            {
                if(_selectableComponent == null)
                {
                    _selectableComponent = GetComponent<Selectable>();
                }
                return _selectableComponent;
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            // IsSelected = false;
        }


        public void OnSelect(BaseEventData eventData)
        {
            // IsSelected = true;
            OnSelected.Invoke(this);
        }

        public bool Select()
        {
            if (EventSystem.current.alreadySelecting)
            {
                return false;
            }
            EventSystem.current.SetSelectedGameObject(null, new BaseEventData(EventSystem.current));
            EventSystem.current.SetSelectedGameObject(SelectableComponent.gameObject, new BaseEventData(EventSystem.current));
            return true;
        }

        private void SelectNav(Selectable nav)
        {
            if(nav == null)
            {
                return;
            }
            var target = nav.GetComponent<UISelectable>();
            target?.Select();
        }
        public void SelectLeft()
        {
            SelectNav(SelectableComponent.navigation.selectOnLeft);
        }
        public void SelectRight()
        {
            SelectNav(SelectableComponent.navigation.selectOnRight);
        }
        public void SelectUp()
        {
            SelectNav(SelectableComponent.navigation.selectOnUp);
        }
        public void SelectDown()
        {
            SelectNav(SelectableComponent.navigation.selectOnDown);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsPointerInside = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsPointerInside = false;
        }
    }
}
