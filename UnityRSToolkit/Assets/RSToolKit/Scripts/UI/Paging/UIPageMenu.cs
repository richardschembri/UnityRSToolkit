using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.Helpers;
using RSToolkit.Controls;

namespace RSToolkit.UI.Paging
{
    [RequireComponent(typeof(ToggleGroup))]
    public class UIPageMenu : Spawner<UIPageMenuButton> // RSMonoBehaviour//RSSingletonMonoBehaviour<UIPageMenu>
    {

        [SerializeField]
        private UIPageManager _UIPageManagerInstance;

        public bool AutoGenerateMenu = true;
        public bool AutoShow = false;
        // public bool DisableActivePageMenuButton = true;
        private ToggleGroup _toggleGroupComponent;

        public ToggleGroup ToggleGroupComponent
        {
            get
            {
                if(_toggleGroupComponent == null)
                {
                    _toggleGroupComponent = GetComponent<ToggleGroup>();
                }
                return _toggleGroupComponent;
            }
        }

        void Start()
        {
            StartCoroutine(DelayedInit());
        }

        private UIPageMenuButton GenerateMenuButton(UIPage page)
        {
            var menuButton = SpawnAndGetGameObject().GetComponent<UIPageMenuButton>(); //Instantiate(TemplateMenuButton);

            // menuButton.ToggleComponent.isOn = page.LaunchPage;//page.IsCurrentPage();
            menuButton.ToggleComponent.group = ToggleGroupComponent;
            menuButton.name = string.Format("{0} Menu Button", page.name);
            //GameObjectHelpers.NormalizeTransform(menuButton.transform);
            menuButton.transform.ResetScaleAndRotation();
            menuButton.TrySetText(page.GetHeader());
            menuButton.TargetPage = page;
            page.OnNavigatedTo.AddListener(onNavigatedTo);

            return menuButton;
        }

        public void GenerateMenuButtons()
        {
            SetPoolSize(_UIPageManagerInstance.Core.Pages.Length);
            DestroyAllSpawns();
            UIPageMenuButton openPageButton = null;
            ToggleGroupComponent.allowSwitchOff = true;
            for (int i = 0; i < _UIPageManagerInstance.Core.Pages.Length; i++)
            {
                var page = _UIPageManagerInstance.Core.Pages[i];
                if (page.ShowInMenu)
                {
                    var menuButton = GenerateMenuButton(page);
                    if (page.IsCurrentPage())
                    {
                        openPageButton = menuButton;
                    }
                }
            }

            ToggleGroupComponent.SetAllTogglesOff(false);
            if(openPageButton != null)
            {
                ToggleGroupComponent.allowSwitchOff = false;
                openPageButton.ToggleComponent.SetIsOnWithoutNotify(true);
            }
        }

        IEnumerator DelayedInit()
        {
            if(_UIPageManagerInstance == null){
                yield return new WaitUntil(() => UIPageManager.Instance != null); //InitComplete);
                _UIPageManagerInstance = UIPageManager.Instance;
            }
            yield return new WaitUntil(() => _UIPageManagerInstance.Initialized); //InitComplete);
            if(AutoGenerateMenu){
                GenerateMenuButtons();
            }else{
                ToggleGroupComponent.allowSwitchOff = true;
                ToggleGroupComponent.SetAllTogglesOff(false);
                for (int i = 0; i < _UIPageManagerInstance.Core.Pages.Length; i++)
                {
                    var page = _UIPageManagerInstance.Core.Pages[i];
                    if(page.IsCurrentPage() && page.ShowInMenu)
                    {
                        GetMenuButton(page).ToggleComponent.SetIsOnWithoutNotify(true);
                        ToggleGroupComponent.allowSwitchOff = false;
                    }
                    page.OnNavigatedTo.AddListener(onNavigatedTo);
                }
            }
        }


        #region Page Events
        void onNavigatedTo(UIPage page, bool keepCache)
        {
            /*
            for (int i = 0; i < MenuButtons.Count(); i++)
            {
                if(DisableActivePageMenuButton ){
                    MenuButtons[i].MenuButton.interactable = MenuButtons[i].Page != page;
                }
            }
            */

            if(AutoShow){
                this.gameObject.SetActive(page.DisplayMenu);
            }
            else{
                CloseMenu();
            }

            if (!page.ShowInMenu)
            {
                ToggleGroupComponent.SetAllTogglesOff(false);
            }
            else
            {
                var mb = GetMenuButton(page);
                if(mb != null && !mb.ToggleComponent.isOn)
                {
                    mb.ToggleComponent.SetIsOnWithoutNotify(true);
                }
            } 
        }
        #endregion Page Events

        public UIPageMenuButton GetActiveMenuButton()
        {
            return SpawnedGameObjects.FirstOrDefault(sgo => sgo.ToggleComponent.isOn);

        }

        public UIPageMenuButton GetMenuButton(UIPage page)
        {
            return SpawnedGameObjects.FirstOrDefault(mb => mb.TargetPage == page);
        }
        

        public void OpenMenu()
        {
            this.gameObject.SetActive(true);
        }

        public void CloseMenu()
        {
            this.gameObject.SetActive(false);
        }
    }
}