using System;
using Scripts.Chatting.ChatSystem;
using UnityEngine;

namespace Scripts.Chatting.ChatSO
{
    [Serializable]
    public class DialogueNode
    {
        [TextArea] public string[] messages;
        public DialogueChoice[] choices;
    }

    [Serializable]
    public class DialogueChoice
    {
        [TextArea] public string answer;
        public int nextNodeIndex;
    }
    
    [CreateAssetMenu(fileName = "MessageText", menuName = "SO/Message", order = 0)]   
    public class MessageSO : ScriptableObject
    {
        [Header("Identification")]
        public string roomId;       // 유니크 ID
        public string roomName; // 방 이름
        
        [Header("Data")]
        public Texture2D icon;
        public string number;
        [TextArea] public string[] messagePreview;
        
        [Header("Text")]
        public DialogueNode[] nodes;
        
        [Header("SpamValue")]
        public SpamJudgeState isSpam;

        [Header("JudgeValue")] 
        public int value;
        public MessageGradeSO gradeSO;
        
        
    }
}