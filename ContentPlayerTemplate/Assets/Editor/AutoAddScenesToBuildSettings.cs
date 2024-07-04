using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine;

public class AutoAddScenesToBuildSettings : AssetPostprocessor
{
    private static string folderPath = "Assets/Scenes"; // Folder to watch for scenes

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        bool shouldUpdate = importedAssets.Concat(movedAssets).Any(path => path.StartsWith(folderPath) && path.EndsWith(".unity"));

        if (shouldUpdate)
        {
            Debug.Log("Scenes updated in folder: " + folderPath);
        }
    }
}
