using Scripts.Chatting.ChatSO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Test
{
    public class ChatRoomItem : MonoBehaviour
    {
        [SerializeField] private RawImage icon;
        [SerializeField] private TextMeshProUGUI nameTxt;
        [SerializeField] private TextMeshProUGUI lastMsgTxt;
        [SerializeField] private Button openButton;

        private MessageSO _so;
        private ConversationLog _log;
        public string id;
        
        public void Setup(MessageSO so, ConversationLog log, System.Action onOpen)
        {
            _so = so;
            _log = log;
            id = so.roomId;
            
            if (icon != null) icon.texture = so.icon;
            if (nameTxt != null) nameTxt.text = string.IsNullOrEmpty(so.roomName) ? so.number : so.roomName;
            
            string last = (_log.messages.Count > 0) ? _log.messages[^1].text : 
                (so.messagePreview != null && so.messagePreview.Length > 0 ? so.messagePreview[0] : "대화 없음");
            
            if (lastMsgTxt != null) lastMsgTxt.text = last;

            if (openButton != null)
            {
                openButton.onClick.RemoveAllListeners();
                openButton.onClick.AddListener(() => onOpen?.Invoke());
            }
        }
        
    }
}