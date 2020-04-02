namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;

    public class RawImageByScreenDimensions : AdjustByScreenDimensions{

        public RawImageForScreenDimensionsSettings[] settings; 
        public bool SetNativeSize = false;

        public override void Adjust(){
            for(int i = 0; i < settings.Length; i++){
                var presets = GetPresetScreenDimensions(settings[i].ScreenDimensionsType, settings[i].OtherScreenDimensions);
                if(presets.Any( p => IsDimensions(p))){
                    this.GetComponent<RawImage>().texture = settings[i].rawImageTexture;
                    m_adjusted = true;
                    if(SetNativeSize){
                        this.GetComponent<RawImage>().SetNativeSize();
                    }
                    break;
                }
            }
            /*
            var ordScreenDimensions = settings.OrderBy(sd => sd.ResolutionOrAspectRatio).ToList();

            for (int i = 0; i < ordScreenDimensions.Count(); i++){
                var sd = ordScreenDimensions[i];

                if(IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height)){
                    this.GetComponent<RawImage>().texture = sd.rawImageTexture;
                    m_adjusted = true;
                    if(SetNativeSize){
                        this.GetComponent<RawImage>().SetNativeSize();
                    }
                }
            }
            */
        }
    }

    [System.Serializable]
    public struct RawImageForScreenDimensionsSettings {

        public AdjustByScreenDimensions.ResolutionAspectType ScreenDimensionsType;
        public ScreenDimensions OtherScreenDimensions; 

        public Texture2D rawImageTexture;
    }
}