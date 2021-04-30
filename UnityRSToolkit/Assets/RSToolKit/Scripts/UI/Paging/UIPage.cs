using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.UI.Paging
{
    public class UIPage : RSMonoBehaviour
    {
        public bool ShowInMenu = true;
        public bool DisplayHeader = true;
        public bool DisplayMenu = true;
        public int SortOrder = 0;
        [SerializeField]
        private string PageHeader = "";
        public bool LaunchPage = false;
        public bool DisplayBackground = true;
        public Sprite BackgroundImage;
        public Color BackgroundColor = Color.white;

        public bool ShowCursor = true;

        [Header("Shortcuts")]
        public KeyCode NavigateShortCut;
        public KeyCode NavigateToNextPageShortCut = KeyCode.None;

        [Header("Page Navigation")]
        public UIPage PrevPage;
        public UIPage NextPage;

        public class OnNavigatedToEvent : UnityEvent<UIPage, bool> {}
        [Header("Events")]
        public OnNavigatedToEvent OnNavigatedTo = new OnNavigatedToEvent();
        public class OnNavigatedFromEvent : UnityEvent<UIPage> { }
        public OnNavigatedFromEvent OnNavigatedFrom = new OnNavigatedFromEvent();


        #region RSMonoBehaviour Functions
        protected override void InitEvents()
        {
            base.InitEvents();

            OnNavigatedTo.AddListener(onNavigatedTo);
            OnNavigatedFrom.AddListener(onNavigatedFrom);
        }
        #endregion RSMonoBehaviour Functions

        #region MonoBehaviour Functions
        protected virtual void OnEnable()
        {
            Cursor.visible = ShowCursor;
        }
        #endregion MonoBehaviour Functions
        public bool IsCurrentPage()
        {
            return UIPageManager.Instance.IsCurrentPage(this);
        }

        public string GetHeader(){
            return string.IsNullOrEmpty(PageHeader) ? name : PageHeader;
        }

        #region Page Events
        public void NavigateToPrevPage()
        {
            NavigateToPrevPage(false);
        }
        public void NavigateToPrevPage(bool keepCache)
        {
            UIPageManager.Instance.NavigateTo(PrevPage, keepCache);
        }

        public void NavigateToNextPage()
        {
            NavigateToNextPage(false);
        }
        public void NavigateToNextPage(bool keepCache)
        {
            UIPageManager.Instance.NavigateTo(NextPage, keepCache);
        }

        public void NavigateTo(bool keepCache = false)
        {
            UIPageManager.Instance.NavigateTo(this, keepCache);
        }
        #endregion

        protected virtual void onNavigatedTo(UIPage page, bool keepCache){}
        protected virtual void onNavigatedFrom(UIPage page){} 
    }

}