namespace RSToolkit.UI.Paging
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Events;
    public class UIPage : MonoBehaviour
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

        public KeyCode NavigateShortCut;

        public bool ShowCursor = true;

        public KeyCode NavigateToNextPageShortCut = KeyCode.None;
        public UIPage PrevPage;
        public UIPage NextPage;

        public class OnNavigatedToEvent : UnityEvent<UIPage, bool> {}
        public OnNavigatedToEvent OnNavigatedTo = new OnNavigatedToEvent();
        public class OnNavigatedFromEvent : UnityEvent<UIPage> { }
        public OnNavigatedFromEvent OnNavigatedFrom = new OnNavigatedFromEvent();

        #region Unity Event
        // Start is called before the first frame update
        protected virtual void Awake()
        {
            OnNavigatedTo.AddListener(onNavigatedTo);
            OnNavigatedFrom.AddListener(onNavigatedFrom);
        }
        protected virtual void OnEnable()
        {
            Cursor.visible = ShowCursor;
        }
        #endregion
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