namespace RSToolkit.UI
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
        public string PageHeader = "";
        public bool LaunchPage = false;
        public Sprite BackgroundImage;
        public Color BackgroundColor = Color.white;
        public AudioSource NavigationSound;
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
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void OnEnable()
        {
            Cursor.visible = ShowCursor;
        }
        #endregion
        public bool IsCurrentPage()
        {
            return UIPageManager.Instance.IsCurrentPage(this);
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

        public void NavigateTo()
        {
            NavigateTo(false);
        }
        public void NavigateTo(bool keepCache)
        {
            UIPageManager.Instance.NavigateTo(this, keepCache);
        }
        #endregion
    }

}