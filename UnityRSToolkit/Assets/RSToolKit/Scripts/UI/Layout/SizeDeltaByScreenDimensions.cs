namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    // DimensionsByScreenDimensions
    public class SizeDeltaByScreenDimensions : AdjustByScreenDimensions
    {
        public SizeDeltaByScreenDimensionsSettings[] settings;

        // public void SetDimensions(){
        public override void Adjust(){
            for(int i = 0; i < settings.Length; i++){
                var presets = GetPresetScreenDimensions(settings[i].ScreenDimensionsType, settings[i].OtherScreenDimensions);
                if(presets.Any( p => IsDimensions(p))){
                    this.GetComponent<RectTransform>().sizeDelta = settings[i].newSizeDelta;
                    m_adjusted = true;
                    break;
                }
            }
            /*
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
            */
        }

    }

    [System.Serializable]
    public struct SizeDeltaByScreenDimensionsSettings  {
        public AdjustByScreenDimensions.ResolutionAspectType ScreenDimensionsType;
        public ScreenDimensions OtherScreenDimensions; 

        public Vector2 newSizeDelta;
    }

}