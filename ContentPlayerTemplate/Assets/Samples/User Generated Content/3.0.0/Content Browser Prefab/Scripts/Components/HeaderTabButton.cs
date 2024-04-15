using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class HeaderTabButton : Button
    {
        [SerializeField]
        TextMeshProUGUI m_ButtonLabel;

        [SerializeField]
        Color m_NormalStateLabelColor;

        [SerializeField]
        Color m_SelectedStateLabelColor;

        [SerializeField]
        TMP_FontAsset m_NormalStateLabelFont;

        [SerializeField]
        TMP_FontAsset m_SelectedStateLabelFont;

        [SerializeField]
        Image m_SelectedStateOutlineImage;

        bool m_IsSelectedState;

        protected override void Start()
        {
            onClick.AddListener(OnButtonPressed);
        }

        protected override void OnDestroy()
        {
            onClick.RemoveListener(OnButtonPressed);
        }

        public void SetState(bool isSelected)
        {
            if (!isSelected)
            {
                SetButtonNormalState();
            }
            else
            {
                SetButtonSelectedState();
            }
        }

        void SetButtonNormalState()
        {
            m_ButtonLabel.color = m_NormalStateLabelColor;
            m_ButtonLabel.font = m_NormalStateLabelFont;
            m_ButtonLabel.UpdateFontAsset();

            m_SelectedStateOutlineImage.gameObject.SetActive(false);
            m_IsSelectedState = false;
        }

        void SetButtonSelectedState()
        {
            m_ButtonLabel.color = m_SelectedStateLabelColor;
            m_ButtonLabel.font = m_SelectedStateLabelFont;
            m_ButtonLabel.UpdateFontAsset();

            m_SelectedStateOutlineImage.gameObject.SetActive(true);
            m_IsSelectedState = true;
        }

        void OnButtonPressed()
        {
            if (!m_IsSelectedState)
            {
                SetButtonSelectedState();
            }
        }
    }
}
