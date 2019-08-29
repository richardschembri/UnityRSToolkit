namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class TextSizeByScreenDimensions : MonoBehaviour
    {
        public TextSizeByScreenDimensionsSettings[] settings;

        // Use this for initialization
        void Start () {
            SetTextSize();
        }
        
        // Update is called once per frame
        void Update () {
            
        }
        public void SetTextSize(){
            var ordTxtSizeByScreenDimensions = settings.OrderBy(ssd => ssd.ResolutionOrAspectRatio).ToList();
            for (int i = 0; i < ordTxtSizeByScreenDimensions.Count(); i++){
                var tssd = ordTxtSizeByScreenDimensions[i];
                bool is_match = false;
                if(tssd.ResolutionOrAspectRatio){
                    is_match = CheckResolution(tssd);
                }else{
                    is_match = CheckAspectRatio(tssd);
                }

                if(is_match){
                    GetComponent<Text>().fontSize = tssd.fontSize;
                }
            }
        }
        bool CheckResolution(TextSizeByScreenDimensionsSettings tssd){
            return (tssd.Width == Screen.width && tssd.Height == Screen.height); 
        }

        bool CheckAspectRatio(TextSizeByScreenDimensionsSettings tssd){
            return (((float)Screen.width / (float)Screen.height) == (tssd.Width / tssd.Height));
        }
    }

    [System.Serializable]
    public struct TextSizeByScreenDimensionsSettings 
    {
        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;

        public int fontSize;

    }
}
