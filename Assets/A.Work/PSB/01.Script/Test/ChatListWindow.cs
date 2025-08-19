using Scripts.Chatting.ChatSO;
using Scripts.Chatting.ChatSystem;
using UnityEngine;

namespace Scripts.Test
{
    public class ChatListWindow : MonoBehaviour
    {
        [SerializeField] private Transform contentParent;
        [SerializeField] private ChatRoomItem roomItemPrefab;
        [SerializeField] private ChatWindow chatWindowPrefab;
        [SerializeField] private Transform chatParent;
        [SerializeField] private MessageDatabaseSO database;

        private void OnEnable() => Refresh();

        public void Refresh()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            if (database == null || database.allMessages == null) return;
    
            // roomItemPrefab이 Null인지 확인하는 방어 코드 추가
            if (roomItemPrefab == null)
            {
                Debug.LogError("ChatRoomItem 프리팹이 할당되지 않았습니다!");
                return;
            }

            foreach (MessageSO so in database.allMessages)
            {
                if (so == null) continue;
        
                // ChatManager가 Null인지 확인하는 코드
                if (ChatManager.Instance == null)
                {
                    Debug.LogError("ChatManager 인스턴스를 찾을 수 없습니다!");
                    return;
                }
        
                ConversationLog log = ChatManager.Instance.GetOrCreateLog(so);

                ChatRoomItem item = Instantiate(roomItemPrefab, contentParent);
                item.Setup(so, log, () =>
                {
                    ChatManager.Instance.OpenChatWindow(so, chatWindowPrefab, chatParent);
                });
            }
        }
        
        
    }
}