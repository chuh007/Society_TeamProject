using UnityEngine;

namespace Scripts.Chatting.ChatSO
{
    [CreateAssetMenu(fileName = "MessageBase", menuName = "SO/Base", order = 0)]
    public class MessageDatabaseSO : ScriptableObject
    {
        public MessageSO[] allMessages;
    }
}