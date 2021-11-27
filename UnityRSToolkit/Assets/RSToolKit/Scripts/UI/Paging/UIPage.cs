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
        protected string PageHeader = "";
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

        public UIPageManagerCore UIPageManagerCoreInstance{get; private set;}

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

        public void SetUIPageManagerCore(UIPageManagerCore target){
            UIPageManagerCoreInstance = target;
        }
        public bool IsCurrentPage()
        {
            return UIPageManagerCoreInstance.IsCurrentPage(this);
        }

        public string GetHeader(){
            return string.IsNullOrEmpty(PageHeader) ? name : PageHeader;
        }

        public void Close(){
            gameObject.SetActive(false);
        }

        #region Page Events
        public void NavigateToPrevPage()
        {
            NavigateToPrevPage(false);
        }
        public void NavigateToPrevPage(bool keepCache)
        {
            UIPageManagerCoreInstance.NavigateTo(PrevPage, keepCache);
        }

        public void NavigateToNextPage()
        {
            NavigateToNextPage(false);
        }
        public void NavigateToNextPage(bool keepCache)
        {
            UIPageManagerCoreInstance.NavigateTo(NextPage, keepCache);
        }

        public void NavigateTo(bool keepCache = false)
        {
            UIPageManagerCoreInstance.NavigateTo(this, keepCache);
        }
        #endregion

        protected virtual void onNavigatedTo(UIPage page, bool keepCache){}
        protected virtual void onNavigatedFrom(UIPage page){} 
    }

}