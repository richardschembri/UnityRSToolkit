namespace RSToolkit.UI.Paging
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using RSToolkit.Helpers;

    public class UIPageManager : MonoBehaviour
    {


        #region Fields

        public KeyCode ExitKey = KeyCode.Escape;    
 
        public UIPage CurrentPage { get; private set;}

        private static UIPageManager m_instance;

        private UIPage[] m_pages;

        private bool m_initComplete = false;
        public bool InitComplete
        {
            get
            {
                return m_initComplete;
            }
            private set
            {
                m_initComplete = value;
            }
        }
   

        #endregion

        #region Propertiesn

        public static UIPageManager Instance{
            get{ return m_instance; }
        }
       
        public UIPage[] Pages{
            get{
                if (m_pages == null){
                    m_pages = GetPages();
                }
                return m_pages;
            }
        }

        public UIPage[] GetPages() {
            return transform.GetComponentsInChildren<UIPage>(true)
                        .OrderBy(p => p.SortOrder).ToArray();
        }

        #endregion
        #region Unity Events

        void Awake(){
            m_instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            InitPages();
            InitComplete = true;
        }

        // Update is called once per frame
        void Update()
        {
           if (Input.GetKeyUp(ExitKey)){
               Application.Quit();
           } 

           for (int i = 0; i < Pages.Length; i++){
               if(Input.GetKeyUp(Pages[i].NavigateShortCut)){
                   NavigateTo(Pages[i]);
               }
           } 
        }
        #endregion

        public void InitPages(){
            

            UIPage launchPage = Pages[0];
            for (int i = 0; i < Pages.Length; i++){
                var uiPage = Pages[i];
                
                uiPage.gameObject.SetActive(false);

                if (uiPage.LaunchPage){
                    launchPage = uiPage;
                }
            }
            NavigateTo(launchPage);
        }


        #region Page Functions

        private void CloseOtherPages(UIPage activePage)
        {
            for (int i = 0; i < Pages.Length; i++)
            {
                var page = Pages[i];
                if (page != activePage)
                {
                    page.gameObject.SetActive(false);
                }
            }
        }

        #endregion

        public bool IsCurrentPage(UIPage page)
        {
            return CurrentPage == page;
        }


        #region Navigation Functions


        public void NavigateTo(UIPage page, bool keepCache = false)
        {

            if (CurrentPage != null && CurrentPage.OnNavigatedFrom != null)
            {
                CurrentPage.OnNavigatedFrom.Invoke(CurrentPage);
            }

            CurrentPage = page;
            CloseOtherPages(page);

            page.gameObject.SetActive(true);

            CurrentPage.OnNavigatedTo.Invoke(CurrentPage, keepCache);

        }

        public void NavigateToNextPage()
        {
            CurrentPage.NavigateToNextPage();
        }

        public void NavigateToPrevPage()
        {
            CurrentPage.NavigateToPrevPage();
        }


        #endregion
    }

}