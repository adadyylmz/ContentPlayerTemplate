using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class ContentCardView : MonoBehaviour
    {
        public event Action<Content> OnViewContentEvent;
        
        [SerializeField]
        RawImage m_ContentRawImage;
        
        [SerializeField]
        TextMeshProUGUI m_ContentNameLabel;

        [SerializeField]
        TextMeshProUGUI m_ContentDescriptionLabel;

        [SerializeField]
        TextMeshProUGUI m_SubscriptionStat;
        
        [SerializeField]
        TextMeshProUGUI m_RateStat;

        [SerializeField]
        TextMeshProUGUI m_ReportStat;

        [SerializeField]
        Button m_ViewButton;
        
        [SerializeField]
        SubscriptionButton m_SubscriptionButton;
        
        Content m_CurrentContent;
        Texture m_DefaultThumbnailTexture;
        Vector2 m_DefaultTextureSize;

        void Start()
        {
            m_DefaultThumbnailTexture = m_ContentRawImage.texture;
            m_DefaultTextureSize = m_ContentRawImage.rectTransform.sizeDelta;
            
            m_ViewButton.onClick.AddListener(OnViewButtonPressed);
            m_SubscriptionButton.onClick.AddListener(OnSubscriptionButtonPressed);
        }

        void OnDestroy()
        {
            m_ViewButton.onClick.RemoveListener(OnViewButtonPressed);
            m_SubscriptionButton.onClick.RemoveListener(OnSubscriptionButtonPressed);
        }

        public async void SetContent(Content content)
        {
            RefreshSubscription();
            if (m_CurrentContent != null && m_CurrentContent.Id == content.Id && m_CurrentContent.ThumbnailMd5Hash == content.ThumbnailMd5Hash)
                return;

            m_CurrentContent = content;
            m_ContentNameLabel.SetText(m_CurrentContent.Name);
            m_ContentDescriptionLabel.SetText(m_CurrentContent.Description);
            m_SubscriptionStat.SetText((m_CurrentContent.Statistics?.SubscriptionsCount?.AllTime ?? 0).ToString());
            m_RateStat.SetText((m_CurrentContent.Statistics?.RatingsAverage?.AllTime ?? 0).ToString("F"));
            m_ReportStat.SetText((m_CurrentContent.Statistics?.ReportsCount?.AllTime ?? 0).ToString());

            try
            {
                await UgcService.Instance.DownloadContentDataAsync(m_CurrentContent, false, true);
            }
            catch (RequestFailedException exception)
            {
                Debug.LogWarning(exception);
            }

            if (m_CurrentContent.DownloadedThumbnail != null && m_CurrentContent.DownloadedThumbnail.Length > 0)
            {
                var texture = new Texture2D(2, 2);
                texture.LoadImage(m_CurrentContent.DownloadedThumbnail);
                m_ContentRawImage.texture = texture;
                m_ContentRawImage.rectTransform.sizeDelta = TextureUtilities.CalculateNewTextureSize(m_ContentRawImage.rectTransform.sizeDelta, texture);
            }
            else
            {
                m_ContentRawImage.texture = m_DefaultThumbnailTexture;
                m_ContentRawImage.rectTransform.sizeDelta = m_DefaultTextureSize;
            }

            gameObject.SetActive(true);
        }

        public void ClearViewEvent()
        {
            OnViewContentEvent = null;
        }
        
        async Task RefreshSubscription()
        {
            var isSubscribed = await UgcService.Instance.IsSubscribedToAsync(m_CurrentContent.Id);
            m_SubscriptionButton.SetSubscriptionState(isSubscribed);
        }

        async Task SubscribeContent()
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

            await RefreshSubscription();
        }

        void OnViewButtonPressed()
        {
            OnViewContentEvent?.Invoke(m_CurrentContent);
        }

        void OnSubscriptionButtonPressed()
        {
            SubscribeContent();
        }
    }
}
