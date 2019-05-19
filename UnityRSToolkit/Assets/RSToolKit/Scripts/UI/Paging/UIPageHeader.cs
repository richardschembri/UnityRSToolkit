namespace RSToolkit.UI.Paging
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPageHeader : MonoBehaviour
    {
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
                if (page.DisplayHeader)
                {
                    page.OnNavigatedTo.AddListener(onNavigatedTo);
                    if (page.LaunchPage)
                    {
                        SetHeader(page);
                    }
                }
                
            }
        }

        public void SetHeader(UIPage page)
        {
            var text = page.PageHeader;
            if (string.IsNullOrEmpty(text))
            {
                text = page.gameObject.name;
            }

            this.gameObject.GetComponentInChildren<Text>().text = text;

        }
        #region Page Events
        void onNavigatedTo(UIPage page, bool keepCache)
        {
            SetHeader(page);
        }
        #endregion Page Events
    }
}
