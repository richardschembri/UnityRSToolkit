using UnityEditor;
using UnityEngine;
using System;
namespace RSToolkit{
    public class Packager
    {
        [UnityEditor.MenuItem("Tools/RSToolkit/Packager/ExportSpawner")]
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
    }
}