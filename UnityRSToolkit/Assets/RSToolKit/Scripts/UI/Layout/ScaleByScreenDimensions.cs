namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class ScaleByScreenDimensions : MonoBehaviour
    {
        public ScaleByScreenDimensionsSettings[] settings; 
        // Use this for initialization
        void Start () {
            SetScale();
        }
        
        // Update is called once per frame
        void Update () {
            
        }

        void Awake(){
            SetScale();
        }

    /* 
        void OnEnable()
        {
            SetScale();
        }
    */
        public void SetScale(){
            var ordSclByScreenDimensions = settings.OrderBy(ssd => ssd.ResolutionOrAspectRatio).ToList();
            for (int i = 0; i < ordSclByScreenDimensions.Count(); i++){
                var ssd = ordSclByScreenDimensions[i];
                bool is_match = false;
                if(ssd.ResolutionOrAspectRatio){
                    is_match = CheckResolution(ssd);
                }else{
                    is_match = CheckAspectRatio(ssd);
                }

                if(is_match){
                    this.transform.localScale = ssd.Scale;
                }
            }
        }

        bool CheckResolution(ScaleByScreenDimensionsSettings ssd){
            return (ssd.Width == Screen.width && ssd.Height == Screen.height); 
        }

        bool CheckAspectRatio(ScaleByScreenDimensionsSettings ssd){
            return (((float)Screen.width / (float)Screen.height) == (ssd.Width / ssd.Height));
        }
    }

    [System.Serializable]
    public struct ScaleByScreenDimensionsSettings {
        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;
        public Vector3 Scale;
    }
}