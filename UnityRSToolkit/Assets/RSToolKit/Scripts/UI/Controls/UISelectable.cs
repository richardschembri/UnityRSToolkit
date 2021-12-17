using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSToolkit.UI.Controls
{
    public class UISelectable : RSMonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public bool IsSelected { get; private set; } = false;

        public class OnSelectedEvent : UnityEvent<UISelectable> { }
        public OnSelectedEvent OnSelected = new OnSelectedEvent();
        public Selectable _selectableComponent;
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
            IsSelected = false;
        }


        public void OnSelect(BaseEventData eventData)
        {
            IsSelected = true;
            OnSelected.Invoke(this);
        }

        public bool Select()
        {
            if (EventSystem.current.alreadySelecting)
            {
                return false;
            }
            EventSystem.current.SetSelectedGameObject(null, new BaseEventData(EventSystem.current));
            EventSystem.current.SetSelectedGameObject(gameObject, new BaseEventData(EventSystem.current));
            return true;
        }

        public void SelectLeft()
        {
            var target = SelectableComponent.navigation.selectOnLeft.GetComponent<UISelectable>();
            target?.Select();
        }
        public void SelectRight()
        {
            var target = SelectableComponent.navigation.selectOnRight.GetComponent<UISelectable>();
            target?.Select();
        }
        public void SelectUp()
        {
            var target = SelectableComponent.navigation.selectOnUp.GetComponent<UISelectable>();
            target?.Select();
        }
        public void SelectDown()
        {
            var target = SelectableComponent.navigation.selectOnDown.GetComponent<UISelectable>();
            target?.Select();
        }

    }
}
