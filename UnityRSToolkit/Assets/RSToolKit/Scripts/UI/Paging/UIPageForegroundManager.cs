namespace RSToolkit.UI.Paging
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UIPageForegroundManager : MonoBehaviour
    {
        private UIPageHeader m_uiPageHeader;
        private UIPageHeader m_UIPageHeader{
            get{
                if(m_uiPageHeader == null){
                    m_uiPageHeader = this.GetComponentInChildren<UIPageHeader>();
                }
                return m_uiPageHeader;
            }
        }

        IEnumerator Init()
        {
            yield return new WaitUntil(() => UIPageManager.Instance != null && UIPageManager.Instance.Initialized); //InitComplete);
            for (int i = 0; i < UIPageManager.Instance.Pages.Length; i++)
            {
                var page = UIPageManager.Instance.Pages[i];
                page.OnNavigatedTo.AddListener(onNavigatedTo);
                
            }

            SetHeader(UIPageManager.Instance.CurrentPage);
        }
        // Start is called before the first frame update
        void Start()
        {
           StartCoroutine(Init()); 
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        public void SetHeader(UIPage page)
        {
            if(m_UIPageHeader != null){
                if(page.DisplayHeader){
                    m_UIPageHeader.SetHeaderText(page); 
                }
                m_UIPageHeader.gameObject.SetActive(page.DisplayHeader);
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