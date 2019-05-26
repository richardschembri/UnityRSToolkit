namespace RSToolkit.Helpers
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class LoadImageTools
    {
        // Start is called before the first frame update
        private static string[] m_extensions = {"jpg", "jpeg", "png", "gif" };
        public string ImagesFolderPath = "Images/Dynamic";

        private string ImagesFolderDirectory(){
            return string.Format("{0}/{1}", Application.dataPath, ImagesFolderPath );	
        }

        private string ImagesFileDirectory(string fileName = "Images/Dynamic"){
            return string.Format("{0}/{1}", ImagesFolderDirectory(), fileName);
        }

        public LoadImageTools(string imagesFolderPath){
            ImagesFolderPath = imagesFolderPath;
        }

        private List<Texture2D> m_loadedTextures = new List<Texture2D>();
        public List<Texture2D> LoadedTextures{
            get{
                return m_loadedTextures;
            }
        }

        public void LoadTexturesInMemory(bool shuffle = false){
            for (int i = 0; i < m_loadedTextures.Count; i++){
                m_loadedTextures[i] = null;
            }
            m_loadedTextures.Clear();
            var fileInfoList = FileHelpers.GetFileInfoList(m_extensions, ImagesFolderDirectory(), false);

            for (int i = 0; i < fileInfoList.Length; i++)
            {
                var referenceTexture = LoadImage(ImagesFileDirectory(fileInfoList[i].Name));
                referenceTexture.name = fileInfoList[i].Name.Replace(fileInfoList[i].Extension, "");
                m_loadedTextures.Add(referenceTexture);
            }

            if(shuffle){
                m_loadedTextures.Shuffle(); 
            }else{
                m_loadedTextures = m_loadedTextures.OrderBy(gt => gt.name).ToList();
            }
        }


        

        public static Texture2D LoadImage(string imageFileDirectory, bool isRelativePath = false){
            if (string.IsNullOrEmpty(imageFileDirectory)){
                    return null;
            }

            bool has_ext = m_extensions.Any( e => imageFileDirectory.EndsWith(string.Format(".{0}", e)));

            //if(imageFileDirectory.EndsWith(".jpg"))
            var absolutePath = imageFileDirectory;
            if (isRelativePath)
            {
                absolutePath = FileHelpers.GetFullSaveFilePath(imageFileDirectory);
            }

            if(!has_ext){
                for(int i = 0; i < m_extensions.Length; i++){
                    string extPath = string.Format("{0}.{1}", absolutePath, m_extensions[i]);
                    if(File.Exists(extPath)){
                        absolutePath = extPath;
                        break;         
                    }
                }
            }

            if (!File.Exists(absolutePath)){
                return null;
            }

            var www = new WWW("file://" + absolutePath );
            
            var imageTexture = new Texture2D(4, 4);

            while(!www.isDone){}
            www.LoadImageIntoTexture((Texture2D)imageTexture);
            imageTexture.Apply();
            return imageTexture;
        }


        public Texture2D GetRandomLoadedTexture(){
            return LoadedTextures[RandomHelpers.RandomInt(LoadedTextures.Count)];
        }
    }

}