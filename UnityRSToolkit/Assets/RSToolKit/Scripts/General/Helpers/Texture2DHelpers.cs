namespace RSToolkit.Helpers
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public static class Texture2DHelpers 
    {

        public static Sprite ToSprite(this Texture2D texture2D, Vector2 pivot){
            return Sprite.Create( texture2D, new Rect(0,0, texture2D.width, texture2D.height), pivot);
        }

        public static Sprite ToSprite(this Texture2D texture2D){
            return texture2D.ToSprite(new Vector2(.5f, .5f));
        }
    }

}