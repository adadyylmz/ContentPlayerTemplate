using UnityEngine.SceneManagement;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.EventSystems;

public class SceneManager_Addressables : MonoBehaviour
{
    //public static SceneLoaderManager Instance { get; private set; }

    #region Fields

    [SerializeField] Slider percentDownloadingUI;
    [SerializeField] TMP_Text percentDownloadingText;

    [SerializeField] AsyncOperationHandle<SceneInstance> _sceneLoaderHandler;
    [SerializeField] public Dictionary<string, SceneInstance> _downloadedScenes = new Dictionary<string, SceneInstance>();

    // A dictionary to hold buttons and their text components
    private Dictionary<string, (Button, Text)> sceneButtons = new Dictionary<string, (Button, Text)>();

    #endregion

    #region Unity Methods

    void Start()
    {
        Caching.ClearCache();
        Debug.Log("Scene cache is cleared.");
    }

    void Update()
    {
        if (_sceneLoaderHandler.IsValid())
        {
            float percent = _sceneLoaderHandler.GetDownloadStatus().Percent;
            if (percent < 1)
            {
                percentDownloadingUI.value = percent;
                percentDownloadingText.text = "Downloading   " + percent * 100 + "%";
                Debug.Log(percent);
            }
        }
    }

    #endregion

    #region Public Methods

    // General method to handle scene download and button update
    public void HandleSceneButton(AssetReference sceneKey, Button button, Text buttonText, string sceneName)
    {
        if (_downloadedScenes.ContainsKey(sceneKey.RuntimeKey.ToString()))
        {
            // If the scene is already downloaded, set the button to load the scene
            buttonText.text = $"Play {sceneName} scene";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => LoadScene(sceneKey));
        }
        else
        {
            // If the scene is not downloaded, set the button to download the scene
            buttonText.text = $"Download {sceneName} scene";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => DownloadScene(sceneKey, button, buttonText, sceneName));
        }

        // Store button and text reference in the dictionary
        if (!sceneButtons.ContainsKey(sceneKey.RuntimeKey.ToString()))
        {
            sceneButtons.Add(sceneKey.RuntimeKey.ToString(), (button, buttonText));
        }
    }

    // Downloading the scene without scene activation
    public async void DownloadScene(AssetReference sceneKey, Button button, Text buttonText, string sceneName)
    {
        bool isInCache = await Addressables.GetDownloadSizeAsync(sceneKey) == 0;
        if (!isInCache)
        {
            string sceneReference = sceneKey.RuntimeKey.ToString();

            percentDownloadingUI.gameObject.SetActive(true);
            percentDownloadingText.gameObject.SetActive(true);

            _sceneLoaderHandler = Addressables.LoadSceneAsync(sceneKey, LoadSceneMode.Single, false, 100);

            _sceneLoaderHandler.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    _downloadedScenes.Add(sceneReference, op.Result);
                    Debug.Log("Scene downloaded successfully: " + sceneKey);

                    // Change button text to "Play ${sceneName} scene"
                    buttonText.text = $"Play {sceneName} scene";

                    // Update button functionality to load the scene
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => LoadScene(sceneKey));
                }
                else
                {
                    Debug.Log("Failed to download video: " + op.OperationException);
                }
            };

            await _sceneLoaderHandler.Task;

            percentDownloadingUI.gameObject.SetActive(false);
            percentDownloadingText.gameObject.SetActive(false);

            Debug.Log("Addressable scene downloaded");
        }
        else
        {
            Debug.Log("Scene is already in cache!");
        }
    }

    // Loading the scene using scenekey
    public async void LoadScene(AssetReference sceneKey)
    {
        bool isCached = await IsSceneCached(sceneKey);
        // Checking if the scene is downloaded
        if (isCached)
        {
            // Loading the scene
            _sceneLoaderHandler = Addressables.LoadSceneAsync(sceneKey, LoadSceneMode.Single, true, 100);

            _sceneLoaderHandler.Completed += (op) =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log("Scene with key " + sceneKey + " loaded.");
                }
                else
                {
                    Debug.LogError("Scene with key " + sceneKey + " is downloaded but failed to load.");
                }
            };
        }
        else
        {
            Debug.LogError("Scene with key " + sceneKey + " is not downloaded.");
        }
    }

    public async UniTask<bool> IsSceneCached(AssetReference sceneKey)
    {
        long downloadSize = await GetSceneDownloadSize(sceneKey);
        return downloadSize == 0;
    }

    public async UniTask<long> GetSceneDownloadSize(AssetReference sceneKey)
    {
        AsyncOperationHandle<long> downloadSizeHandle = Addressables.GetDownloadSizeAsync(sceneKey);
        await downloadSizeHandle.Task;
        return downloadSizeHandle.Result;
    }

    // Load the LobbyScene
    public void LoadLobbyScene()
    {
        SceneManager.LoadSceneAsync("LobbyScene");
    }

    #endregion
}
