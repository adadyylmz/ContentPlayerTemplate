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
    public GameObject xrRig; // Reference to the XRRig in the LobbyScene
    public TMP_Dropdown filesDropdown; // Reference to the Dropdown element for listing files
    public Button loadSceneButton; // Reference to the Load Scene button

    void Start()
    {
        // Add listener for the Load Scene button
        loadSceneButton.onClick.AddListener(OnFileSelected);
    }

    void Update()
    {

    }

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

    public void LoadVRReadyScene(string sceneName)
    {
        // Disable the XRRig in the LobbyScene
        if (xrRig != null)
        {
            xrRig.SetActive(false);
        }
        // Load the selected scene
        StartCoroutine(AddAndLoadSceneAsync(sceneName));
    }

    public void LoadNotVRScene(string sceneName)
    {
        // Ensure the XRRig in the LobbyScene is active
        if (xrRig != null)
        {
            xrRig.SetActive(true);
        }
        // Load the selected scene
        StartCoroutine(AddAndLoadSceneAsync(sceneName));
    }

    // Load the LobbyScene
    public void LoadLobbyScene()
    {
        SceneManager.LoadSceneAsync("LobbyScene");
    }

    private IEnumerator AddAndLoadSceneAsync(string sceneName)
    {
        // Add the scene to the build settings
        AddSceneToBuildSettings(sceneName);

        // Ensure the scene is in the build settings
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
