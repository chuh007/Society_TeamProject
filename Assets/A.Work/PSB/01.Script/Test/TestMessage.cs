using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Test
{
    public class TestMessage : MonoBehaviour
    {
        [SerializeField] private RawImage iconImage;
        [SerializeField] private TextMeshProUGUI numTxt;
        [SerializeField] private TextMeshProUGUI messageTxt;

        private MessageSO _messageData;
        private Button _button;
        private TestChatWindow _chatWindowPrefab;
        private Transform _chatParent;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        public void Initialize(MessageSO data, TestChatWindow chatPrefab, Transform chatParent)
        {
            _messageData = data;
            _chatWindowPrefab = chatPrefab;
            _chatParent = chatParent;

            iconImage.texture = data.icon;
            numTxt.text = data.number;
            messageTxt.text = data.messagePreview.Length > 0 ? data.messagePreview[0] : "";
        }

        private void OnClick()
        {
            var chatInstance = Instantiate(_chatWindowPrefab, _chatParent);
            chatInstance.Open(_messageData);
        }
        
    }
    
}