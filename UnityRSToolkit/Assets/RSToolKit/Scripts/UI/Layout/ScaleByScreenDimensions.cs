namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class ScaleByScreenDimensions : AdjustByScreenDimensions
    {
        public ScaleByScreenDimensionsSettings[] settings; 

    /* 
        void OnEnable()
        {
            SetScale();
        }
    */
        // public void SetScale(){
        public override void Adjust(){
            for(int i = 0; i < settings.Length; i++){
                var presets = GetPresetScreenDimensions(settings[i].ScreenDimensionsType, settings[i].OtherScreenDimensions);
                if(presets.Any( p => IsDimensions(p))){
                    this.transform.localScale = settings[i].Scale;
                    _adjusted = true;
                    break;
                }
            }
            /*
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
            */
        }

        /*
        bool CheckResolution(ScaleByScreenDimensionsSettings ssd){
            return (ssd.Width == Screen.width && ssd.Height == Screen.height); 
        }

        bool CheckAspectRatio(ScaleByScreenDimensionsSettings ssd){
            return (((float)Screen.width / (float)Screen.height) == (ssd.Width / ssd.Height));
        }
        */
    }

    [System.Serializable]
    public struct ScaleByScreenDimensionsSettings {
        public AdjustByScreenDimensions.ResolutionAspectType ScreenDimensionsType;
        public ScreenDimensions OtherScreenDimensions; 
        public Vector3 Scale;
    }
}