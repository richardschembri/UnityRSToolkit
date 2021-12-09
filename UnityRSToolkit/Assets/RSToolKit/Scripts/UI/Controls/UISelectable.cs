using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSToolkit.UI.Controls
{
    public class UISelectable : MonoBehaviour, ISelectHandler, IDeselectHandler
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

    }
}
