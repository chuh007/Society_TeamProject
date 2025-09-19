using TMPro;
using UnityEngine;

namespace Scripts.Chatting.ChatUI
{
    public class MessageBubble : MonoBehaviour
    {
        [SerializeField] private RectTransform bubbleRect;
        [SerializeField] private TextMeshProUGUI messageText;
        
        [SerializeField] private float paddingTopBottom = 10f;
        [SerializeField] private float minHeight = 40f;

        public void SetText(string text)
        {
            messageText.text = text;
            
            float preferredHeight = messageText.preferredHeight + paddingTopBottom * 2;
            
            float finalHeight = Mathf.Max(preferredHeight, minHeight);

            Vector2 size = bubbleRect.sizeDelta;
            size.y = finalHeight;
            bubbleRect.sizeDelta = size;
        }
        
        
    }
}