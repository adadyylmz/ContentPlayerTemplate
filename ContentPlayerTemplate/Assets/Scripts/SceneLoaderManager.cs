using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SceneLoaderManager : MonoBehaviour
{
    public DownloadManager downloadManager;
    public GameObject xrRig;
    public TMP_Dropdown filesDropdown;
    public Button loadSceneButton;

    void Start()
    {
        loadSceneButton.onClick.AddListener(OnLoadSceneButtonClicked);
    }

    void Update()
    {

    }

    // Called when the Load Scene button is clicked
    private void OnLoadSceneButtonClicked()
    {
        string selectedFile = filesDropdown.options[filesDropdown.value].text;
        DetermineSceneTypeAndLoad(selectedFile);
    }

    // Determines whether to load a VR or non-VR scene based on the selected file
    private void DetermineSceneTypeAndLoad(string sceneName)
    {
        if (sceneName.EndsWith("_VRS.unity"))
        {
            LoadVRReadyScene(sceneName);
        }
        else if (sceneName.EndsWith(".unity"))
        {
            LoadNotVRScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Selected file is not a valid Unity scene.");
        }
    }

    // Loads a VR-ready scene and disables the XR rig if present
    public void LoadVRReadyScene(string sceneName)
    {
        if (xrRig != null)
        {
            xrRig.SetActive(false);
        }

        StartCoroutine(LoadSceneAsync(sceneName));
    }

    // Loads a non-VR scene and enables the XR rig if present
    public void LoadNotVRScene(string sceneName)
    {
        if (xrRig != null)
        {
            xrRig.SetActive(true);
        }
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    // Loads the lobby scene
    public void LoadLobbyScene()
    {
        SceneManager.LoadSceneAsync("LobbyScene");
    }

    // Asynchronously loads the scene
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        string scenePath = System.IO.Path.Combine("Assets/DownloadedFiles", sceneName);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenePath);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
