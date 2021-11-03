using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RSToolkit.Helpers;

namespace RSToolkit.UI.Paging
{
    public class UIPageMenu : RSMonoBehaviour//RSSingletonMonoBehaviour<UIPageMenu>
    {
        public struct UIMenuButton
        {
            public Button MenuButton;
            public UIPage Page;
        }

        public RectTransform Container;

        [SerializeField]
        private UIPageManager _UIPageManagerInstance;

        public bool AutoGenerate = true;
        public bool AutoShow = false;
        public bool DisableActivePageMenuButton = true;

        public Button TemplateMenuButton;
        private List<UIMenuButton> m_menubuttons;
        public List<UIMenuButton> MenuButtons
        {
            get
            {
                if (m_menubuttons == null)
                {
                    m_menubuttons = new List<UIMenuButton>();
                }
                return m_menubuttons;
            }
            private set{
                m_menubuttons = value;
            }

        }

        #region MonoBehaviour Functions
        void Start()
        {
            if(Container == null){
                Container = this.GetComponent<RectTransform>();
            }
            StartCoroutine(Init());
        }
        #endregion MonoBehaviour Functions

        private void GenerateMenuButton(UIPage page)
        {
            var menuButton = Instantiate(TemplateMenuButton);
            menuButton.interactable = !page.IsCurrentPage();
            //menuButton.transform.SetParent(this.transform);
            menuButton.transform.SetParent(Container.transform);
            menuButton.name = string.Format("{0} Menu Button", page.name);
            //GameObjectHelpers.NormalizeTransform(menuButton.transform);
            menuButton.transform.ResetScaleAndRotation();
            menuButton.GetComponentInChildren<Text>().text = page.GetHeader();
            menuButton.onClick.AddListener(delegate {
                page.NavigateTo();
                
            });

            page.OnNavigatedTo.AddListener(onNavigatedTo);

            var uiMenuButton = new UIMenuButton { MenuButton = menuButton, Page = page };
            MenuButtons.Add(uiMenuButton);
        }

        private void DestroyAllButtons(){
            if(!MenuButtons.Any()){
                return;
            }
            for(int i = 0; i < MenuButtons.Count; i++){
                Destroy(MenuButtons[i].MenuButton.gameObject);
            }
            MenuButtons.Clear();
        }

        public void GenerateMenuButtons()
        {
            DestroyAllButtons();
            for (int i = 0; i < _UIPageManagerInstance.Core.Pages.Length; i++)
            {
                var page = _UIPageManagerInstance.Core.Pages[i];
                if (page.ShowInMenu)
                {
                    GenerateMenuButton(page);
                }
            }
        }

        IEnumerator Init()
        {
            if(_UIPageManagerInstance == null){
                yield return new WaitUntil(() => UIPageManager.Instance != null); //InitComplete);
                _UIPageManagerInstance = UIPageManager.Instance;
            }
            yield return new WaitUntil(() => _UIPageManagerInstance.Initialized); //InitComplete);
            if(AutoGenerate){
                GenerateMenuButtons();
            }else{
                for (int i = 0; i < _UIPageManagerInstance.Core.Pages.Length; i++)
                {
                    var page = _UIPageManagerInstance.Core.Pages[i];
                    page.OnNavigatedTo.AddListener(onNavigatedTo);
                }
            }
        }


        #region Page Events
        void onNavigatedTo(UIPage page, bool keepCache)
        {
            for (int i = 0; i < MenuButtons.Count(); i++)
            {
                if(DisableActivePageMenuButton ){
                    MenuButtons[i].MenuButton.interactable = MenuButtons[i].Page != page;
                }
            }
            if(AutoShow){
                this.gameObject.SetActive(page.DisplayMenu);
            }
            else{
                CloseMenu();
            }
        }
        #endregion Page Events

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