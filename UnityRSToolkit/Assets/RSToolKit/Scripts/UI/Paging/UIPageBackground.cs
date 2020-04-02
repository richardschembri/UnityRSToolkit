namespace RSToolkit.UI.Paging
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPageBackground : MonoBehaviour
    {
        public Sprite DefaultBackgroundSprite;
        public Image TargetGraphic;

        public bool SetNativeSize = false;
        private static UIPageBackground m_instance;
        public static UIPageBackground Instance{
            get{ return m_instance; }
        }
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Init());
        }

        // Update is called once per frame
        void Awake()
        {
            m_instance = this;
        }

        IEnumerator Init()
        {
            yield return new WaitUntil(() => UIPageManager.Instance.InitComplete);
            for (int i = 0; i < UIPageManager.Instance.Pages.Length; i++)
            {
                var page = UIPageManager.Instance.Pages[i];
                page.OnNavigatedTo.AddListener(onNavigatedTo);
                if (page.LaunchPage)
                {
                    SetBackground(page);
                }
            }
        }

        private void SetBackground(UIPage page)
        {
            TargetGraphic.gameObject.SetActive(page.DisplayBackground);
            if(!page.DisplayBackground){
                return;
            }
            
            if (page.BackgroundImage != null)
            {
                TargetGraphic.sprite = page.BackgroundImage;
            }
            else
            {
                TargetGraphic.sprite = DefaultBackgroundSprite;
            }

            TargetGraphic.color = page.BackgroundColor;
            if(SetNativeSize){
                TargetGraphic.SetNativeSize();
            }
            
        }

        #region Page Events

        void onNavigatedTo(UIPage page, bool keepCache)
        {
            SetBackground(page);
        }

        #endregion Page Events
    }
}