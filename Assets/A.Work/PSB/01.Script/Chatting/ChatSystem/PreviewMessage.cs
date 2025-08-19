using System.Collections;
using DG.Tweening;
using Scripts.Chatting.ChatSO;
using Scripts.Test;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatSystem
{
    public class PreviewMessage : MonoBehaviour
    {
        [SerializeField] private RawImage iconImage;
        [SerializeField] private TextMeshProUGUI numTxt;    
        [SerializeField] private TextMeshProUGUI messageTxt; 

        private MessageSO _messageData;
        private Button _button;
        private ChatWindow _chatWindowPrefab;
        private Transform _chatParent;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null) _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(OnClick);
        }

        public void Initialize(MessageSO data, ChatWindow chatPrefab, Transform chatParent)
        {
            _messageData = data;
            _chatWindowPrefab = chatPrefab;
            _chatParent = chatParent;

            if (iconImage != null) iconImage.texture = data.icon;

            numTxt.text = data.number;

            string preview = (data.messagePreview != null && data.messagePreview.Length > 0)
                ? data.messagePreview[0] : "";
            messageTxt.text = preview;
            
            Transform tr = transform;
            float startY = tr.localPosition.y - 100f;
            tr.localPosition = new Vector3(tr.localPosition.x, startY, tr.localPosition.z);
            tr.DOLocalMoveY(startY + 100f, 0.35f).SetEase(Ease.OutCubic);
        }

        private void OnClick()
        {
            StartCoroutine(ClickCoroutine());
        }

        private IEnumerator ClickCoroutine()
        {
            ChatManager.Instance.OpenChatWindow(_messageData, _chatWindowPrefab, _chatParent);

            yield return null;
            
            transform.DOLocalMoveY(transform.localPosition.y - 75f, 0.25f)
                    .SetEase(Ease.InCubic)
                    .OnComplete(() => Destroy(gameObject));
        }
        
        
    }
}
