using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Collections;

public class VideoPlayerManager : MonoBehaviour
{
    #region Fields
    public VideoPlayer videoPlayer;
    private Material _skyMaterial;
    private string videoName;

    [SerializeField] Slider percentDownloadingUI;
    [SerializeField] TMP_Text percentDownloadingText;
    [SerializeField] AsyncOperationHandle<VideoClip> videoLoaderHandler;


    public Dictionary<string, VideoClip> _downloadedClips = new Dictionary<string, VideoClip>();
    #endregion

    #region Unity Methods
    void Start()
    {
        Caching.ClearCache();
        _skyMaterial = RenderSettings.skybox;
        Debug.Log("Video cache is cleared.");

    }

    void Update()
    {
        //Loading slider and UI is updated while it's downloading
        if (videoLoaderHandler.IsValid() && !videoLoaderHandler.IsDone)
        {
            float percent = videoLoaderHandler.GetDownloadStatus().Percent;
            percentDownloadingUI.value = percent;
            percentDownloadingText.text = "Downloading   " + Mathf.Round(percent * 100) + "%";
            Debug.Log(percent);
        }
    }

    #endregion

    #region Public Methods

    // Download video using Addressables
    public void DownloadVideo(string videoAddressableKey)
    {
        percentDownloadingUI.gameObject.SetActive(true);
        percentDownloadingText.gameObject.SetActive(true);

        if (_downloadedClips.ContainsKey(videoAddressableKey))
        {
            Debug.Log("Video with key " + videoAddressableKey + " is already downloaded.");
            return;
        }

        //Download the video
        videoLoaderHandler = Addressables.LoadAssetAsync<VideoClip>(videoAddressableKey);

        //When it's downloaded
        videoLoaderHandler.Completed += (op) =>
        {

            percentDownloadingUI.gameObject.SetActive(false);
            percentDownloadingText.gameObject.SetActive(false);

            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                _downloadedClips.Add(videoAddressableKey, op.Result);
                Debug.Log("Video downloaded successfully: " + videoAddressableKey);

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
    
    #endregion
}
