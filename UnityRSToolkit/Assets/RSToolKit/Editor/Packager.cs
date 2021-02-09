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
            var toExportPaths = new string[]{
                "Assets/RSToolKit/Scripts/General/Controls/Spawner",
                "Assets/RSToolKit/Scripts/General/RSMonoBehaviour.cs",
                "Assets/RSToolKit/Scripts/General/Helpers/TransformHelpers.cs",
                "Assets/RSToolKit/Scripts/General/Helpers/RandomHelpers.cs",
                "Assets/RSToolKit/Scripts/General/Helpers/DebugHelpers.cs"
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    "rstoolkit-spawner.unitypackage",
                    ExportPackageOptions.Recurse);
            Debug.Log("Exported Spawner");
        }
    
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Paging")]
        public static void ExportUIPaging(){

            Debug.Log("Exporting UI Paging");
            var toExportPaths  = new string[]{
                "Assets/RSToolKit/Scripts/UI/Paging",
                "Assets/RSToolKit/Prefabs/UI/Paging",
                "Assets/RSToolKit/Scripts/General/Collections/SizedStack.cs"
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    "rstoolkit-uipaging.unitypackage",
                    ExportPackageOptions.Recurse);
            Debug.Log("Exported UI Paging");
        }
    
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Controls/Popup")]
        public static void ExportUIControlsPopup(){

            Debug.Log("Exporting UI Controls Popup");
            var toExportPaths = new string[]{
                "Assets/RSToolKit/Scripts/UI/Controls/UIPopup.cs",
                "Assets/RSToolKit/Prefabs/UI/Controls/UI Popup.prefab"
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    "rstoolkit-uipopup.unitypackage",
                    ExportPackageOptions.Recurse);
            Debug.Log("Exported UI Controls Popup");
        }
    
        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/AI/Bot")]
        public static void ExportBotAI(){

            Debug.Log("Exporting Bot AI");
            var toExportPaths = new string[]{
                "Assets/RSToolKit/Scripts/AI",
                "Assets/RSToolKit/Scripts/General/Helpers/DebugHelpers.cs",
                "Assets/RSToolKit/Scripts/General/Helpers/PhysicsHelpers.cs",
                "Assets/RSToolKit/Scripts/General/Helpers/RandomHelpers.cs",
                "Assets/RSToolKit/Scripts/General/Helpers/TransformHelpers.cs",
                "Assets/RSToolKit/Scripts/Space3D/Draggable3DObject.cs",
                "Assets/RSToolKit/Scripts/Space3D/Flying3DObject.cs",
                "Assets/RSToolKit/Scripts/Space3D/GameObjectNavSpawnerMarker.cs",
                "Assets/RSToolKit/Scripts/Space3D/LightNavSpawnerMarker.cs",
                "Assets/RSToolKit/Scripts/Space3D/NavSpawnerMarker.cs",
                "Assets/RSToolKit/Scripts/Space3D/SpawnerMarker.cs",
                "Assets/RSToolKit/Scripts/Space3D/ProximityChecker.cs"
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    "rstoolkit-botai.unitypackage",
                    ExportPackageOptions.Recurse);
            Debug.Log("Exported Bot AI");
        }

        [UnityEditor.MenuItem("Tools/RSToolkit/Export Package/UI/Adjust Layout Tools")]
        public static void ExportAdjustLayoutTools(){
            Debug.Log("Exporting Adjust Layout Tools");
            var toExportPaths = new string[]{
                "Assets/RSToolKit/Scripts/UI/Layout",
            };
            AssetDatabase.ExportPackage(
                    toExportPaths,
                    "rstoolkit-adjustlayouttools.unitypackage",
                    ExportPackageOptions.Recurse);
            Debug.Log("Exported Adjust Layout Tools");
        }

// 
    }
}