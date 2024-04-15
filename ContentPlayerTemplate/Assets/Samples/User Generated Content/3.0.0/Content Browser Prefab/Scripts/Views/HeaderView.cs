using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class HeaderView : MonoBehaviour
    {
        public event Action OnAllContentTabSelectedEvent;
        public event Action OnSubscribedTabSelectedEvent;
        public event Action OnCloseContentBrowserPressedEvent;

        [SerializeField]
        TextMeshProUGUI m_GameNameLabel;

        [SerializeField]
        HeaderTabButton m_AllContentTabButton;

        [SerializeField]
        HeaderTabButton m_SubscribedTabButton;

        [SerializeField]
        Button m_CloseContentBrowserButton;

        void Start()
        {
            m_GameNameLabel.SetText(Application.productName);

            m_AllContentTabButton.onClick.AddListener(OnAllContentTabSelected);
            m_SubscribedTabButton.onClick.AddListener(OnSubscribedTabSelected);
            m_CloseContentBrowserButton.onClick.AddListener(OnCloseContentBrowserPressed);
        }

        void OnDestroy()
        {
            m_AllContentTabButton.onClick.RemoveListener(OnAllContentTabSelected);
            m_SubscribedTabButton.onClick.RemoveListener(OnSubscribedTabSelected);
            m_CloseContentBrowserButton.onClick.RemoveListener(OnCloseContentBrowserPressed);
        }

        public void RefreshTabButtonsState(ContentLibraryType contentLibraryType)
        {
            m_AllContentTabButton.SetState(contentLibraryType == ContentLibraryType.AllContent);
            m_SubscribedTabButton.SetState(contentLibraryType == ContentLibraryType.Subscribed);
        }

        void OnAllContentTabSelected()
        {
            OnAllContentTabSelectedEvent?.Invoke();
        }

        void OnSubscribedTabSelected()
        {
            OnSubscribedTabSelectedEvent?.Invoke();
        }

        void OnCloseContentBrowserPressed()
        {
            OnCloseContentBrowserPressedEvent?.Invoke();
        }
    }
}
