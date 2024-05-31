using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
    public VideoPlayerManager videoManager;
    public SceneLoaderManager sceneManager;

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
        public Button button;
        public Text buttonText;
    }

    public List<VideoButton> videoButtons;
    public List<SceneButton> sceneButtons;

    private void Start()
    {
        InitializeVideoButtons();
        InitializeSceneButtons();
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
            sceneManager.HandleSceneButton(sceneButton.sceneReference, sceneButton.button, sceneButton.buttonText);
        }
    }

    #region Scene Buttons

    [Button("Return to Lobby")]
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    #endregion
}
