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

            // 가용 너비를 넣어서 줄바꿈 포함 높이 계산
            float availableWidth = bubbleRect.rect.width;
            Vector2 preferredSize = messageText.GetPreferredValues(text, availableWidth, 0);

            float finalHeight = Mathf.Max(preferredSize.y + paddingTopBottom * 2, minHeight);

            Vector2 size = bubbleRect.sizeDelta;
            size.y = finalHeight;
            bubbleRect.sizeDelta = size;
        }
        
        
    }
}