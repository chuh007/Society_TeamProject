using System.Collections.Generic;
using Scripts.Chatting.ChatSO;
using Scripts.Chatting.ChatSystem;
using Scripts.Chatting.System;
using UnityEngine;

namespace Scripts.Chatting.ChatCore
{
    [DefaultExecutionOrder(-10)]
    public class ChatManager : MonoBehaviour
    {
        public static ChatManager Instance;
        
        private Dictionary<string, ConversationLog> _logs = new();
        public IReadOnlyDictionary<string, ConversationLog> Logs => _logs;
        
        private readonly Dictionary<string, ChatWindow> _openWindows = new();
        private HashSet<MessageSO> _appearedMessages = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadMessages();
        }

        #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                ClearAllData();
            }
        }
        #endif

        public ConversationLog GetOrCreateLog(MessageSO so)
        {
            if (so == null || string.IsNullOrEmpty(so.roomId))
            {
                Debug.LogError("[ChatManager] MessageSO 혹은 roomId가 비었습니다.");
                return null;
            }
            if (!_logs.TryGetValue(so.roomId, out ConversationLog log))
            {
                log = new ConversationLog
                {
                    roomId = so.roomId,
                    roomName = so.roomName,
                    currentNodeIndex = 0,
                    judgeState = SpamJudgeState.UnKnown
                };
                _logs[so.roomId] = log;
                SaveAll();
            }
            else
            {
                // 이름이 바뀐 경우 최신화
                if (log.roomName != so.roomName)
                    log.roomName = so.roomName;
            }
            
            return log;
        }

        public void OpenChatWindow(MessageSO so, ChatWindow windowPrefab, Transform parent)
        {
            if (so == null || windowPrefab == null || parent == null)
            {
                Debug.LogError("[ChatManager] OpenChatWindow 인자 누락");
                return;
            }

            ConversationLog log = GetOrCreateLog(so);
            if (log == null) return;

            if (_openWindows.TryGetValue(so.roomId, out ChatWindow existing) && existing != null)
            {
                existing.gameObject.SetActive(true);
                existing.BringToFront();
                existing.ReOpen(so, log, SaveAll);
                return;
            }

            ChatWindow inst = Instantiate(windowPrefab, parent);
            inst.OpenRoom(so, log, SaveAll);
            
            _openWindows[so.roomId] = inst;
        }
        
        public void ClearAllData()
        {
            _logs.Clear();
            _appearedMessages.Clear();
            SaveSystem.Clear(SaveDTO.SaveKeys.ChatLogs); 
            
            foreach (ChatWindow window in _openWindows.Values)
            {
                if (window != null)
                {
                    Destroy(window.gameObject);
                }
            }
            _openWindows.Clear();
            
            if (ChatListWindow.Instance != null)
            {
                foreach (Transform child in ChatListWindow.Instance.contentParent)
                {
                    Destroy(child.gameObject);
                }
            }
            
            SaveAll();
        }

        public void CloseChatWindow(string roomId)
        {
            if (string.IsNullOrEmpty(roomId)) return;
            if (_openWindows.TryGetValue(roomId, out ChatWindow w) && w != null)
                w.gameObject.SetActive(false);
        }

        public void DestroyChatWindow(string roomId)
        {
            if (string.IsNullOrEmpty(roomId)) return;
            if (_openWindows.TryGetValue(roomId, out ChatWindow w) && w != null)
            {
                Destroy(w.gameObject);
            }
            _openWindows.Remove(roomId);
        }

        public void LoadMessages()
        {
            var collection = SaveSystem.Load<SaveDTO.ConversationCollection>(SaveDTO.SaveKeys.ChatLogs);
            _logs = collection?.ToDictionary() ?? new Dictionary<string, ConversationLog>();
        }

        public void SaveAll()
        {
            var collection = new SaveDTO.ConversationCollection();
            collection.FromDictionary(_logs);
            SaveSystem.Save(collection, SaveDTO.SaveKeys.ChatLogs);
        }
        
        public bool HasAppeared(MessageSO message) => _appearedMessages.Contains(message);

        public void RegisterMessage(MessageSO message)
        {
            _appearedMessages.Add(message);
        }
        
        
    }
}