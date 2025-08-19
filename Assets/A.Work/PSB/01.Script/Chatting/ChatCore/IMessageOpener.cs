using Scripts.Chatting.ChatSO;

namespace Scripts.Chatting.ChatCore
{
    public interface IMessageOpener
    {
        void Open(MessageSO messageData);
    }
}