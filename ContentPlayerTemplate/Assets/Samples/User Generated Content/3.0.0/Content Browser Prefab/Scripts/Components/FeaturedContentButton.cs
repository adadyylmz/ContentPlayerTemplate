using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class FeaturedContentButton : Button
    {
        [SerializeField]
        RawImage m_ContentRawImage;

        Content m_CurrentContent;
        Texture m_DefaultThumbnailTexture;
        Vector2 m_DefaultTextureSize;

        protected override void Start()
        {
            m_DefaultThumbnailTexture = m_ContentRawImage.texture;
            m_DefaultTextureSize = m_ContentRawImage.rectTransform.sizeDelta;
        }

        public async Task SetContent(Content content)
        {
            if (m_CurrentContent != null && m_CurrentContent.Id == content.Id && m_CurrentContent.ThumbnailMd5Hash == content.ThumbnailMd5Hash)
            {
                return;
            }

            m_CurrentContent = content;

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
        }
    }
}
