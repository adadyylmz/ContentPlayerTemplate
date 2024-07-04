using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
    public VideoPlayerManager_Addressables videoManager;
    public SceneManager_Addressables sceneManager;

    [System.Serializable]
    public struct VideoButton
    {
        public string videoName;
        public Button button;
        public Text buttonText;
    }

    [System.Serializable]
    public struct SceneButton
    {
        public AssetReference sceneReference;
        public string sceneName;  // Added sceneName property
        public Button button;
        public Text buttonText;
    }

    public List<VideoButton> videoButtons;
    public List<SceneButton> sceneButtons;

    public Button pauseVideoButton;
    public Button stopVideoButton;

    private void Start()
    {
        InitializeVideoButtons();
        InitializeSceneButtons();
        InitializeControlButtons();
    }

    private void InitializeVideoButtons()
    {
        foreach (var videoButton in videoButtons)
        {
            videoManager.HandleVideoButton(videoButton.videoName, videoButton.button, videoButton.buttonText);
        }
    }

    private void InitializeSceneButtons()
    {
        foreach (var sceneButton in sceneButtons)
        {
            sceneManager.HandleSceneButton(sceneButton.sceneReference, sceneButton.button, sceneButton.buttonText, sceneButton.sceneName);
        }
    }

    private void InitializeControlButtons()
    {
        pauseVideoButton.onClick.AddListener(videoManager.PauseVideo);
        stopVideoButton.onClick.AddListener(videoManager.StopVideo);
    }

    #region Scene Buttons

    
    [Button("Return to Lobby")]
    public void LoadLobbyScene()
    {
        sceneManager.LoadLobbyScene();
    }
    
    #endregion
}
