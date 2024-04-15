using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.Ugc.Samples
{
    public class FeaturedContentView : MonoBehaviour
    {
        public event Action<Content> OnViewContentEvent;

        [SerializeField, Range(1, 25)]
        int m_FeaturedContentCount;

        [SerializeField]
        FeaturedContentCardView m_FeaturedContentCardView;

        [SerializeField]
        FeaturedContentButton m_PreviousCardButton;

        [SerializeField]
        FeaturedContentButton m_NextCardButton;

        int m_CurrentFeaturedCardIndex;
        List<Content> m_FeaturedContentList;

        void Start()
        {
            m_FeaturedContentCardView.OnViewButtonPressedEvent += OnViewContentPressed;

            m_PreviousCardButton.onClick.AddListener(OnPreviousCardButtonPressed);
            m_NextCardButton.onClick.AddListener(OnNextCardButtonPressed);
        }

        void OnDestroy()
        {
            m_FeaturedContentCardView.OnViewButtonPressedEvent -= OnViewContentPressed;

            m_PreviousCardButton.onClick.RemoveListener(OnPreviousCardButtonPressed);
            m_NextCardButton.onClick.RemoveListener(OnNextCardButtonPressed);
        }

        public async Task RefreshFeaturedContentList()
        {
            m_CurrentFeaturedCardIndex = 0;

            var requestArgs = new GetContentsArgs() {Tags = new List<string>() { "1" }};
            requestArgs.AddSortBy(SearchContentSortBy.ContentEnvironmentStatistics_Data_SubscriptionsCount_AllTime, true);

            var pagedResults = await UgcService.Instance.GetContentsAsync(requestArgs);
            var minContentCount = Math.Min(pagedResults.Results.Count, m_FeaturedContentCount);
            m_FeaturedContentList = pagedResults.Results.GetRange(0, minContentCount);
            
            Debug.Log($"Fetched {pagedResults.Total} featured content");

            UpdateFeaturedContent();
        }

        void UpdateFeaturedContent()
        {
            // set previous content card/button
            var previousContentIndex = m_CurrentFeaturedCardIndex - 1;
            if (previousContentIndex < 0)
            {
                previousContentIndex = m_FeaturedContentList.Count - 1;
            }

            var previousContent = m_FeaturedContentList[previousContentIndex];
            m_PreviousCardButton.SetContent(previousContent);

            // set current content card
            var currentContent = m_FeaturedContentList[m_CurrentFeaturedCardIndex];
            m_FeaturedContentCardView.SetContent(currentContent);

            // set next content card/button
            var nextContentIndex = m_CurrentFeaturedCardIndex + 1;
            if (nextContentIndex >= m_FeaturedContentList.Count)
            {
                nextContentIndex = 0;
            }

            var nextContent = m_FeaturedContentList[nextContentIndex];
            m_NextCardButton.SetContent(nextContent);
        }

        void OnViewContentPressed()
        {
            OnViewContentEvent?.Invoke(m_FeaturedContentList[m_CurrentFeaturedCardIndex]);
        }

        void OnPreviousCardButtonPressed()
        {
            m_CurrentFeaturedCardIndex--;
            if (m_CurrentFeaturedCardIndex < 0)
            {
                m_CurrentFeaturedCardIndex = m_FeaturedContentList.Count - 1;
            }

            UpdateFeaturedContent();
        }

        void OnNextCardButtonPressed()
        {
            m_CurrentFeaturedCardIndex++;
            if (m_CurrentFeaturedCardIndex >= m_FeaturedContentList.Count)
            {
                m_CurrentFeaturedCardIndex = 0;
            }

            UpdateFeaturedContent();
        }
    }
}
