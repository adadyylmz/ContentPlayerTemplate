using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class SubscriptionButton : Button
    {
        const string k_SubscriptionText = "Subscribe";
        const string k_SubscribedText = "Subscribed";

        [SerializeField]
        Color m_SubscriptionColor;

        [SerializeField]
        Color m_SubscribedColor;

        [SerializeField]
        Sprite m_SubscriptionIcon;

        [SerializeField]
        Sprite m_SubscribedIcon;

        [SerializeField]
        TextMeshProUGUI m_ButtonLabel;

        [SerializeField]
        Image m_ButtonImage;

        [SerializeField]
        Image m_IconImage;

        bool m_IsSubscribed;

        public void SetSubscriptionState(bool toggle)
        {
            m_IsSubscribed = toggle;
            m_ButtonImage.color = m_IsSubscribed ? m_SubscribedColor : m_SubscriptionColor;
            m_ButtonLabel.SetText(m_IsSubscribed ? k_SubscribedText : k_SubscriptionText);
            m_IconImage.sprite = m_IsSubscribed ? m_SubscribedIcon : m_SubscriptionIcon;
        }
    }
}
