using UnityEngine;
using DG.Tweening;
using System;
using NaughtyAttributes;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class VideoPlayerManager : MonoBehaviour
{
    #region Fields
    public VideoPlayer videoPlayer;
    public float fadeDuration = 2.0f;
    private Material _skyMaterial;

    #endregion

    #region Unity Methods
    public void Start()
    {
        _skyMaterial = RenderSettings.skybox;
    }

    #endregion

    #region Public Methods
    
    /*
    //For testing playing this locally only for now

    //Loading video using addressables
    [Button("Play Waterfall video")]
    public void LoadVideoWaterfall()
    {
        LoadVideo("Waterfall");
        Debug.Log("Waterfall video loaded.");
    }
    */

    //Loading video using addressables
    [Button("Play Pond video")]
    public void LoadVideoPond()
    {
        LoadVideo("Pond");
        Debug.Log("Pond video loaded.");
    }


    //For testing playing this locally only for now

    [Button("Play Waterfall video")]
    public void PlayVideoWaterfall()
    {
        PlayVideo("Waterfall");
        Debug.Log("Waterfall video is being played.");

    }

    [Button("Pause Video")]
    public void PauseVideo()
    {
        videoPlayer.Pause();
        Debug.Log("Video is paused.");

    }

    #endregion

    #region Private Methods

    //Play locally
    private void PlayVideo(string videoClipName)
    {
        VideoClip clipToPlay = FindVideoClip(videoClipName);
        if (clipToPlay != null)
        {
                videoPlayer.clip = clipToPlay;
                videoPlayer.Play();
        }
        else
        {
            Debug.LogWarning("Video clip with name " + videoClipName + " not found!");
        }
    }

    //Loading video using addressables
    private void LoadVideo(string videoAddressableKey)
    {
        AsyncOperationHandle<VideoClip> handle = Addressables.LoadAssetAsync<VideoClip>(videoAddressableKey);

        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                VideoClip videoClip = op.Result;
                videoPlayer.clip = videoClip;

                videoPlayer.Play();
            }
            else
            {
                Debug.LogError("Failed to load video: " + op.OperationException);
            }
        };
    }

    private VideoClip FindVideoClip(string clipName)
    {
        return Resources.Load<VideoClip>(clipName);
    }

    #endregion

}
