using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public class SceneLoaderManager : MonoBehaviour
{
    public DownloadManager downloadManager;
    public GameObject xrRig;
    public TMP_Dropdown filesDropdown;
    public Button loadSceneButton;

    void Start()
    {
        loadSceneButton.onClick.AddListener(OnFileSelected);
    }

    void Update()
    {

    }

    // Called when a file is selected from the dropdown; determines if the scene is VR-ready or not
    private void OnFileSelected()
    {
        string selectedFile = filesDropdown.options[filesDropdown.value].text;

        if (selectedFile.EndsWith("_VRS.unity"))
        {
            LoadVRReadyScene(selectedFile);
        }
        else if (selectedFile.EndsWith(".unity"))
        {
            LoadNotVRScene(selectedFile);
        }
    }

    // Loads a VR-ready scene and disables the XR rig if present
    public void LoadVRReadyScene(string sceneName)
    {
        if (xrRig != null)
        {
            xrRig.SetActive(false);
        }

        StartCoroutine(AddAndLoadSceneAsync(sceneName));
    }

    // Loads a non-VR scene and enables the XR rig if present
    public void LoadNotVRScene(string sceneName)
    {
        if (xrRig != null)
        {
            xrRig.SetActive(true);
        }
        StartCoroutine(AddAndLoadSceneAsync(sceneName));
    }

    // Loads the lobby scene
    public void LoadLobbyScene()
    {
        SceneManager.LoadSceneAsync("LobbyScene");
    }

    // Adds the scene to build settings and asynchronously loads it
    private IEnumerator AddAndLoadSceneAsync(string sceneName)
    {
        AddSceneToBuildSettings(sceneName);

        string scenePath = System.IO.Path.Combine("Assets/DownloadedFiles", sceneName);
        if (!IsSceneInBuildSettings(scenePath))
        {
            Debug.LogError("Scene " + scenePath + " is not in build settings.");
            yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Adds the selected scene to the build settings if it's not already added
    private void AddSceneToBuildSettings(string sceneName)
    {
        string scenePath = System.IO.Path.Combine("Assets/DownloadedFiles", sceneName);

        var buildScenes = EditorBuildSettings.scenes.ToList();

        if (!buildScenes.Any(s => s.path == scenePath))
        {
            buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log("Added scene to build settings: " + scenePath);
        }
        else
        {
            Debug.LogWarning("Scene already in build settings: " + scenePath);
        }
    }

    // Checks if the specified scene is already included in the build settings
    private bool IsSceneInBuildSettings(string scenePath)
    {
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.path == scenePath)
            {
                return true;
            }
        }
        return false;
    }
}
