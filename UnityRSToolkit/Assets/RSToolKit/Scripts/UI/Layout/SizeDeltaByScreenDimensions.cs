namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    // DimensionsByScreenDimensions
    public class SizeDeltaByScreenDimensions : MonoBehaviour
    {
        public SizeDeltaByScreenDimensionsSettings[] settings;
        // Use this for initialization
        void Start () {
            SetDimensions();
        }
        
        // Update is called once per frame
        void Update () {
            
        }

        void Awake(){
            SetDimensions();
        }

        public void SetDimensions(){
            var ordDmnByScreenDimensions = settings.OrderBy(psd => psd.ResolutionOrAspectRatio).ToList();
            for (int i = 0; i < ordDmnByScreenDimensions.Count(); i++){
                var dsd = ordDmnByScreenDimensions[i];
                bool is_match = false;
                if(dsd.ResolutionOrAspectRatio){
                    is_match = CheckResolution(dsd);
                }else{
                    is_match = CheckAspectRatio(dsd);
                }

                if(is_match){
                    //this.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleCenter);
                    this.GetComponent<RectTransform>().sizeDelta = dsd.newSizeDelta;
                }
            }
        }

        bool CheckResolution(SizeDeltaByScreenDimensionsSettings settings){
            return (settings.Width == Screen.width && settings.Height == Screen.height); 
        }

        bool CheckAspectRatio(SizeDeltaByScreenDimensionsSettings settings){
            return (((float)Screen.width / (float)Screen.height) == (settings.Width / settings.Height));
        }

    }

    [System.Serializable]
    public struct SizeDeltaByScreenDimensionsSettings  {
        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;

        public Vector2 newSizeDelta;
    }

}