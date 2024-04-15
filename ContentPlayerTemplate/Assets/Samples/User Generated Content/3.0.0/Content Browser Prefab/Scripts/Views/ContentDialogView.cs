using System;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Ugc.Generated.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class ContentDialogView : MonoBehaviour
    {
        public event Action OnDialogViewClosedEvent;
        
        public event Action<Content> OnContentPlayedEvent;
        
        [SerializeField]
        Button m_CloseButton;

        [SerializeField]
        TextMeshProUGUI m_ContentNameText;

        [SerializeField]
        RawImage m_ThumbnailRawImage;

        [SerializeField]
        TextMeshProUGUI m_ContentDescriptionText;

        [SerializeField]
        TextMeshProUGUI m_AuthorText;

        [SerializeField]
        TextMeshProUGUI m_SubscriptionText;

        [SerializeField]
        TextMeshProUGUI m_RatingText;

        [SerializeField]
        TextMeshProUGUI m_ReportText;

        [SerializeField]
        TextMeshProUGUI m_UpdatedText;

        [SerializeField]
        CounterButton m_RateButton;

        [SerializeField]
        CounterButton m_ReportButton;

        [SerializeField]
        SubscriptionButton m_SubscriptionButton;

        [SerializeField]
        Button m_PlayButton;

        Content m_CurrentContent;
        Texture m_DefaultThumbnailTexture;
        Vector2 m_DefaultTextureSize;

        void Start()
        {
            m_DefaultThumbnailTexture = m_ThumbnailRawImage.texture;
            m_DefaultTextureSize = m_ThumbnailRawImage.rectTransform.sizeDelta;
            
            m_CloseButton.onClick.AddListener(OnCloseButtonPressed);
            m_RateButton.onClick.AddListener(OnRateButtonPressed);
            m_ReportButton.onClick.AddListener(OnReportButtonPressed);
            m_SubscriptionButton.onClick.AddListener(OnSubscribeButtonPressed);
            m_PlayButton.onClick.AddListener(OnPlayButtonPressed);
        }

        void OnDestroy()
        {
            m_CloseButton.onClick.RemoveListener(OnCloseButtonPressed);
            m_RateButton.onClick.RemoveListener(OnRateButtonPressed);
            m_ReportButton.onClick.RemoveListener(OnReportButtonPressed);
            m_SubscriptionButton.onClick.RemoveListener(OnSubscribeButtonPressed);
            m_PlayButton.onClick.RemoveListener(OnPlayButtonPressed);
        }

        public void ShowContent(Content content)
        {
            m_CurrentContent = content;

            GetThumbnail();
            m_ContentNameText.SetText(m_CurrentContent.Name);
            m_ContentDescriptionText.SetText(m_CurrentContent.Description);

            // stats
            m_AuthorText.SetText(m_CurrentContent.CreatorAccountId);
            var subscriptionCount = m_CurrentContent.Statistics?.SubscriptionsCount?.AllTime ?? 0;
            m_SubscriptionText.SetText(subscriptionCount.ToString());
            var averageRating = m_CurrentContent.Statistics?.RatingsAverage?.AllTime ?? 0;
            m_RatingText.SetText(m_CurrentContent.AverageRating != null ? $"{averageRating:F}/5" : "Not enough ratings.");
            m_ReportText.SetText(m_CurrentContent.CreatedAt.ToString());
            m_UpdatedText.SetText(m_CurrentContent.UpdatedAt.ToString());

            // buttons
            UpdateRateButtons();
            UpdateReportButton();
            UpdateSubscriptionButton();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void OnCloseButtonPressed()
        {
            Hide();
            OnDialogViewClosedEvent?.Invoke();
        }

        void OnSubscribeButtonPressed()
        {
            SubscribeToContent();
        }

        void OnPlayButtonPressed()
        {
            OnContentPlayedEvent?.Invoke(m_CurrentContent);
        }

        void OnRateButtonPressed()
        {
            RateContent(true);
        }

        void OnReportButtonPressed()
        {
            ReportContent();
        }
        
        async void SubscribeToContent()
        {
            try
            {
                var isSubscribed = await UgcService.Instance.IsSubscribedToAsync(m_CurrentContent.Id);
                if (isSubscribed)
                {
                    await UgcService.Instance.DeleteSubscriptionAsync(m_CurrentContent.Id);
                }
                else
                {
                    await UgcService.Instance.CreateSubscriptionAsync(m_CurrentContent.Id);
                }
            }
            catch (RequestFailedException exception)
            {
                Debug.LogWarning(exception);
            }

            UpdateSubscriptionButton();
        }

        async void RateContent(bool rateUp)
        {
            var rate = rateUp ? 5f : 1f;
            await UgcService.Instance.SubmitUserContentRatingAsync(m_CurrentContent.Id, rate);

            UpdateRateButtons();
        }

        async void ReportContent()
        {
            var reportContentArgs = new ReportContentArgs(m_CurrentContent.Id, Reason.Inappropriate);
            await UgcService.Instance.ReportContentAsync(reportContentArgs);

            UpdateReportButton();
        }

        async void GetThumbnail()
        {
            if (m_CurrentContent.DownloadedThumbnail == null || m_CurrentContent.DownloadedThumbnail.Length == 0)
            {
                try
                {
                    await UgcService.Instance.DownloadContentDataAsync(m_CurrentContent, false, true);
                }
                catch (RequestFailedException exception)
                {
                    Debug.LogWarning(exception);
                }
            }

            if (m_CurrentContent.DownloadedThumbnail != null && m_CurrentContent.DownloadedThumbnail.Length > 0)
            {
                var texture = new Texture2D(2, 2);
                texture.LoadImage(m_CurrentContent.DownloadedThumbnail);
                m_ThumbnailRawImage.texture = texture;
                m_ThumbnailRawImage.rectTransform.sizeDelta = TextureUtilities.CalculateNewTextureSize(m_ThumbnailRawImage.rectTransform.sizeDelta, texture);
            }
            else
            {
                m_ThumbnailRawImage.texture = m_DefaultThumbnailTexture;
                m_ThumbnailRawImage.rectTransform.sizeDelta = m_DefaultTextureSize;
            }
        }

        async void UpdateSubscriptionButton()
        {
            try
            {
                var isSubscribed = await UgcService.Instance.IsSubscribedToAsync(m_CurrentContent.Id);
                m_SubscriptionButton.SetSubscriptionState(isSubscribed);
            }
            catch (RequestFailedException exception)
            {
                Debug.LogWarning(exception);
            }
        }

        async void UpdateRateButtons()
        {
            try
            {
                var contentUserRating = await UgcService.Instance.GetUserContentRatingAsync(m_CurrentContent.Id);
                m_RateButton.SetState(contentUserRating.CreatedAt != null);
                m_RateButton.SetCount(m_CurrentContent.Statistics?.RatingsCount?.AllTime ?? 0);
            }
            catch (RequestFailedException exception)
            {
                Debug.LogWarning(exception);
            }
        }

        void UpdateReportButton()
        {
            m_ReportButton.SetCount(m_CurrentContent.Statistics?.ReportsCount?.AllTime ?? 0);
        }
    }
}
