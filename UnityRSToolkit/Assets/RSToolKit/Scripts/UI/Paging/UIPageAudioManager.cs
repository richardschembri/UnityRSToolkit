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

        public bool m_skipNavigationSound = false;

        public void SkipNavigationSound(){
            m_skipNavigationSound = true;
        } 

        private static UIPageAudioManager m_instance;

        public static UIPageAudioManager Instance{
            get{ return m_instance; }
        }
        
        protected virtual void Start()
        {
            StartCoroutine(Init());
        }

        protected virtual void Awake(){
            m_instance = this;
        }

        IEnumerator Init()
        {
            yield return new WaitUntil(() => UIPageManager.Instance.InitComplete);
            for (int i = 0; i < UIPageManager.Instance.Pages.Length; i++)
            {
                var page = UIPageManager.Instance.Pages[i];
                if(page.GetComponent<UIPageAudio>() != null){
                    page.OnNavigatedTo.AddListener(onNavigatedTo);
                }
            }
        }
        void PlayPageAudio(AudioSource source, AudioClip pageAudioClip, AudioClip defaultClip)
        {
            source.Stop();

            if (pageAudioClip != null)
            {
                source.clip = pageAudioClip;
            } else if (defaultClip != null){
                source.clip = defaultClip;
            }

            if (source.clip != null)
            {
                source.Play();
            }
        }

        public void PlayPageAudio(UIPage page)
        {
            if(!canPlayAudio(page)){
                return;
            }
            var pageAudio = page.GetComponent<UIPageAudio>();
            if(!m_skipNavigationSound){
                PlayPageAudio(NavigateAudioSource, pageAudio.NavigationAudioClip, DefaultNavigationAudioClip);
            }
            m_skipNavigationSound = false;
            PlayPageAudio(BGMAudioSource, pageAudio.BGMAudioClip, DefaultBGMAudioClip);
        }

        #region Page Events


        void onNavigatedTo(UIPage page, bool keepCache)
        {
            PlayPageAudio(page);
        }

        bool canPlayAudio(UIPage page){
            var pageAudio = page.GetComponent<UIPageAudio>();
            return (!pageAudio.IgnoreSelfNavigation || 
                (pageAudio.IgnoreSelfNavigation && page != UIPageManager.Instance.PreviousPage)
            );
        }
        #endregion Page Events
       
    }
}

