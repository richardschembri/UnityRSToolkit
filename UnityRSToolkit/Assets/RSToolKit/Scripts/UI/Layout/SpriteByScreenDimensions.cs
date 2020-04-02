namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class SpriteByScreenDimensions : AdjustByScreenDimensions {
        public Sprite defaultsprite;
        private Sprite m_sprite;
        public SpriteByScreenDimensionsSettings[] settings; 

        // Use this for initialization
        public override void Adjust(){
            for(int i = 0; i < settings.Length; i++){
                var presets = GetPresetScreenDimensions(settings[i].ScreenDimensionsType, settings[i].OtherScreenDimensions);
                if(presets.Any( p => IsDimensions(p))){
                    m_sprite = settings[i].sprite;
                    m_adjusted = true;
                    break;
                }
            }
            /*
            var ordScreenDimensions = settings.OrderBy(sd => sd.ResolutionOrAspectRatio).ToList();

            for (int i = 0; i < ordScreenDimensions.Count(); i++){
                var sd = ordScreenDimensions[i];

                if(IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height)){
                    m_sprite = sd.sprite;
                    m_adjusted = true;
                }
            }
            */
        }

        public Sprite GetSprite(){

            if(!m_adjusted){
                Adjust();
            }
            if(m_sprite == null){
                m_sprite = defaultsprite;
            }
            return m_sprite;
        }
    }

    [System.Serializable]
    public struct SpriteByScreenDimensionsSettings {

        public AdjustByScreenDimensions.ResolutionAspectType ScreenDimensionsType;
        public ScreenDimensions OtherScreenDimensions; 

        public Sprite sprite;
    }
}