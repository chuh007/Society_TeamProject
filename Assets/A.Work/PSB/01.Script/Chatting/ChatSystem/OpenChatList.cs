using DG.Tweening;
using Scripts.Cores;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatSystem
{
    public class OpenChatList : MonoBehaviour
    {
        [SerializeField] private GameObject chatPrefab;
        [SerializeField] private RectTransform chatParent;
        
        private Button _button;
        [SerializeField] private Button closeBtn;

        private bool _isOpen = false;

        private void Awake()
        {
            _button = GetComponent<Button>();
            
            _button.onClick.AddListener(OpenList);
            closeBtn.onClick.AddListener(CloseList);
            
            chatPrefab.transform.position = transform.position;
        }

        private void Start()
        {
            chatPrefab.SetActive(false);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OpenList);
            closeBtn.onClick.RemoveListener(CloseList);
        }
        
        private void FixedUpdate()
        {
            if (GameTypeSingleton.Instance.GameType != GameType.Game)
            {
                CloseList();
            }
        }

        private void OpenList()
        {
            if (_isOpen) return;
            _isOpen = true;
            
            chatPrefab.transform.localScale = Vector3.zero;
            chatPrefab.gameObject.SetActive(true);
            
            chatPrefab.transform.DOMove(chatParent.position, 0.5f);
            chatPrefab.transform.DOScale(Vector3.one, 0.5f);
        }

        private void CloseList()
        {
            if (!_isOpen) return;
            _isOpen = false;

            chatPrefab.transform.DOMove(transform.position, 0.5f);
            chatPrefab.transform.DOScale(Vector3.zero, 0.5f)
                .OnComplete(() =>
                {
                    chatPrefab.gameObject.SetActive(false);
                });
        }
        
    }
}
