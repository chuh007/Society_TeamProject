using System;
using DG.Tweening;
using Scripts.Chatting.ChatCore;
using Scripts.Chatting.ChatSO;
using Scripts.Chatting.ChatUI;
using Scripts.Cores;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatSystem
{
    public class ChatListWindow : MonoBehaviour
    {
        public static ChatListWindow Instance;
        
        [field: SerializeField] public Transform contentParent;
        [SerializeField] private ChatRoomItem roomItemPrefab;
        [SerializeField] private ChatWindow chatWindowPrefab;
        [SerializeField] private Transform chatParent;
        [SerializeField] private MessageDatabaseSO database;
        
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable() => RefreshAll();
        
        public Action<MessageSO> OnRefreshUI => Refresh;
        
        public void Refresh(MessageSO so)
        {
            if (so == null) return;
            AddChatRoomItem(so);
        }
        
        public void RefreshAll()
        {
            if (ChatManager.Instance == null)
            {
                Debug.LogError("ChatManager 인스턴스를 찾을 수 없습니다");
                return;
            }

            foreach (var pair in ChatManager.Instance.Logs)
            {
                string roomId = pair.Key;
                ConversationLog log = pair.Value;

                // 이미 UI 존재 여부 체크
                bool exists = false;
                foreach (Transform child in contentParent)
                {
                    ChatRoomItem existingItem = child.GetComponent<ChatRoomItem>();
                    if (existingItem != null && existingItem.id == roomId)
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists) continue;

                // database에서 MessageSO 찾기
                MessageSO so = FindMessageSO(roomId);

                AddChatRoomItem(so, log);
            }
        }
        
        private void AddChatRoomItem(MessageSO so)
        {
            if (roomItemPrefab == null)
            {
                Debug.LogError("ChatRoomItem 프리팹이 할당되지 않았습니다");
                return;
            }
            
            if (ChatManager.Instance == null)
            {
                Debug.LogError("ChatManager 인스턴스를 찾을 수 없습니다");
                return;
            }
            
            foreach (Transform child in contentParent)
            {
                ChatRoomItem existingItem = child.GetComponent<ChatRoomItem>();
                if (existingItem != null && existingItem.id == so.roomId)
                {
                    Debug.Log($"이미 존재하는 방({so.roomName})은 다시 만들지 않습니다.");
                    return; // 중복 방 생성 방지
                }
            }
        
            ConversationLog log = ChatManager.Instance.GetOrCreateLog(so);

            ChatRoomItem item = Instantiate(roomItemPrefab, contentParent);
            
            item.Setup(so, log, () =>
            {
                ChatManager.Instance.OpenChatWindow(so, chatWindowPrefab, chatParent);
            });
        }
        
        private void AddChatRoomItem(MessageSO so, ConversationLog log)
        {
            if (so == null || log == null) return;
            if (roomItemPrefab == null) return;

            ChatRoomItem item = Instantiate(roomItemPrefab, contentParent);
            item.Setup(so, log, () =>
            {
                ChatManager.Instance.OpenChatWindow(so, chatWindowPrefab, chatParent);
            });
        }

        private MessageSO FindMessageSO(string roomId)
        {
            if (database == null || database.allMessages == null) return null;
            foreach (var so in database.allMessages)
            {
                if (so != null && so.roomId == roomId)
                    return so;
            }
            return null;
        }
        
        public bool HasRoom(string roomId)
        {
            foreach (Transform child in contentParent)
            {
                ChatRoomItem existingItem = child.GetComponent<ChatRoomItem>();
                if (existingItem != null && existingItem.id == roomId)
                    return true;
            }
            return false;
        }
        
        
    }
}