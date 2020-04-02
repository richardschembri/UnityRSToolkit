namespace RSToolkit.UI.Paging
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using RSToolkit.Helpers;

    public class UIPageMenu : MonoBehaviour
    {
        public struct UIMenuButton
        {
            public Button MenuButton;
            public UIPage Page;
        }

        public RectTransform Container;

        private static UIPageMenu m_instance;
        public static UIPageMenu Instance
        {
            get { return m_instance; }
        }

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

        void Start()
        {
            if(Container == null){
                Container = this.GetComponent<RectTransform>();
            }
            StartCoroutine(Init());
        }

        void Awake()
        {
            m_instance = this;
        }

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
            for (int i = 0; i < UIPageManager.Instance.Pages.Length; i++)
            {
                var page = UIPageManager.Instance.Pages[i];
                if (page.ShowInMenu)
                {
                    GenerateMenuButton(page);
                }
            }
        }

        IEnumerator Init()
        {
            yield return new WaitUntil(() => UIPageManager.Instance.InitComplete);
            if(AutoGenerate){
                GenerateMenuButtons();
            }else{
                for (int i = 0; i < UIPageManager.Instance.Pages.Length; i++)
                {
                    var page = UIPageManager.Instance.Pages[i];
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