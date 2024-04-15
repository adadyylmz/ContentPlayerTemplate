using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class PageButton : Button
    {
        [SerializeField]
        TextMeshProUGUI m_PageIndexLabel;

        [SerializeField]
        Image m_SelectedImage;

        public void SetPageIndex(int index)
        {
            index++; // increment to fit 1-based listing
            m_PageIndexLabel.SetText(index.ToString());
        }

        public void SetSelectedState(bool isSelected)
        {
            m_SelectedImage.gameObject.SetActive(isSelected);
        }
    }
}
