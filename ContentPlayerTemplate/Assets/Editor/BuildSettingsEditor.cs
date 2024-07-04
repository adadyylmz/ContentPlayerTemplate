using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class BuildSettingsEditor : EditorWindow
{
    private string folderPath = "Assets/DownloadedFiles"; // Default folder path

    [MenuItem("Tools/Auto-Add Scenes to Build Settings")]
    public static void ShowWindow()
    {
        GetWindow<BuildSettingsEditor>("Auto-Add Scenes to Build Settings");
    }

    private void OnGUI()
    {
        GUILayout.Label("Auto-Add Scenes to Build Settings", EditorStyles.boldLabel);

        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

        if (GUILayout.Button("Add Scenes from Folder"))
        {
            AddScenesFromFolder(folderPath);
        }
    }

    private void AddScenesFromFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Folder does not exist: " + folderPath);
            return;
        }

        var sceneFiles = Directory.GetFiles(folderPath, "*.unity", SearchOption.AllDirectories);
        if (sceneFiles.Length == 0)
        {
            Debug.LogWarning("No scenes found in folder: " + folderPath);
            return;
        }

        var buildScenes = EditorBuildSettings.scenes.ToList();

        foreach (var sceneFile in sceneFiles)
        {
            if (!buildScenes.Any(s => s.path == sceneFile))
            {
                buildScenes.Add(new EditorBuildSettingsScene(sceneFile, true));
                Debug.Log("Added scene: " + sceneFile);
            }
            else
            {
                Debug.LogWarning("Scene already in build settings: " + sceneFile);
            }
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
        Debug.Log("Scenes added to build settings from folder: " + folderPath);
    }
}
