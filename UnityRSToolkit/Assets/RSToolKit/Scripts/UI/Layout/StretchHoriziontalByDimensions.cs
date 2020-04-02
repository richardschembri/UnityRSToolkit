namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using RSToolkit.Helpers;

    public class StretchHoriziontalByDimensions : AdjustByScreenDimensions  {

        public StretchHoriziontalByDimensionsSettings[] settings; 
        // Use this for initialization
        public override void Adjust(){
            for(int i = 0; i < settings.Length; i++){
                var presets = GetPresetScreenDimensions(settings[i].ScreenDimensionsType, settings[i].OtherScreenDimensions);
                if(presets.Any( p => IsDimensions(p))){
                    if(settings[i].RectHeight > 0){
                        var sd = this.GetComponent<RectTransform>().sizeDelta;
                        this.GetComponent<RectTransform>().sizeDelta = new Vector2(sd.x, settings[i].RectHeight);
                    }
                    var ap = this.GetComponent<RectTransform>().anchoredPosition;
                    this.GetComponent<RectTransform>().anchoredPosition = new Vector2(ap.x , settings[i].PosY);
                    this.GetComponent<RectTransform>().SetStretch_Left(settings[i].Left);
                    this.GetComponent<RectTransform>().SetStretch_Right(settings[i].Right);
                    m_adjusted = true;
                    break;
                }
            }
            /*
            var ordShdByScreenDimensions = settings.OrderBy(shdv => shdv.ResolutionOrAspectRatio).ToList();
            for (int i = 0; i < ordShdByScreenDimensions.Count(); i++){
                var shd = ordShdByScreenDimensions[i];
                if(IsDimensions(shd.ResolutionOrAspectRatio, shd.Width, shd.Height)){
                    if(shd.RectHeight > 0){
                        var sd = this.GetComponent<RectTransform>().sizeDelta;
                        this.GetComponent<RectTransform>().sizeDelta = new Vector2(sd.x, shd.RectHeight);
                    }
                    var ap = this.GetComponent<RectTransform>().anchoredPosition;
                    // this.GetComponent<RectTransform>().SetAnchor(RectTransformHelpers.AnchorPresets.HorStretchMiddle);
                    this.GetComponent<RectTransform>().anchoredPosition = new Vector2(ap.x , shd.PosY);
                    this.GetComponent<RectTransform>().SetStretch_Left(shd.Left);
                    this.GetComponent<RectTransform>().SetStretch_Right(shd.Right);
                    m_adjusted = true;
                }
            }
            */
        }
    }

    [System.Serializable]
    public struct StretchHoriziontalByDimensionsSettings {
        public AdjustByScreenDimensions.ResolutionAspectType ScreenDimensionsType;
        public ScreenDimensions OtherScreenDimensions; 
        public float Left;
        public float Right;
        public float PosY;
        public float RectHeight;
    }
}