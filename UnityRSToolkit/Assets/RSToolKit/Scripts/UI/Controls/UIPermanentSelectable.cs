using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.UI.Controls
{
    public class UIPermanentSelectable : RSMonoBehaviour
    {
        public UISelectable[] UISelectableComponents { get; private set; }
        public UISelectable LastSelection { get; private set; } = null;
        public UISelectable CurrentSelection { get; private set; } = null;

        public UISelectable.OnSelectedEvent OnCurrentSelectionChanged = new UISelectable.OnSelectedEvent();

        [SerializeField]
        private UIPermanentSelectableGroup _group;
        private UIPermanentSelectableGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
                SetGroup(_group, true);
            }
        }

        public bool HasSelected { get => CurrentSelection != null && CurrentSelection.IsSelected; }

        void OnDisable()
        {
            SetGroup(null, false);
        }
        void OnEnable()
        {
            SetGroup(_group, false);
        }

        private void SetGroup(UIPermanentSelectableGroup newGroup, bool setMemberValue)
        {
            if (_group != null)
                _group.UnRegisterUIPermanentSelectable(this);

            if (setMemberValue)
                _group = newGroup;

            if (newGroup != null && isActiveAndEnabled)
                newGroup.RegisterUIPermanentSelectable(this);
                
        }

        private void GatherUISelectables()
        {
            UISelectableComponents = GetComponentsInChildren<UISelectable>(true);
            LastSelection = null;
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
            LastSelection = CurrentSelection;
            CurrentSelection = uiSelectable;
            OnCurrentSelectionChanged.Invoke(CurrentSelection);
        }

        private static Navigation GetNewExplicitNav()
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

        public static bool SetSequentialNavigationVertical(UISelectable[] uiSelectableComponents)
        {
            Navigation nav;
            if (uiSelectableComponents.Length <= 0)
            {
                return false;
            }
            for (int i = 0; i < uiSelectableComponents.Length; i++)
            {

                nav = GetNewExplicitNav();
                if (i > 0)
                {
                    nav.selectOnUp = uiSelectableComponents[i - 1].SelectableComponent;
                }
                if (i < uiSelectableComponents.Length - 1)
                {
                    nav.selectOnDown = uiSelectableComponents[i + 1].SelectableComponent;
                }
                uiSelectableComponents[i].SelectableComponent.navigation = nav;
            }
            return true;
        }

        public bool SetSequentialNavigationVertical()
        {
            return SetSequentialNavigationVertical(UISelectableComponents);
        }
        public static bool SetSequentialNavigationHorizontal(UISelectable[] uiSelectableComponents)
        {
            Navigation nav;
            if (uiSelectableComponents.Length <= 0)
            {
                return false;
            }
            for (int i = 0; i < uiSelectableComponents.Length; i++)
            {

                nav = GetNewExplicitNav();
                if (i > 0)
                {
                    nav.selectOnLeft = uiSelectableComponents[i - 1].SelectableComponent;
                }
                if (i < uiSelectableComponents.Length - 1)
                {
                    nav.selectOnRight = uiSelectableComponents[i + 1].SelectableComponent;
                }
                uiSelectableComponents[i].SelectableComponent.navigation = nav;
            }
            return true;
        }

        public bool SetSequentialNavigationHorizontal()
        {
            return SetSequentialNavigationHorizontal(UISelectableComponents);
        }

        private bool Contains(UISelectable target)
        {
           if(Group != null)
            {
                return Group.Contains(target);
            }

            return UISelectableComponents.Contains(target);
        }

        #region SelectOn

        private bool CannotSelectOn(UISelectable uiselectable)
        {
            return uiselectable != null && !Contains(uiselectable);
        }

        public bool SetSelectOnLeft(UISelectable uiselectable)
        {
            if (CannotSelectOn(uiselectable))
            {
                return false;
            }

            for (int i = 0; i < UISelectableComponents.Length; i++)
            {
                var nav = UISelectableComponents[i].SelectableComponent.navigation;
                nav.selectOnLeft = null;
                if(uiselectable != null)
                {
                    nav.selectOnLeft = uiselectable.SelectableComponent;
                }
                UISelectableComponents[i].SelectableComponent.navigation = nav;
            }

            return true;
        }
        public bool SetSelectOnRight(UISelectable uiselectable)
        {
            if (CannotSelectOn(uiselectable))
            {
                return false;
            }

            for (int i = 0; i < UISelectableComponents.Length; i++)
            {
                var nav = UISelectableComponents[i].SelectableComponent.navigation;
                if (uiselectable != null)
                {
                    nav.selectOnRight = uiselectable.SelectableComponent;
                }
                UISelectableComponents[i].SelectableComponent.navigation = nav;
            }

            return true;
        }
        public bool SetSelectOnUp(UISelectable uiselectable)
        {
            if (CannotSelectOn(uiselectable))
            {
                return false;
            }

            for (int i = 0; i < UISelectableComponents.Length; i++)
            {
                var nav = UISelectableComponents[i].SelectableComponent.navigation;
                if (uiselectable != null)
                {
                    nav.selectOnUp = uiselectable.SelectableComponent;
                }
                UISelectableComponents[i].SelectableComponent.navigation = nav;
            }

            return true;
        }

        public bool SetSelectOnDown(UISelectable uiselectable)
        {
            if (CannotSelectOn(uiselectable))
            {
                return false;
            }

            for (int i = 0; i < UISelectableComponents.Length; i++)
            {
                var nav = UISelectableComponents[i].SelectableComponent.navigation;
                if (uiselectable != null)
                {
                    nav.selectOnDown = uiselectable.SelectableComponent;
                }
                UISelectableComponents[i].SelectableComponent.navigation = nav;
            }

            return true;
        }

        #endregion SelectOn

        void LateUpdate()
        {
            if(Group != null && Group.CurrentSelection != null && Group.CurrentSelection.IsSelected)
            {
                return;
            }
            if (CurrentSelection != null && !CurrentSelection.IsSelected
                && (Group == null || Group.CurrentSelection == CurrentSelection))
            {
                CurrentSelection.Select();
            }
        }

    }
}
