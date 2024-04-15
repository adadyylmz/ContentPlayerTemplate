using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Services.Ugc.Samples
{
    public class ContentGroupView : MonoBehaviour
    {
        public event Action<Content> OnViewContentEvent;
        
        [SerializeField]
        ContentCardView m_ContentCardViewPrefab;

        [SerializeField]
        Transform m_ContentCardViewsRoot;
    
        List<ContentCardView> m_ContentCardViews;

        public void Initialize(int contentPerPage)
        {
            m_ContentCardViews = new List<ContentCardView>();
            for (int i = 0; i < contentPerPage; i++)
            {
                var contentCardView = Instantiate(m_ContentCardViewPrefab, m_ContentCardViewsRoot);
                m_ContentCardViews.Add(contentCardView);
                
                contentCardView.gameObject.SetActive(false);
            }
        }

        public void RefreshContentList(Content[] pageContent)
        {
            Debug.Log($"Refreshing Content with {pageContent.Length} items");

            for (int i = 0; i < m_ContentCardViews.Count; i++)
            {
                if (i < pageContent.Length)
                {
                    m_ContentCardViews[i].SetContent(pageContent[i]);

                    m_ContentCardViews[i].ClearViewEvent();
                    m_ContentCardViews[i].OnViewContentEvent += OnViewContent;
                    m_ContentCardViews[i].gameObject.SetActive(true);
                }
                else
                {
                    m_ContentCardViews[i].gameObject.SetActive(false);
                }
            }
        }

        void OnViewContent(Content content)
        {
            OnViewContentEvent?.Invoke(content);
        }
    }
}
