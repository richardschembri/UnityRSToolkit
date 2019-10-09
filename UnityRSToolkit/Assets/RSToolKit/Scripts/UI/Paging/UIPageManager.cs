namespace RSToolkit.UI.Paging
{
    using System.Linq;
    using System.Collections;
    using UnityEngine;
    using RSToolkit.Collections;
    

    public class UIPageManager : MonoBehaviour
    {


        #region Fields

        public KeyCode ExitKey = KeyCode.Escape;    
 
        public UIPage CurrentPage { get; private set;}

        private static UIPageManager m_instance;

        private UIPage[] m_pages;

        public UIPage NavigatedFromPage{get{
            return NavigationHistory.PeekOrDefault();
        }}

        public SizedStack<UIPage> NavigationHistory = new SizedStack<UIPage>(5);

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

        #region Properties

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

        protected virtual void Awake(){
            m_instance = this;
        }
        // Start is called before the first frame update
        protected virtual void Start()
        {
            InitPages();
            InitComplete = true;
        }

        // Update is called once per frame
        protected virtual void Update()
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

        protected void CloseOtherPages(UIPage activePage)
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

        public T GetPage<T>(){
            return Pages.OfType<T>().FirstOrDefault();
        }

        public UIPage GetPage(string name){
            return Pages.FirstOrDefault(p => p.gameObject.name == name);
        }


        #region Navigation Functions


        public virtual void NavigateTo(UIPage page, bool keepCache = false)
        {

            if (CurrentPage != null && CurrentPage.OnNavigatedFrom != null)
            {
                CurrentPage.OnNavigatedFrom.Invoke(CurrentPage);
            }
            NavigationHistory.Push(CurrentPage);
            //NavigatedFromPage = CurrentPage;
            CurrentPage = page;
            CloseOtherPages(page);

            page.gameObject.SetActive(true);

            CurrentPage.OnNavigatedTo.Invoke(CurrentPage, keepCache);

        }

        public virtual void NavigateBack(){
            if(NavigatedFromPage != null){
                //NavigatedFromPage.NavigateTo();           
                NavigationHistory.Pop().NavigateTo();
            }
        }

        public virtual void NavigateBackToLastUniquePage(){
            UIPage lastPage = null;
            do{
                lastPage = NavigationHistory.Pop();
            }while((NavigationHistory.Any() && lastPage == null)
                    || (lastPage != null && lastPage == CurrentPage));

            if(lastPage != null){
                lastPage.NavigateTo();
            }
        }

        public virtual void NavigateToNextPage()
        {
            CurrentPage.NavigateToNextPage();
        }

        public virtual void NavigateToPrevPage()
        {
            CurrentPage.NavigateToPrevPage();
        }


        #endregion
    }

}