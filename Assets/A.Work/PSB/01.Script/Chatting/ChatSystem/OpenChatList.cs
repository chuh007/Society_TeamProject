using System;
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
            chatPrefab.gameObject.SetActive(true);
        }

        private void CloseList()
        {
            chatPrefab.gameObject.SetActive(false);
        }
        
    }
}