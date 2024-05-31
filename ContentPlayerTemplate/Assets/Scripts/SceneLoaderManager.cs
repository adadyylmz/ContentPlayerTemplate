using UnityEngine.SceneManagement;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SceneLoaderManager : MonoBehaviour
{
    #region Fields

    [SerializeField] Slider percentDownloadingUI;
    [SerializeField] TMP_Text percentDownloadingText;

    [SerializeField] AsyncOperationHandle<SceneInstance> _sceneLoaderHandler;
    [SerializeField] public Dictionary<string, SceneInstance> _downloadedScenes = new Dictionary<string, SceneInstance>();

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
                percentDownloadingText.text = "Downloading " + percent * 100 + "%";
                Debug.Log(percent);
            }
        }
    }

    #endregion

    #region Public Methods

    // Downloading the scene without scene activation
    public async UniTask DownloadScene(AssetReference sceneKey)
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
                }
                else
                {
                    Debug.Log("Failed to download scene: " + op.OperationException);
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
    public async UniTask LoadScene(AssetReference sceneKey)
    {
        bool isCached = await IsSceneCached(sceneKey);
        if (isCached)
        {
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

    public void HandleSceneButton(AssetReference sceneKey, Button button, Text buttonText)
    {
        button.onClick.AddListener(async () =>
        {
            if (_downloadedScenes.ContainsKey(sceneKey.RuntimeKey.ToString()))
            {
                await LoadScene(sceneKey);
            }
            else
            {
                await DownloadScene(sceneKey);
                buttonText.text = $"Play {sceneKey.SubObjectName} Scene";
            }
        });
    }

    #endregion
}
