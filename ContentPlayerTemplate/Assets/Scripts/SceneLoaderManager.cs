using UnityEngine.SceneManagement;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
public class SceneLoaderManager : MonoBehaviour
{
    #region Fields

    //[SerializeField] private AssetReference EmptyScene;
    //[SerializeField] private AssetReference LobbyScene;

    [SerializeField] private AssetReference XRDemoScene;
    [SerializeField] private AssetReference ShootingGameScene;
    [SerializeField] AsyncOperationHandle<SceneInstance> _sceneLoaderHandler;

    #endregion

    #region Public Methods


    [Button("Load XR Demo Scene")]
    public async void LoadXRDemoScene()
    {
        await LoadAddressableScene(XRDemoScene);
        Debug.Log("XR Demo scene loaded remotely.");

    }

    /*
    //Test scene is causing error: unknown
    [Button("Load Test Scene")]
    public async void LoadEmptyScene()
    {
        await LoadAddressableScene(EmptyScene);
        Debug.Log("Empty scene loaded remotely.");

    }
    */

    [Button("Load Shooting Game Scene")]
    public async void LoadShootingGameScene()
    {
        await LoadAddressableScene(ShootingGameScene);
        Debug.Log("Shooting game scene loaded remotely.");
    }

    [Button("Return to Lobby")]
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
        Debug.Log("Returned to lobby.");
    }

    #endregion

    #region Private Methods
    private async UniTask LoadAddressableScene(AssetReference sceneKey)
    {
        _sceneLoaderHandler = Addressables.LoadSceneAsync(sceneKey, LoadSceneMode.Single);
        await _sceneLoaderHandler.Task;
        Debug.Log("Addrassable scene loaded");

    }
    #endregion
}