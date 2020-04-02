namespace RSToolkit.Helpers
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine.Networking;
    using System.Threading.Tasks;

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
        private List<Sprite> m_loadedSprites = new List<Sprite>();
        public List<Sprite> LoadedSprites{
            get{
                return m_loadedSprites;
            }
        }

        public async void LoadTexturesInMemory(bool shuffle = false){
            for (int i = 0; i < m_loadedTextures.Count; i++){
                m_loadedTextures[i] = null;
            }
            m_loadedTextures.Clear();
            var fileInfoList = FileHelpers.GetFileInfoList(m_extensions, ImagesFolderDirectory(), false);

            for (int i = 0; i < fileInfoList.Length; i++)
            {
                var referenceTexture = await LoadTexture2D(ImagesFileDirectory(fileInfoList[i].Name));
                referenceTexture.name = fileInfoList[i].Name.Replace(fileInfoList[i].Extension, "");
                m_loadedTextures.Add(referenceTexture);
            }

            if(shuffle){
                m_loadedTextures.Shuffle(); 
            }else{
                m_loadedTextures = m_loadedTextures.OrderBy(gt => gt.name).ToList();
            }
        }


        public void LoadSpritesInMemory(bool shuffle = false){
            LoadTexturesInMemory(shuffle);
            m_loadedSprites = LoadedTextures.Select(t => t.ToSprite()).ToList();
        }
        
        public static async Task<Sprite> LoadSprite(string imageFileDirectory, bool isRelativePath = false){
            var resultTexture = await LoadTexture2D(imageFileDirectory, isRelativePath);
            return resultTexture.ToSprite();
        }

        public static bool FilePathHasImageExtension(string path){
            return FileHelpers.FilePathHasExtension(path, m_extensions);
        }

        public static bool ImageFileExists(string path){

            bool has_ext = FilePathHasImageExtension(path);

            if(!has_ext){
                for(int i = 0; i < m_extensions.Length; i++){
                    string extPath = string.Format("{0}.{1}", path, m_extensions[i]);
                    if(File.Exists(extPath)){
                        return true;
                    }
                }
            }

            return File.Exists(path);
        }

        public static async Task<Texture2D> LoadTexture2D(string path, bool isRelativePath = false){
            if (string.IsNullOrEmpty(path)){
                    return null;
            }

            bool has_ext = FilePathHasImageExtension(path);

            //if(imageFileDirectory.EndsWith(".jpg"))
            var absolutePath = path;
            if (isRelativePath)
            {
                absolutePath = FileHelpers.GetFullSaveFilePath(path);
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

            var www = UnityWebRequestTexture.GetTexture("file://" + absolutePath);
            var request = www.SendWebRequest();

            while(!request.isDone ){
                await Task.Delay( 1000 / 30 );
            }
            if (www.isHttpError || www.isNetworkError)
            {
                UnityEngine.Debug.LogError("Error while Receiving: " + www.error);
                return null;
            }
            else
            {
                //imageTexture.LoadImage(handle.data);
                return DownloadHandlerTexture.GetContent(www);
            }

        }


        public Texture2D GetRandomLoadedTexture(){
            return LoadedTextures[RandomHelpers.RandomInt(LoadedTextures.Count)];
        }
    }

}