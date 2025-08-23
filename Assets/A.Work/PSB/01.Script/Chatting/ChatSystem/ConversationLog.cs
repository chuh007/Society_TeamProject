using System;
using System.Collections.Generic;

namespace Scripts.Chatting.ChatSystem
{
    public enum SpamJudgeState
    {
        UnKnown = 0,
        NotSpam = 1,
        Spam = 2
    }
    
    [Serializable]
    public class MessageData
    {
        public string text;
        public bool isMine; // true면 내 메시지, false면 상대 메시지
    }
    
    [Serializable]
    public class ConversationLog
    {
        public string roomId;
        public string roomName;
        
        public List<MessageData> messages = new();
        
        public int currentNodeIndex = 0;
        public SpamJudgeState judgeState = SpamJudgeState.UnKnown;
    }
}