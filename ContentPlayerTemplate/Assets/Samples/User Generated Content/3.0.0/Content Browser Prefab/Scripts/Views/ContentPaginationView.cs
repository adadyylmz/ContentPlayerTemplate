using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class ContentPaginationView : MonoBehaviour
    {
        public event Action<int> OnPageChangedEvent;
        
        public int currentPageIndex => m_CurrentPageIndex;

        [SerializeField]
        int m_MaxPagesDisplayed;

        [SerializeField]
        Button m_PreviousPageButton;

        [SerializeField]
        Button m_NextPageButton;

        [SerializeField]
        GameObject m_StartTruncatedIcon;

        [SerializeField]
        GameObject m_EndTruncatedIcon;

        [SerializeField]
        PageButton m_PageButtonTemplate;

        [SerializeField]
        HorizontalLayoutGroup m_PageButtonsGroup;

        List<PageButton> m_PageButtons;

        int m_CurrentPageIndex;
        int m_PagesCount;

        void Awake()
        {
            m_PageButtons = new List<PageButton>();
            for (int i = 0; i < m_MaxPagesDisplayed; i++)
            {
                var pageButton = Instantiate(m_PageButtonTemplate, m_PageButtonsGroup.transform);
                pageButton.SetSelectedState(false);

                m_PageButtons.Add(pageButton);
            }
        }

        void Start()
        {
            m_PreviousPageButton.onClick.AddListener(OnPreviousPagePressed);
            m_NextPageButton.onClick.AddListener(OnNextPagePressed);
        }
        
        public void ResetPaginationIndex()
        {
            m_CurrentPageIndex = 0;
            UpdatePaginationSelection();
        }

        public void RefreshPagination(int pagesCount)
        {
            m_PagesCount = pagesCount;
            UpdatePaginationSelection();
        }

        void UpdatePaginationSelection()
        {
            var truncateStart = m_PagesCount > m_MaxPagesDisplayed && m_CurrentPageIndex - 2 > 0;
            m_StartTruncatedIcon.SetActive(truncateStart);

            var truncateEnd = m_PagesCount > m_MaxPagesDisplayed && m_CurrentPageIndex + 2 < m_PagesCount - 1;
            m_EndTruncatedIcon.SetActive(truncateEnd);

            var startingPageIndex = truncateStart ? m_CurrentPageIndex - 2 : 0;
            var endingPageIndex = truncateEnd ? m_CurrentPageIndex + 2 : m_PagesCount - 1;
            var nextPageDisplayedCount = endingPageIndex - startingPageIndex;

            for (int i = 0; i < m_PageButtons.Count; i++)
            {
                var pageIndex = i + startingPageIndex;
                var pageButton = m_PageButtons[i];
                if (pageIndex <= nextPageDisplayedCount)
                {
                    pageButton.SetPageIndex(pageIndex);
                    pageButton.SetSelectedState(pageIndex == m_CurrentPageIndex);
                    
                    pageButton.onClick.RemoveAllListeners();
                    pageButton.onClick.AddListener(() => OnPageSelected(pageIndex));
                    
                    pageButton.gameObject.SetActive(true);
                }
                else
                {
                    pageButton.gameObject.SetActive(false);
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
        
        void OnPageSelected(int pageIndex)
        {
            m_CurrentPageIndex = pageIndex;
            UpdatePaginationSelection();
            
            OnPageChangedEvent?.Invoke(m_CurrentPageIndex);
        }

        void OnPreviousPagePressed()
        {
            var pageIndex = m_CurrentPageIndex - 1;
            if (pageIndex <= 0)
            {
                return;
            }

            m_CurrentPageIndex = pageIndex;
            UpdatePaginationSelection();
            
            OnPageChangedEvent?.Invoke(m_CurrentPageIndex);
        }

        void OnNextPagePressed()
        {
            var pageIndex = m_CurrentPageIndex + 1;
            if (pageIndex >= m_PagesCount)
            {
                return;
            }
            
            m_CurrentPageIndex = pageIndex;
            UpdatePaginationSelection();
            
            OnPageChangedEvent?.Invoke(m_CurrentPageIndex);
        }
    }
}
