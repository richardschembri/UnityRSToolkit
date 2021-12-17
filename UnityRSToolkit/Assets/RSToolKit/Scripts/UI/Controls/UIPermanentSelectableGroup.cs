using System.Collections.Generic;
using System.Linq;

namespace RSToolkit.UI.Controls
{
    public class UIPermanentSelectableGroup : RSMonoBehaviour
    {
        private List<UIPermanentSelectable> _uiPermanentSelectables = new List<UIPermanentSelectable>();
        public UISelectable CurrentSelection { get; private set; } = null;

        public bool Contains(UISelectable target)
        {
            return _uiPermanentSelectables.Any(ps => ps.UISelectableComponents.Contains(target));
        }
        public bool RegisterUIPermanentSelectable(UIPermanentSelectable uiPermanentSelectable)
        {
            if (_uiPermanentSelectables.Contains(uiPermanentSelectable))
                return false;
            _uiPermanentSelectables.Add(uiPermanentSelectable);
            uiPermanentSelectable.OnCurrentSelectionChanged.AddListener(UISelectableComponentsOnSelected_Listener);
            if(uiPermanentSelectable.CurrentSelection != null)
            {
                CurrentSelection = uiPermanentSelectable.CurrentSelection;
            }
            return true;
        }


        public bool UnRegisterUIPermanentSelectable(UIPermanentSelectable uiPermanentSelectable)
        {
            if (_uiPermanentSelectables.Contains(uiPermanentSelectable))
                return false;
            uiPermanentSelectable.OnCurrentSelectionChanged.RemoveListener(UISelectableComponentsOnSelected_Listener);
            _uiPermanentSelectables.Remove(uiPermanentSelectable);
            return true;
        }


        private void UISelectableComponentsOnSelected_Listener(UISelectable uiSelectable )
        {
            CurrentSelection = uiSelectable;
        }
    }
}
