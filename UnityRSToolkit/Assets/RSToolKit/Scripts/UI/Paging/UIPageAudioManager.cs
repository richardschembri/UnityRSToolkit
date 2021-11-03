namespace RSToolkit.UI.Paging
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UIPageAudioManager : RSSingletonMonoBehaviour<UIPageAudioManager>
    {
        public AudioClip DefaultNavigationAudioClip;
        public AudioClip DefaultBGMAudioClip;
        public AudioSource NavigateAudioSource;
        public AudioSource BGMAudioSource;

        [SerializeField]
        private UIPageManager _UIPageManagerInstance;

        public bool _skipNavigationSound = false;

        public void SkipNavigationSound(){
            _skipNavigationSound = true;
        }

        #region MonoBehaviour Functions 
        protected virtual void Start()
        {
            StartCoroutine(Init());
        }
        #endregion MonoBehaviour Functions 

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
            if(!_skipNavigationSound){
                PlayPageAudio(NavigateAudioSource, pageAudio.NavigationAudioClip, DefaultNavigationAudioClip);
            }
            _skipNavigationSound = false;
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
                (pageAudio.IgnoreSelfNavigation && page != _UIPageManagerInstance.Core.PreviousPage)
            );
        }
        #endregion Page Events
       
    }
}

