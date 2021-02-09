namespace RSToolkit.UI.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class AdjustByScreenDimensions : MonoBehaviour
    {
        public bool AdjustOnStart = true;
        protected bool _adjusted = false;

        public enum ResolutionAspectType{
            iPhonePremium,
            iPhoneBudget,
            iPad,
            Other
        }
#region MonoBehaviour Funcitons
        protected virtual void Start () {
            if(AdjustOnStart){
                Adjust();		
            }
        }

        protected virtual void Awake(){
            if(AdjustOnStart){
                Adjust();		
            }
        }

        protected virtual void Update () {
            
        }
#endregion MonoBehaviour Funcitons

        public List<ScreenDimensions> GetPresetScreenDimensions(ResolutionAspectType rat, ScreenDimensions other){
            var result = new List<ScreenDimensions>();

            switch(rat){
                case ResolutionAspectType.iPhonePremium:
                result.Add(new ScreenDimensions(false, new Vector2(90, 185)));
                result.Add(new ScreenDimensions(true, new Vector2(1125, 2436)));
                result.Add(new ScreenDimensions(true, new Vector2(1242, 2688)));
                break;
                case ResolutionAspectType.iPhoneBudget:
                result.Add(new ScreenDimensions(true, new Vector2(828, 1792)));
                break;
                case ResolutionAspectType.iPad:
                result.Add(new ScreenDimensions(false, new Vector2(3, 4)));
                result.Add(new ScreenDimensions(false, new Vector2(512, 683)));
                result.Add(new ScreenDimensions(true, new Vector2(1668, 2388)));
                break;
                case ResolutionAspectType.Other:
                result.Add(other);
                break;
            }

            return result;
        }

        public bool IsDimensions(ScreenDimensions sd){
            return IsDimensions(sd.ResolutionOrAspectRatio, sd.Dimensions);
        }

        public bool IsDimensions(bool resolutionOrAspectRatio, Vector2 dimensions){
            if(resolutionOrAspectRatio){
                return CheckResolution(dimensions);
            }else{
                return CheckAspectRatio(dimensions);
            }
        }

        public bool CheckResolution(Vector2 resolution){
            return (resolution.x == Screen.width && resolution.y == Screen.height); 
        }

        public bool CheckAspectRatio(Vector2 ratio){
            return (((float)Screen.width / (float)Screen.height) == (ratio.x / ratio.y));
        }

        public virtual void Adjust(){
            _adjusted = true;
        }

        public bool IsAdjusted{
            get{
                return _adjusted;
            }
        }
    }

    [System.Serializable]
    public struct ScreenDimensions {
        public bool ResolutionOrAspectRatio;
        public Vector2 Dimensions;
        public ScreenDimensions(bool resolutionOrAspectRatio, Vector2 dimensions){
            ResolutionOrAspectRatio = resolutionOrAspectRatio;
            Dimensions = dimensions;
        }
    }
}