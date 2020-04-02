// Scaling code taken from https://pastebin.com/qkkhWs2J
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

        public static void Rescale(this Texture2D tex, int width, int height)
        {
            TextureScale.Bilinear(tex, width, height);
        }


        public static Texture2D GetScaledCopy(this Texture2D src, int width, int height)
        {
            Texture2D result = new Texture2D(src.width, src.height, TextureFormat.RGBA32, true);
            result.SetPixels(src.GetPixels());
            TextureScale.Bilinear(result, width, height);
            return result;                 
        }

        public static string SaveToPNG(this Texture2D texture2D, string folderPath = "", string fileName = "textureImage", bool addTimeStamp = true){
            string textureFileName = string.Empty;

            if (!addTimeStamp){
                textureFileName = FileHelpers.GenerateFileName(fileName, "png");
            }
            else{
                textureFileName = FileHelpers.GenerateFileName_WithDateTimeStamp(fileName, "png");
            }

            var savePath = FileHelpers.GetFullSaveFilePath(textureFileName, folderPath);
            texture2D.SaveToPNG(savePath);
            return textureFileName;
        }

        public static void SaveToPNG(this Texture2D texture2D, string filePath){
            FileHelpers.CreateDirectoryIfNotExists(filePath);

            var bytes = texture2D.EncodeToPNG();
            System.IO.File.WriteAllBytes(filePath, bytes);
        }

        /*
        static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
        {
                //We need the source texture in VRAM because we render with it
                src.filterMode = fmode;
                src.Apply(true);       
                               
                //Using RTT for best quality and performance. Thanks, Unity 5
                RenderTexture rtt = new RenderTexture(width, height, 32);
               
                //Set the RTT in order to render to it
                Graphics.SetRenderTarget(rtt);
               
                //Setup 2D matrix in range 0..1, so nobody needs to care about sized
                GL.LoadPixelMatrix(0,1,1,0);
               
                //Then clear & draw the texture to fill the entire RTT.
                GL.Clear(true,true,new Color(0,0,0,0));
                Graphics.DrawTexture(new Rect(0,0,1,1),src);
        }

        public static void Rescale(this Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
                Rect texR = new Rect(0,0,width,height);
                _gpu_scale(tex,width,height,mode);
               
                // Update new texture
                tex.Resize(width, height);
                tex.ReadPixels(texR,0,0,true);
                tex.Apply(true);        //Remove this if you hate us applying textures for you :)
        }

        public static Texture2D GetScaledCopy(this Texture2D src, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
                Rect texR = new Rect(0,0,width,height);
                _gpu_scale(src,width,height,mode);
               
                //Get rendered data back to a new texture
                Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, true);
                result.Resize(width, height);
                result.ReadPixels(texR,0,0,true);
                return result;                 
        }
        */

    }
}