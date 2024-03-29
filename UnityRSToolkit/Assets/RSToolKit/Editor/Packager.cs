﻿using UnityEditor;
using UnityEngine;
using System.Linq;

namespace RSToolkit{
    public class Packager
    {

        #region TOOLS
        private enum PATH_TYPE{
            SCRIPTS,
            PREFABS,
            MATERIALS
        }
        const string RS_PATH = "Assets/RSToolKit";
        static readonly string rsPathScripts = $"{RS_PATH}/Scripts";
        static readonly string rsPathPrefabs = $"{RS_PATH}/Prefabs";
        static readonly string rsPathMaterials = $"{RS_PATH}/Materials";
        const string RS_SUBPATH_GENERAL = "General"; 
        const string RS_SUBPATH_SPACE2D = "Space2D"; 
        const string RS_SUBPATH_SPACE3D = "Space3D"; 
        const string RS_SUBPATH_GAME = "Game"; 
        const string RS_SUBPATH_AI = "AI"; 
        const string RS_SUBPATH_CHARACTER = "Character"; 
        const string RS_SUBPATH_UI = "UI"; 
        const string RS_SUBPATH_ANIMATION = "Animation"; 
        const string RS_SUBPATH_NETWORK = "Network"; 
        private static string GetPath(PATH_TYPE pathType, string subPath = ""){
            string basePath = string.Empty;
            switch(pathType){
                case PATH_TYPE.SCRIPTS:
                basePath = rsPathScripts;
                break;
                case PATH_TYPE.PREFABS:
                basePath = rsPathPrefabs;
                break;
                case PATH_TYPE.MATERIALS:
                basePath = rsPathMaterials;
                break;
            }

            return $"{basePath}/{subPath}";
        }

        private static string GetPackageName(string name){
            return $"rstoolkit-{name}.unitypackage";
        }
        private static void DebugLogExportStart(string packageName){
            Debug.Log($"Exporting {packageName}");
        } 
        private static void DebugLogExportEnd(string packageName){
            Debug.Log($"Exported {packageName}");
        } 

        #endregion TOOLS

        private static string[] GetFileList_Spawner()
        {
            return new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Controls/Spawner",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/TransformHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/RandomHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/DebugHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Collections/SizedStack.cs"
            };
        }

        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/Controls/Spawner")]
        public static void ExportSpawner()
        {
            DebugLogExportStart("Spawner");
            var toExportPaths = GetFileList_Spawner();
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("spawner"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("Spawner");
        }

        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Controls/ListBox")]
        public static void ExportUIListBox(){
            DebugLogExportStart("UI ListBox");

            var toExportPaths_Spawner = GetFileList_Spawner();
            var toExportPaths  = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI)}/Controls/ListBox",
            };
            AssetDatabase.ExportPackage(
                    toExportPaths.Concat(toExportPaths_Spawner).ToArray(),
                    GetPackageName("uilistbox"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("UI ListBox");
        }
    
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Paging")]
        public static void ExportUIPaging(){

            DebugLogExportStart("UI Paging");
            var toExportPaths_Spawner = GetFileList_Spawner();
            var toExportPaths  = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSSingletonMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI) }/Paging",
                $"{GetPath(PATH_TYPE.PREFABS, RS_SUBPATH_UI)}/Paging",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/TransformHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/DebugHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Collections/SizedStack.cs"
            };
            AssetDatabase.ExportPackage(
                    //toExportPaths,
                    toExportPaths.Concat(toExportPaths_Spawner).ToArray(),
                    GetPackageName("uipaging"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("UI Paging");
        }
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Controls/Selectable")]
        public static void ExportUISelectable(){

            DebugLogExportStart("UI Selectable");
            var toExportPaths_Spawner = GetFileList_Spawner();
            var toExportPaths  = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI) }/Controls/UISelectable.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI)}/Controls/UIPermanentSelectable.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI)}/Controls/UIPermanentSelectableGroup.cs"
            };
            AssetDatabase.ExportPackage(
                    //toExportPaths,
                    toExportPaths.Concat(toExportPaths_Spawner).ToArray(),
                    GetPackageName("uiselectable"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("UI Selectable");
        }
    
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Controls/Popup")]
        public static void ExportUIControlsPopup(){

            DebugLogExportStart("UI Controls Popup");
            var toExportPaths = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI)}/Controls/UIPopup.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI)}/Controls/UIPopupGroup.cs",
                $"{GetPath(PATH_TYPE.PREFABS, RS_SUBPATH_UI)}/Controls/UI Popup.prefab"
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("uipopup"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("UI Controls Popup");
        }
    
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/AI/Debug/NavMeshVisualizer")]
        public static void ExportNavMeshVisualizer(){
            DebugLogExportStart("NavMeshVisualizer");
            var toExportPaths = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_AI)}/NavMeshVisualizer.cs",
                $"{GetPath(PATH_TYPE.MATERIALS)}/NavMeshVisualizer.mat",
                $"{GetPath(PATH_TYPE.PREFABS, RS_SUBPATH_AI)}/NavMeshVisualizer.prefab"
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("navmeshvisualizer"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("NavMeshVisualizer");
        }



        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/Game/EndlessRunner")]
        public static void ExportEndlessRunner(){
            DebugLogExportStart("Endless Runner");

            var toExportPaths_Spawner = GetFileList_Spawner();
            var toExportPaths  = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/RandomHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSPlayerInputManager.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_CHARACTER)}",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GAME)}/EndlessRunner",
            };
            AssetDatabase.ExportPackage(
                    toExportPaths.Concat(toExportPaths_Spawner).ToArray(),
                    GetPackageName("endlessrunner"),
                    ExportPackageOptions.Recurse);

            DebugLogExportEnd("Endless Runner");
        }
    
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/AI/Bot")]
        public static void ExportBotAI(){

            DebugLogExportStart("Bot AI");
            var toExportPaths = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_AI)}",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_ANIMATION)}",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_NETWORK)}",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/DebugHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/PhysicsHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/RandomHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/TransformHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Helpers/CollectionHelpers.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE3D)}/Draggable3DObject.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE3D)}/Flying3DObject.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE3D)}/GameObjectNavSpawnerMarker.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE3D)}/LightNavSpawnerMarker.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE3D)}/NavSpawnerMarker.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE3D)}/SpawnerMarker.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE3D)}/ProximityChecker.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE3D)}/Helpers",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE3D)}/Cameras",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI)}/Controls/UITextFollow3D.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI)}/Controls/UITextFollow3DManager.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSSingletonMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSShadow.cs",
                "Assets/Demo/AI/Bot",
                "Assets/Materials",

            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("botai"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("Bot AI");
        }

        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Adjust Layout Tools")]
        public static void ExportAdjustLayoutTools(){
            DebugLogExportStart("Adjust Layout Tools");
            var toExportPaths = new string[]{
                "Assets/RSToolKit/Scripts/UI/Layout",
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("adjustlayouttools"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("Adjust Layout Tools");
        }

        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/Character/2D")]
        public static void ExportCharacter2D(){

            DebugLogExportStart("Character 2D");
            var toExportPaths = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_CHARACTER)}/.",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE2D)}/CharacterController2D.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE2D)}/SpriteParallax.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_SPACE2D)}/PlayerLocomotion2D.cs"
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("character2d"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("Character 2D");
        }

        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/Controls/Countdown")]
        public static void ExportCountDown(){

            DebugLogExportStart("Count Down");
            var toExportPaths = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Controls/CountDown.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_UI)}/Controls/UICountDownText.cs"
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("countdown"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("Count Down");
        }

        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/Tools/Server Upload Tools")]
        public static void ExportServerUploadTools(){

            DebugLogExportStart("Server Upload Tools");
            var toExportPaths = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/Tools/ServerUploadTools.cs"
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("countdown"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("Count Down");
        }

        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/Singleton")]
        public static void ExportSingleton()
        {
            DebugLogExportStart("Singleton");
            var toExportPaths = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSSingletonMonoBehaviour.cs",
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("singleton"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("Singleton");
        }

        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/Network")]
        public static void ExportNetwork()
        {
            DebugLogExportStart("Network");
            var toExportPaths = new string[]{
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSSingletonMonoBehaviour.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_GENERAL)}/RSShadow.cs",
                $"{GetPath(PATH_TYPE.SCRIPTS, RS_SUBPATH_NETWORK)}",
                $"{GetPath(PATH_TYPE.PREFABS, RS_SUBPATH_NETWORK)}",
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    GetPackageName("network"),
                    ExportPackageOptions.Recurse);
            DebugLogExportEnd("Network");
        }


    }
}