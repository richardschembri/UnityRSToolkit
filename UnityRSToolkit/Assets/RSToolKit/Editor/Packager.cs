using UnityEditor;
using UnityEngine;
using System;
namespace RSToolkit{
    public class Packager
    {
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/Spawner")]
        public static void ExportSpawner()
        {
            Debug.Log("Exporting Spawner");
            var spawnerPaths = new string[]{
                "Assets/RSToolKit/Scripts/General/Controls/Spawner",
                "Assets/RSToolKit/Scripts/General/RSMonoBehaviour.cs",
                "Assets/RSToolKit/Scripts/General/Helpers/TransformHelpers.cs",
                "Assets/RSToolKit/Scripts/General/Helpers/RandomHelpers.cs",
                "Assets/RSToolKit/Scripts/General/Helpers/DebugHelpers.cs"
            };
            AssetDatabase.ExportPackage(
                    spawnerPaths,
                    "rstoolkit-spawner.unitypackage",
                    ExportPackageOptions.Recurse);
            Debug.Log("Exported Spawner");
        }
    
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Paging")]
        public static void ExportUIPaging(){

            Debug.Log("Exporting UI Paging");
            var spawnerPaths = new string[]{
                "Assets/RSToolKit/Scripts/UI/Paging",
                "Assets/RSToolKit/Prefabs/UI/Paging",
                "Assets/RSToolKit/Scripts/General/Collections/SizedStack.cs"
            };
            AssetDatabase.ExportPackage(
                    spawnerPaths,
                    "rstoolkit-uipaging.unitypackage",
                    ExportPackageOptions.Recurse);
            Debug.Log("Exported UI Paging");
        }
    
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Controls/Popup")]
        public static void ExportUIControlsPopup(){

            Debug.Log("Exporting UI Controls Popup");
            var spawnerPaths = new string[]{
                "Assets/RSToolKit/Scripts/UI/Controls/UIPopup.cs",
                "Assets/RSToolKit/Prefabs/UI/Controls/UI Popup.prefab"
            };
            AssetDatabase.ExportPackage(
                    spawnerPaths,
                    "rstoolkit-uipopup.unitypackage",
                    ExportPackageOptions.Recurse);
            Debug.Log("Exported UI Controls Popup");
        }
    }
}