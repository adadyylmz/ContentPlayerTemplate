using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class VideoPlayerManager : MonoBehaviour
{
    #region Fields
    public VideoPlayer videoPlayer;
    private Material _skyMaterial;
    private Texture initialSkyTexture;
    public RenderTexture yourRenderTexture; // Assign your Render Texture here

    [SerializeField] Slider percentDownloadingUI;
    [SerializeField] TMP_Text percentDownloadingText;
    [SerializeField] AsyncOperationHandle<VideoClip> videoLoaderHandler;

    public Dictionary<string, VideoClip> _downloadedClips = new Dictionary<string, VideoClip>();

    // A dictionary to hold buttons and their text components
    private Dictionary<string, (Button, Text)> videoButtons = new Dictionary<string, (Button, Text)>();

    #endregion

    #region Unity Methods
    void Start()
    {
        Caching.ClearCache();
        _skyMaterial = RenderSettings.skybox;
        initialSkyTexture = _skyMaterial.GetTexture("_MainTex"); // Store the initial skybox texture
        Debug.Log("Video cache is cleared.");
    }

    void Update()
    {
        // Loading slider and UI is updated while it's downloading
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

    // General method to handle video download and button update
    public void HandleVideoButton(string videoName, Button button, Text buttonText)
    {
        if (_downloadedClips.ContainsKey(videoName))
        {
            // If the video is already downloaded, set the button to play the video
            buttonText.text = $"Play {videoName} video";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => PlayVideo(videoName));
        }
        else
        {
            // If the video is not downloaded, set the button to download the video
            buttonText.text = $"Download {videoName} video";
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => DownloadVideo(videoName, button, buttonText));
        }

        // Store button and text reference in the dictionary
        if (!videoButtons.ContainsKey(videoName))
        {
            videoButtons.Add(videoName, (button, buttonText));
        }
    }

    // Download video using Addressables
    public void DownloadVideo(string videoAddressableKey, Button button, Text buttonText)
    {
        percentDownloadingUI.gameObject.SetActive(true);
        percentDownloadingText.gameObject.SetActive(true);

        if (_downloadedClips.ContainsKey(videoAddressableKey))
        {
            Debug.Log("Video with key " + videoAddressableKey + " is already downloaded.");
            return;
        }

        // Download the video
        videoLoaderHandler = Addressables.LoadAssetAsync<VideoClip>(videoAddressableKey);

        // When it's downloaded
        videoLoaderHandler.Completed += (op) =>
        {
            percentDownloadingUI.gameObject.SetActive(false);
            percentDownloadingText.gameObject.SetActive(false);

            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _downloadedClips.Add(videoAddressableKey, op.Result);
                Debug.Log("Video downloaded successfully: " + videoAddressableKey);

                // Change button text to "Play ${videoName} video"
                buttonText.text = $"Play {videoAddressableKey} video";

                // Update button functionality to play the video
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => PlayVideo(videoAddressableKey));
            }
            else
            {
                Debug.Log("Failed to download video: " + op.OperationException);
            }
        };
    }

    // Play video by addressable key from cache
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

    // Stop video playback and clear the render texture
    public void StopVideo()
    {
        videoPlayer.Stop();
        ClearRenderTexture(yourRenderTexture);
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    // Clear the render texture
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
