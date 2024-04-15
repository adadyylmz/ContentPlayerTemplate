using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Services.Ugc.Samples
{
    public class CounterButton : Button
    {
        [SerializeField]
        Image m_IconImage;
        
        [SerializeField]
        TextMeshProUGUI m_CountLabel;

        [SerializeField]
        Sprite m_NormalStateIconSprite;
        
        [SerializeField]
        Sprite m_ActiveStateIconSprite;
        
        public void SetState(bool active)
        {
            m_IconImage.sprite = active ? m_ActiveStateIconSprite : m_NormalStateIconSprite;
        }
        
        public void SetCount(int count)
        {
            m_CountLabel.SetText(count.ToString());
        }
    }
}