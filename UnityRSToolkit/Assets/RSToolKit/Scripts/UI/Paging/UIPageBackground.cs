namespace RSToolkit.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class UIPageBackground : MonoBehaviour
    {
        public Sprite DefaultBackgroundTexture;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Init());
        }

        // Update is called once per frame
        void Update()
        {

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

        public void SetBackground(UIPage page)
        {
            var pageBackground = this.GetComponent<Image>();
            if (page.BackgroundImage != null)
            {
                pageBackground.sprite = page.BackgroundImage;
            }
            else
            {
                pageBackground.sprite = DefaultBackgroundTexture;
            }

            pageBackground.color = page.BackgroundColor;
            //pageBackground.SetNativeSize();
        }

        #region Page Events

        void onNavigatedTo(UIPage page, bool keepCache)
        {
            SetBackground(page);
        }

        #endregion Page Events
    }
}