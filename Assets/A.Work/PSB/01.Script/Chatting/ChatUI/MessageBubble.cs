using TMPro;
using UnityEngine;

namespace Scripts.Chatting.ChatUI
{
    public class MessageBubble : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;

        public void SetText(string text)
        {
            messageText.text = text;
        }
        
    }
}