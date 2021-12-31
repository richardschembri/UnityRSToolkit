using System.Linq;
using UnityEngine;
using RSToolkit.Collections;
using RSToolkit.Helpers;
    
namespace RSToolkit.UI.Paging
{
    public class UIPageManager : RSMonoBehaviour//RSSingletonMonoBehaviour<UIPageManager> // MonoBehaviour
    {

        #region Fields

        public KeyCode ExitKey = KeyCode.Escape;    

        [SerializeField]
        private bool _isSingleton = true;
        public bool IsSingleton {get{return _isSingleton;}}
        public UIPageManagerCore Core {get; protected set;}

        public static UIPageManager Instance { get; protected set; }

        #endregion

        #region Properties

        public UIPage[] CollectPages() {
            return transform.GetTopLevelChildrenEnumerable<UIPage>()
                        .OrderBy(p => p.SortOrder).ToArray();
        }

        private void HandleShortcuts()
        {
           for (int i = 0; i < Core.Pages.Length; i++){
               if(Input.GetKeyUp(Core.Pages[i].NavigateShortCut)){
                   Core.NavigateTo(Core.Pages[i]);
                   return;
               }
           } 
            
           if(Input.GetKeyUp(Core.CurrentPage.NavigateToNextPageShortCut)){
               Core.CurrentPage.NavigateToNextPage();
            }
            else if(Input.GetKeyUp(Core.CurrentPage.NavigateToPrevPageShortCut))
            {
                if(Core.CurrentPage.PrevPage != null)
                {
                    Core.CurrentPage.NavigateToPrevPage();
                }
                else
                {
                    Core.NavigateBack(true);
                }
            }


        }

        #endregion
        #region MonoBehaviour Functions

        protected override void Awake()
        {
            if (Instance != null && Instance != this && IsSingleton)
            {
                Destroy(this);
                throw new System.Exception("An instance of this singleton already exists.");
            }
            else if(IsSingleton)
            {
                Instance = this;
            }
            base.Awake();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
           if (Input.GetKeyUp(ExitKey)){
               Application.Quit();
           }

            HandleShortcuts();

        }
        #endregion MonoBehaviour Functions


        #region RSMonoBehaviour Functions

        protected virtual void InitCore()
        {
            Core = new UIPageManagerCore(CollectPages());
        }
        public override bool Init(bool force = false)
        {
            if (base.Init(force))
            {
                InitCore();
                return true;
            }
            return false;

        }
        #endregion RSMonoBehaviour Functions


    }

}