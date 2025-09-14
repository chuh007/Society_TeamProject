using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatUI
{
    public class ChoiceButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image background;

        [SerializeField] private float paddingTopBottom = 10f;
        [SerializeField] private float minHeight = 40f; 

        public void Setup(string text, Color color, UnityAction onClick)
        {
            if (label != null) label.text = text;
            if (background != null) background.color = color;
            
            if (label != null && background != null)
            {
                float preferredHeight = label.preferredHeight + paddingTopBottom * 2;
                float finalHeight = Mathf.Max(preferredHeight, minHeight); 

                RectTransform bgRect = background.rectTransform;
                Vector2 size = bgRect.sizeDelta;
                size.y = finalHeight;
                bgRect.sizeDelta = size;
            }

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(onClick);
            }
        }
        
        
    }
}