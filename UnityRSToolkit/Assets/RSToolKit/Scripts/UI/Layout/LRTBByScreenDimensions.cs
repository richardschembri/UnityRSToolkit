namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using RSToolkit.Helpers;

    public class LRTBByScreenDimensions : AdjustByScreenDimensions
    {
        public LRTBByScreenDimensionsSettings[] settings; 

        // Use this for initialization
        public override void Adjust(){

            var ordLRTBByScreenDimensions = settings.OrderBy(psd => psd.ResolutionOrAspectRatio).ToList();
            for (int i = 0; i < ordLRTBByScreenDimensions.Count(); i++){
                var sd = ordLRTBByScreenDimensions[i];

                if(IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height)){
                    //this.GetComponent<RectTransform>().offsetMin = new Vector2(sd.NewLeft, sd.NewBottom); // new Vector2(left, bottom); 
                    this.GetComponent<RectTransform>().SetStretch_LeftBottom(sd.NewLeft, sd.NewBottom);
                    //this.GetComponent<RectTransform>().offsetMax = new Vector2(-sd.NewRight, -sd.NewTop); // new Vector2(-right, -top);	
                    this.GetComponent<RectTransform>().SetStretch_RightTop(sd.NewRight, sd.NewTop);
                    m_adjusted = true;
                }
            }
        }
    }

    [System.Serializable]
    public struct LRTBByScreenDimensionsSettings {

        public bool ResolutionOrAspectRatio;
        public float Width;
        public float Height;

        public float NewTop;
        public float NewBottom;
        public float NewLeft;
        public float NewRight;
    }
}