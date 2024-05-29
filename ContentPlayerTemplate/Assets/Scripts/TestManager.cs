using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
    public VideoPlayerManager videoManager;
    public SceneLoaderManager sceneManager;


    [SerializeField] private AssetReference TestScene;
    [SerializeField] private AssetReference XRDemoScene;
    [SerializeField] private AssetReference ParkourGameScene;

    #region Video Buttons

    [Button("Download Pond Video")]
    public void DownloadPondVideo()
    {
        videoManager.DownloadVideo("Pond");
    }
    
    [Button("Play Pond Video")]
    public void PlayPondVideo()
    {
        videoManager.PlayVideo("Pond");
    }

    [Button("Download Waterfall video")]
    public void DownloadWaterfallVideo()
    {
        videoManager.DownloadVideo("Waterfall");
    }

    [Button("Play Waterfall video")]
    public void PlayWaterfallVideo()
    {
        videoManager.PlayVideo("Waterfall");
    }

    [Button("Download My360Video")]
    public void DownloadMyVideo()
    {
        videoManager.DownloadVideo("My360Video");
    }

    [Button("Play My360Video")]
    public void PlayMyVideo()
    {
        videoManager.PlayVideo("My360Video");
    }

    [Button("Pause Video")]
    public void PauseVideo()
    {
        videoManager.videoPlayer.Pause();
    }

    [Button("Stop video")]
    public void StopVideo()
    {
        videoManager.videoPlayer.Stop();
        videoManager.ClearRenderTexture(videoManager.yourRenderTexture);
    }

    #endregion

    #region Scene Buttons

    [Button("Download XR Demo Scene")]
    public async void DownloadXRDemoScene()
    {
        await sceneManager.DownloadScene(XRDemoScene);
    }

    [Button("Load XR Demo Scene")]
    public async void LoadXRDemoScene()
    {
       await sceneManager.LoadScene(XRDemoScene);
    }

    [Button("Download Test Scene")]
    public async void DownloadTestScene()
    {
        await sceneManager.DownloadScene(TestScene);
    }

    [Button("Load Test Scene")]
    public async void LoadTestScene()
    {
         await sceneManager.LoadScene(TestScene);
    }

    [Button("Download Parkour Game Scene")]
    public async void DownloadParkourGameScene()
    {
        await sceneManager.DownloadScene(ParkourGameScene);
    }

    [Button("Load Parkour Game Scene")]
    public async void LoadParkourGameScene()
    {
         await sceneManager.LoadScene(ParkourGameScene);
    }

    [Button("Return to Lobby")]
    public void LoadLobbyScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }
    #endregion
}
