namespace RSToolkit.UI.Paging
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UIPageAudioManager : MonoBehaviour
    {
        public AudioClip DefaultNavigationAudioClip;
        public AudioClip DefaultBGMAudioClip;
        public AudioSource NavigateAudioSource;
        public AudioSource BGMAudioSource;
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
            }
        }
        void PlayPageAudio(AudioSource source, UIPageAudio pageAudio, AudioClip defaultClip)
        {
            source.Stop();
            if (pageAudio == null)
            {
                return;
            }

            if (!pageAudio.UseDefault)
            {
                source.clip = pageAudio.PageAudioClip;
            }
            else
            {
                source.clip = defaultClip;
            }
            source.Play();
        }

        public void PlayPageAudio(UIPage page)
        {
            if(!CanPlayAudio(page)){
                return;
            }
            PlayPageAudio(NavigateAudioSource, page.GetComponent<UIPageNavigationAudio>(), DefaultNavigationAudioClip);
            PlayPageAudio(BGMAudioSource, page.GetComponent<UIPageBGMAudio>(), DefaultBGMAudioClip);
        }

        #region Page Events


        void onNavigatedTo(UIPage page, bool keepCache)
        {
            PlayPageAudio(page);
        }

        bool CanPlayAudio(UIPage page){
            var pageAudio = page.GetComponent<UIPageNavigationAudio>();
            return pageAudio != null && (!pageAudio.IgnoreSelfNavigation || 
                (pageAudio.IgnoreSelfNavigation && page != UIPageManager.Instance.PreviousPage)
            );
        }
        #endregion Page Events
       
    }
}

