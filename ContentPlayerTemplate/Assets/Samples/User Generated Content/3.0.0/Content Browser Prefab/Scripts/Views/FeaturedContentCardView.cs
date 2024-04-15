using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class FeaturedContentCardView : MonoBehaviour
    {
        public event Action OnViewButtonPressedEvent;

        [SerializeField]
        TextMeshProUGUI m_ContentNameLabel;

        [SerializeField]
        RawImage m_ContentRawImage;

        [SerializeField]
        Button m_ViewButton;

        [SerializeField]
        SubscriptionButton m_SubscribeButton;

        Content m_CurrentContent;
        Texture m_DefaultThumbnailTexture;
        Vector2 m_DefaultTextureSize;

        void Start()
        {
            m_DefaultThumbnailTexture = m_ContentRawImage.texture;
            m_DefaultTextureSize = m_ContentRawImage.rectTransform.sizeDelta;
            
            m_ViewButton.onClick.AddListener(OnViewButtonPressed);
            m_SubscribeButton.onClick.AddListener(OnSubscribeButtonPressed);
        }

        void OnDestroy()
        {
            m_ViewButton.onClick.RemoveListener(OnViewButtonPressed);
            m_SubscribeButton.onClick.RemoveListener(OnSubscribeButtonPressed);
        }

        public async Task SetContent(Content content)
        {
            if (m_CurrentContent != null && m_CurrentContent.Id == content.Id && m_CurrentContent.ThumbnailMd5Hash == content.ThumbnailMd5Hash)
            {
                return;
            }

            m_CurrentContent = content;
            m_ContentNameLabel.SetText(m_CurrentContent.Name);

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

            await RefreshSubscription();
        }

        async Task RefreshSubscription()
        {
            var isSubscribed = await UgcService.Instance.IsSubscribedToAsync(m_CurrentContent.Id);
            m_SubscribeButton.SetSubscriptionState(isSubscribed);
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
            OnViewButtonPressedEvent?.Invoke();
        }

        void OnSubscribeButtonPressed()
        {
            SubscribeContent();
        }
    }
}
