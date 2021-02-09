namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;

    [RequireComponent(typeof(Text))]
    public class FontByScreenDimensions : AdjustByScreenDimensions
    {

        public FontByScreenDimensionsSettings[] settings; 

        public override void Adjust(){

            for(int i = 0; i < settings.Length; i++){
                var presets = GetPresetScreenDimensions(settings[i].ScreenDimensionsType, settings[i].OtherScreenDimensions);
                if(presets.Any( p => IsDimensions(p))){
                    this.GetComponent<Text>().fontSize = settings[i].NewFontSize;
                    _adjusted = true;
                    break;
                }
            }
            /*
            var ordLRTBByScreenDimensions = settings.OrderBy(psd => psd.ResolutionOrAspectRatio).ToList();
            for (int i = 0; i < ordLRTBByScreenDimensions.Count(); i++){
                var sd = ordLRTBByScreenDimensions[i];

                if(IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height)){
                    this.GetComponent<Text>().fontSize = sd.NewFontSize;
                    m_adjusted = true;
                }
            }
            */
        }

    }

    [System.Serializable]
    public struct FontByScreenDimensionsSettings {
        public AdjustByScreenDimensions.ResolutionAspectType ScreenDimensionsType;
        public ScreenDimensions OtherScreenDimensions; 
        public int NewFontSize;
    }
}
