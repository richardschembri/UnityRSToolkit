using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RSToolkit.UI.Controls
{
    public class UIPermanentSelectable : RSMonoBehaviour
    {
        public UISelectable[] UISelectableComponents { get; private set; }
        public UISelectable CurrentSelection { get; private set; } = null;

        private void GatherUISelectables()
        {
            UISelectableComponents = GetComponentsInChildren<UISelectable>(true);
            CurrentSelection = UISelectableComponents.FirstOrDefault(s => s.IsSelected);
        }

        protected override void InitComponents()
        {
            base.InitComponents();
            GatherUISelectables();
        }

        protected override void InitEvents()
        {
            base.InitEvents();
            for (int i = 0; i < UISelectableComponents .Length; i++)
            {
                UISelectableComponents[i].OnSelected.AddListener(UISelectableComponentsOnSelected_Listener);
            }
        }
        private void UISelectableComponentsOnSelected_Listener(UISelectable uiSelectable )
        {
            CurrentSelection = uiSelectable;
        }

        private Navigation GetNewExplicitNav()
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            return nav;
        }

        private Navigation GetNewExplicitNav(Navigation target)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnRight = target.selectOnRight;
            nav.selectOnDown = target.selectOnDown;
            nav.selectOnUp = target.selectOnUp;
            nav.selectOnLeft = target.selectOnLeft;
            return nav;
        }

        public bool SetSequentialNavigation()
        {
            Navigation nav;
            if (UISelectableComponents.Length <= 0)
            {
                return false;
            }
            for (int i = 0; i < UISelectableComponents.Length; i++)
            {

                nav = GetNewExplicitNav();
                if (i > 0)
                {
                    nav.selectOnUp = UISelectableComponents[i - 1].SelectableComponent;
                }
                if (i < UISelectableComponents.Length - 1)
                {
                    nav.selectOnDown = UISelectableComponents[i + 1].SelectableComponent;
                }
                UISelectableComponents[i].SelectableComponent.navigation = nav;
            }
            return true;
        }

        void LateUpdate()
        {
            if (CurrentSelection != null && !CurrentSelection.IsSelected)
            {
                EventSystem.current.SetSelectedGameObject(CurrentSelection.gameObject, new BaseEventData(EventSystem.current));
            }
        }

    }
}
