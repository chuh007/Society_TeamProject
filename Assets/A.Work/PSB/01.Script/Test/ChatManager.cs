using System;
using System.Collections.Generic;
using Scripts.Chatting.ChatSO;
using Scripts.Chatting.ChatSystem;
using UnityEngine;

namespace Scripts.Test
{
    [DefaultExecutionOrder(-10)]
    public class ChatManager : MonoBehaviour
    {
        public static ChatManager Instance;

        // 모든 방의 저장 로그
        private Dictionary<string, ConversationLog> _logs = new();
        public IReadOnlyDictionary<string, ConversationLog> Logs => _logs;

        // 열려 있는 창(방당 1개만)
        private readonly Dictionary<string, ChatWindow> _openWindows = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _logs = SaveSystem.Load();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                ClearAllData();
            }
        }

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
                    judgedSpam = null
                };
                _logs[so.roomId] = log;
                SaveSystem.Save(_logs);
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
                existing.ReOpen(so, log, () => SaveSystem.Save(_logs)); // <-- ReOpen 호출
                return;
            }

            ChatWindow inst = Instantiate(windowPrefab, parent);
            inst.OpenRoom(so, log, () => SaveSystem.Save(_logs));

            _openWindows[so.roomId] = inst;
        }
        
        public void ClearAllData()
        {
            _logs.Clear(); // 메모리에 있는 로그 데이터 삭제
            SaveSystem.Clear(); // 저장된 JSON 파일 삭제
            
            // 열려 있는 모든 채팅 창 파괴
            foreach (var window in _openWindows.Values)
            {
                if (window != null)
                {
                    Destroy(window.gameObject);
                }
            }
            _openWindows.Clear();
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

        public void SaveAll() => SaveSystem.Save(_logs);
        
        public void ClearAll()
        {
            _logs.Clear();
            SaveSystem.Clear();
        }
    }
}