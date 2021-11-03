using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RSToolkit.UI.Paging
{
    public class UIPageBackground : RSMonoBehaviour
    {
        public Sprite DefaultBackgroundSprite;
        public Image TargetGraphic;

        [SerializeField]
        private UIPageManager _UIPageManagerInstance;

        public bool SetNativeSize = false;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Init());
        }

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