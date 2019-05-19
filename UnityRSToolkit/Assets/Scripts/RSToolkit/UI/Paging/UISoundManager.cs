namespace RSToolkit.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UISoundManager : MonoBehaviour
    {
        public AudioSource DefaultNavigateSound;
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
            for(int i = 0; i < UIPageManager.Instance.Pages.Length; i++)
            {
                var page = UIPageManager.Instance.Pages[i];
                if (DefaultNavigateSound != null || page.NavigationSound != null)
                {
                    page.OnNavigatedTo.AddListener(onNavigatedTo);
                }         
            }
        }

        public void PlayNavigationSound(UIPage page)
        {
            if (page.NavigationSound != null)
            {
                page.NavigationSound.Play();
            }
            else if (DefaultNavigateSound != null)
            {
                DefaultNavigateSound.Play();
            }
 
        }

        #region Page Events

        
        void onNavigatedTo(UIPage page, bool keepCache)
        {
            PlayNavigationSound(page);
        }
        #endregion Page Events
    }
}