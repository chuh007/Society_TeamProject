using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatSystem
{
    public class OpenChatList : MonoBehaviour
    {
        [SerializeField] private GameObject chatPrefab;
        [SerializeField] private RectTransform chatParent;
        
        private Button _button;
        private Button _closeBtn;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _closeBtn = chatPrefab.GetComponentInChildren<Button>();
            
            _button.onClick.AddListener(OpenList);
            _closeBtn.onClick.AddListener(CloseList);
            
            chatPrefab.transform.position = transform.position;
        }

        private void Start()
        {
            chatPrefab.SetActive(false);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OpenList);
            _closeBtn.onClick.RemoveListener(CloseList);
        }

        private void OpenList()
        {
            chatPrefab.transform.localScale = Vector3.zero;
            chatPrefab.gameObject.SetActive(true);
            
            chatPrefab.transform.DOMove(chatParent.position, 0.5f);
            chatPrefab.transform.DOScale(Vector3.one, 0.5f);
        }

        private void CloseList()
        {
            chatPrefab.transform.DOMove(transform.position, 0.5f);
            chatPrefab.transform.DOScale(Vector3.zero, 0.5f)
                .OnComplete(() =>
                {
                    chatPrefab.gameObject.SetActive(false);
                });
        }
        
    }
}