    using System.Collections;
    using UnityEngine;

namespace RSToolkit.UI.Paging
{
    public class UIPageForegroundManager : MonoBehaviour
    {
        private UIPageHeader _uiPageHeader;
        private UIPageHeader _UIPageHeader{
            get{
                if(_uiPageHeader == null){
                    _uiPageHeader = this.GetComponentInChildren<UIPageHeader>();
                }
                return _uiPageHeader;
            }
        }

        [SerializeField]
        private UIPageManager _UIPageManagerInstance;

        IEnumerator Init()
        {
            if(_UIPageManagerInstance == null){
                yield return new WaitUntil(() => UIPageManager.Instance != null); //InitComplete);
                _UIPageManagerInstance = UIPageManager.Instance;
            }
            yield return new WaitUntil(() => _UIPageManagerInstance.Initialized); //InitComplete);
            for (int i = 0; i < _UIPageManagerInstance.Core.Pages.Length; i++)
            {
                var page = _UIPageManagerInstance.Core.Pages[i];
                page.OnNavigatedTo.AddListener(onNavigatedTo);
                
            }

            SetHeader(_UIPageManagerInstance.Core.CurrentPage);
        }
        // Start is called before the first frame update
        void Start()
        {
           StartCoroutine(Init()); 
        }

        public void SetHeader(UIPage page)
        {
            if(_UIPageHeader != null){
                if(page.DisplayHeader){
                    _UIPageHeader.SetHeaderText(page); 
                }
                _UIPageHeader.gameObject.SetActive(page.DisplayHeader);
            }
        }
        #region Page Events
        void onNavigatedTo(UIPage page, bool keepCache)
        {
            SetHeader(page);
        }
        #endregion Page Events
    }
}