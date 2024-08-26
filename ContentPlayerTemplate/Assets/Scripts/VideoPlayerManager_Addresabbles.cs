using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class VideoPlayerManager_Addressables : MonoBehaviour
{
    #region Fields
    public VideoPlayer videoPlayer;
    private Material _skyMaterial;
    private Texture initialSkyTexture;
    public RenderTexture yourRenderTexture;

    [SerializeField] Slider percentDownloadingUI;
    [SerializeField] TMP_Text percentDownloadingText;
    [SerializeField] AsyncOperationHandle<VideoClip> videoLoaderHandler;

    public Dictionary<string, VideoClip> _downloadedClips = new Dictionary<string, VideoClip>();

    private Dictionary<string, (Button, Text)> videoButtons = new Dictionary<string, (Button, Text)>();

    #endregion

    #region Unity Methods

    void Start()
    {
        Caching.ClearCache();
        _skyMaterial = RenderSettings.skybox;
        initialSkyTexture = _skyMaterial.GetTexture("_MainTex");
        Debug.Log("Video cache is cleared.");
    }

    void Update()
    {
        if (videoLoaderHandler.IsValid() && !videoLoaderHandler.IsDone)
        {
            float percent = videoLoaderHandler.GetDownloadStatus().Percent;
            percentDownloadingUI.value = percent;
            percentDownloadingText.text = "Downloading   " + Mathf.Round(percent * 100) + "%";
            Debug.Log(percent);
        }
    }

    void OnApplicationQuit()
    {
        ClearRenderTexture(yourRenderTexture);
    }

    #endregion

    #region Public Methods

    // Handles setting up video buttons based on whether the video is downloaded or needs to be downloaded
    public void HandleVideoButton(string videoName, Button button, Text buttonText)
    {
        if (_downloadedClips.ContainsKey(videoName))
        {
            buttonText.text = $"Play {videoName} video";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => PlayVideo(videoName));
        }
        else
        {
            buttonText.text = $"Download {videoName} video";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => DownloadVideo(videoName, button, buttonText));
        }

        if (!videoButtons.ContainsKey(videoName))
        {
            videoButtons.Add(videoName, (button, buttonText));
        }
    }

    // Initiates video download via Addressables and updates UI during download
    public void DownloadVideo(string videoAddressableKey, Button button, Text buttonText)
    {
        percentDownloadingUI.gameObject.SetActive(true);
        percentDownloadingText.gameObject.SetActive(true);

        if (_downloadedClips.ContainsKey(videoAddressableKey))
        {
            Debug.Log("Video with key " + videoAddressableKey + " is already downloaded.");
            return;
        }

        videoLoaderHandler = Addressables.LoadAssetAsync<VideoClip>(videoAddressableKey);

        videoLoaderHandler.Completed += (op) =>
        {
            percentDownloadingUI.gameObject.SetActive(false);
            percentDownloadingText.gameObject.SetActive(false);

            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _downloadedClips.Add(videoAddressableKey, op.Result);
                Debug.Log("Video downloaded successfully: " + videoAddressableKey);

                buttonText.text = $"Play {videoAddressableKey} video";

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => PlayVideo(videoAddressableKey));
            }
            else
            {
                Debug.Log("Failed to download video: " + op.OperationException);
            }
        };
    }

    // Plays the video if it has already been downloaded
    public void PlayVideo(string videoAddressableKey)
    {
        if (_downloadedClips.ContainsKey(videoAddressableKey))
        {
            VideoClip clipToPlay = _downloadedClips[videoAddressableKey];
            videoPlayer.clip = clipToPlay;
            videoPlayer.Play();
        }
        else
        {
            Debug.Log("Video with key " + videoAddressableKey + " not found!");
        }
    }

    // Plays the currently loaded video
    public void PlayVideo()
    {
        videoPlayer.Play();
    }

    // Stops the video and clears the render texture
    public void StopVideo()
    {
        videoPlayer.Stop();
        ClearRenderTexture(yourRenderTexture);
    }

    // Pauses the video
    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    // Clears the given render texture by setting it to a black color
    public void ClearRenderTexture(RenderTexture rt)
    {
        if (rt != null)
        {
            RenderTexture temp = RenderTexture.active;
            RenderTexture.active = rt;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = temp;
            Debug.Log("Render Texture has been cleared.");
        }
    }

    #endregion
}
