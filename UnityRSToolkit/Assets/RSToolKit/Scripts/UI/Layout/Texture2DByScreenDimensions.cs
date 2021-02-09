namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class Texture2DByScreenDimensions : AdjustByScreenDimensions {

        public Texture2D defaulttexture2D;
        private Texture2D m_texture2D;
        public Texture2DScreenDimensionsSettings[] settings; 

        // Use this for initialization
        public override void Adjust(){
            for(int i = 0; i < settings.Length; i++){
                var presets = GetPresetScreenDimensions(settings[i].ScreenDimensionsType, settings[i].OtherScreenDimensions);
                if(presets.Any( p => IsDimensions(p))){
                    m_texture2D = settings[i].texture2D;
                    _adjusted = true;
                    break;
                }
            }
            /*
            var ordScreenDimensions = settings.OrderBy(sd => sd.ResolutionOrAspectRatio).ToList();

            for (int i = 0; i < ordScreenDimensions.Count(); i++){
                var sd = ordScreenDimensions[i];

                if(IsDimensions(sd.ResolutionOrAspectRatio, sd.Width, sd.Height)){
                    m_texture2D = sd.texture2D;
                    m_adjusted = true;
                }
            }
            */
        }

        public Texture2D GetTexture2D(){

            if(!_adjusted){
                Adjust();
            }
            if(m_texture2D == null){
                m_texture2D = defaulttexture2D;
            }
            return m_texture2D;
        }
    }

    [System.Serializable]
    public struct Texture2DScreenDimensionsSettings {

        public AdjustByScreenDimensions.ResolutionAspectType ScreenDimensionsType;
        public ScreenDimensions OtherScreenDimensions; 

        public Texture2D texture2D;
    }
}