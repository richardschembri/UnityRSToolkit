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

            for(int i = 0; i < settings.Length; i++){
                var presets = GetPresetScreenDimensions(settings[i].ScreenDimensionsType, settings[i].OtherScreenDimensions);
                if(presets.Any( p => IsDimensions(p))){
                    this.GetComponent<RectTransform>().SetStretch_LeftBottom(settings[i].NewLeft, settings[i].NewBottom);
                    this.GetComponent<RectTransform>().SetStretch_RightTop(settings[i].NewRight, settings[i].NewTop);
                    m_adjusted = true;
                    break;
                }
            }

            /*
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
            */
        }
    }

    [System.Serializable]
    public struct LRTBByScreenDimensionsSettings {

        public AdjustByScreenDimensions.ResolutionAspectType ScreenDimensionsType;
        public ScreenDimensions OtherScreenDimensions; 

        public float NewTop;
        public float NewBottom;
        public float NewLeft;
        public float NewRight;
    }
}