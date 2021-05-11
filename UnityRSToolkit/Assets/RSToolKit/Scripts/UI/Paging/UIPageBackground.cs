using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.UI.Paging
{
    public class UIPageBackground : RSSingletonMonoBehaviour<UIPageBackground>
    {
        public Sprite DefaultBackgroundSprite;
        public Image TargetGraphic;

        public bool SetNativeSize = false;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            yield return new WaitUntil(() => UIPageManager.Instance != null && UIPageManager.Instance.Initialized);
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