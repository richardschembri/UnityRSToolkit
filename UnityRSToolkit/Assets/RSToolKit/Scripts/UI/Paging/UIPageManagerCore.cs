using UnityEngine;
using RSToolkit.Collections;
using System.Linq;

namespace RSToolkit.UI.Paging
{
    public class UIPageManagerCore
    {
        public UIPageManagerCore(UIPage[] pages){
            Pages = pages;
            InitPages();
        }
        #region Fields

        public UIPage PreviousPage {get; private set;}

        public UIPage CurrentPage { get; private set;}

        public UIPage LaunchPage { get; private set;}

        public UIPage NavigatedFromPage{get{
            return NavigationHistory.PeekOrDefault();
        }}

        public SizedStack<UIPage> NavigationHistory {get; private set;} = new SizedStack<UIPage>(5);

        #endregion

        public UIPage[] Pages{ get; private set; }


        #region Page Functions
        protected void CloseOtherPages(UIPage target)
        {
            for (int i = 0; i < Pages.Length; i++)
            {
                var page = Pages[i];
                if (page != target)
                {
                    page.Close();
                }
            }
        }

        public bool InitPages(){
            
            if (Pages.Length <= 0) return false;
            LaunchPage = Pages[0];
            for (int i = 0; i < Pages.Length; i++){
                var uiPage = Pages[i];
                uiPage.SetUIPageManagerCore(this); 
                uiPage.Close();

                if (uiPage.LaunchPage){
                    LaunchPage = uiPage;
                }
            }
            NavigateTo(LaunchPage);
            return true;
        }

        public bool IsCurrentPage(UIPage page)
        {
            return CurrentPage == page;
        }

        public T GetPage<T>() where T : UIPage{
            return Pages.OfType<T>().FirstOrDefault();
        }

        public UIPage GetPage(string name){
            return Pages.FirstOrDefault(p => p.gameObject.name == name);
        }
        #endregion Page Functions

        #region Navigation Functions

        public virtual void NavigateTo(UIPage page, bool keepCache = false)
        {

            if (CurrentPage != null && CurrentPage.OnNavigatedFrom != null)
            {
                PreviousPage = CurrentPage;
                if (Application.isPlaying)
                {
                    CurrentPage.OnNavigatedFrom.Invoke(CurrentPage);
                }
            }
            NavigationHistory.Push(CurrentPage, true);

            CurrentPage = page;
            CloseOtherPages(page);

            page.gameObject.SetActive(true);

            if (Application.isPlaying)
            {
                CurrentPage.OnNavigatedTo.Invoke(CurrentPage, keepCache);
            }

        }

        public virtual void NavigateBack(bool keepCache = false){
            if(NavigatedFromPage != null){
                //NavigatedFromPage.NavigateTo();           
                NavigationHistory.Pop().NavigateTo(keepCache);
            }
        }

        public void NavigateBackToLastUniquePage(bool keepCache = false){
            UIPage lastPage = null;
            do{
                lastPage = NavigationHistory.Pop();
            }while((NavigationHistory.Any() && lastPage == null)
                    || (lastPage != null && lastPage == CurrentPage));

            if(lastPage != null){
                lastPage.NavigateTo(keepCache);
            }
        }

        public void NavigateToNextPage()
        {
            CurrentPage.NavigateToNextPage();
        }

        public void NavigateToPrevPage()
        {
            CurrentPage.NavigateToPrevPage();
        }

        #endregion Navigation Functions

    }
}