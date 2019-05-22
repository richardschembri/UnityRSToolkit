namespace RSToolkit.UI.Paging
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Image))]
    public class UIPageBackground : MonoBehaviour
    {
        public Sprite DefaultBackgroundTexture;
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