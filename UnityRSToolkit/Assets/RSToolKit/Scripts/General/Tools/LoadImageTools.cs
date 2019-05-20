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

        private List<FileInfo> _lstFileInfo;
        public List<FileInfo> lstFileInfo{
            get{
                if (_lstFileInfo == null || _lstFileInfo.Count <= 0){
                    _lstFileInfo = FileHelpers.GetFileInfoList(new List<string>{".jpg", ".png"},ImagesFolderDirectory(), false);
                }
                return _lstFileInfo;
            }
        }
        private List<Texture2D> _generatedTextures;
        public List<Texture2D> GeneratedTextures{
            get{

                if ((_generatedTextures == null) || (_generatedTextures.Count <= 0))
                {
                    lstFileInfo.Shuffle();
                    _generatedTextures = new List<Texture2D>();
                    for (int i = 0; i < lstFileInfo.Count; i++)
                    {
                        var referenceTexture = LoadImage(ImagesFileDirectory(lstFileInfo[i].Name));
                        referenceTexture.name = lstFileInfo[i].Name.Replace(lstFileInfo[i].Extension, "");
                        _generatedTextures.Add(referenceTexture);
                    }
                    _generatedTextures = _generatedTextures.OrderBy(gt => gt.name).ToList();
                }

                return _generatedTextures;
            }
        }

        public bool HasTextures(){
            return ((_generatedTextures != null) && (!_generatedTextures.Any()));
        }

        void Start () {
        }

        void Update () {
        }

        public static Texture2D LoadImage(string imageFileDirectory, bool isRelativePath = false){
            var absolutePath = imageFileDirectory;
            if (isRelativePath)
            {
                absolutePath = FileHelpers.GetFullSaveFilePath(imageFileDirectory);
            }
            if (string.IsNullOrEmpty(imageFileDirectory) || !File.Exists(absolutePath)){
                return null;
            }

            var www = new WWW("file://" + absolutePath );
            var slotTexture = new Texture2D(4, 4);

            while(!www.isDone){}
            www.LoadImageIntoTexture((Texture2D)slotTexture);
            slotTexture.Apply();
            return slotTexture;
        }

        public void RefreshReferenceSlotImages(){
            lstFileInfo.Clear();	
            GeneratedTextures.Clear();
        }


        public void ShuffleGeneratedTextures(){
            GeneratedTextures.Shuffle();
        }

        public Texture2D GetRandomGeneratedTexture(){
            return GeneratedTextures[RandomHelpers.RandomInt(GeneratedTextures.Count)];
        }
    }

}