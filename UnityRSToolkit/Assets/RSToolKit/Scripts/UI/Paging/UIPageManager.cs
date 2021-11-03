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
        public UIPageManagerCore Core {get; private set;}

        public static UIPageManager Instance { get; protected set; }

        #endregion

        #region Properties

        public UIPage[] CollectPages() {
            return transform.GetTopLevelChildrenEnumerable<UIPage>()
                        .OrderBy(p => p.SortOrder).ToArray();
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

           for (int i = 0; i < Core.Pages.Length; i++){
               if(Input.GetKeyUp(Core.Pages[i].NavigateShortCut)){
                   Core.NavigateTo(Core.Pages[i]);
               }
           } 
        }
        #endregion MonoBehaviour Functions


        #region RSMonoBehaviour Functions
        public override bool Init(bool force = false)
        {
            if (base.Init(force))
            {
                Core = new UIPageManagerCore(CollectPages());
                return true;
            }
            return false;

        }
        #endregion RSMonoBehaviour Functions


    }

}