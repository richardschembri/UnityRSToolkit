namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;

    public class ImageByScreenDimensions : AdjustByScreenDimensions
    {
        public ImageByScreenDimensionsSettings[] settings;
        public override void Adjust(){
            for(int i = 0; i < settings.Length; i++){
                var presets = GetPresetScreenDimensions(settings[i].ScreenDimensionsType, settings[i].OtherScreenDimensions);
                if(presets.Any( p => IsDimensions(p))){
                    this.GetComponent<Image>().sprite = settings[i].imageSprite;
                    this.GetComponent<Image>().preserveAspect = settings[i].preserveAspect;
                    m_adjusted = true;
                    break;
                }
            }

            /*
            var ordScreenDimensions = settings.OrderBy(sd => sd.ResolutionOrAspectRatio).ToList();
            for (int i = 0; i < ordScreenDimensions.Count(); i++){
                var sd = ordScreenDimensions[i];

                if(IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height)){
                    this.GetComponent<Image>().sprite = sd.imageSprite;
                    this.GetComponent<Image>().preserveAspect = sd.preserveAspect;
                }
            }
            */
        }
    }

    [System.Serializable]
    public struct ImageByScreenDimensionsSettings {

        public AdjustByScreenDimensions.ResolutionAspectType ScreenDimensionsType;
        public ScreenDimensions OtherScreenDimensions; 

        public Sprite imageSprite;

        public bool preserveAspect;
    }
}