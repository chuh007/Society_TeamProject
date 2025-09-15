using System;
using System.Collections.Generic;
using Scripts.Chatting.ChatSystem;
using UnityEngine;

namespace Scripts.Chatting.System
{
    public class SaveDTO : MonoBehaviour
    {
        public static class SaveKeys
        {
            public const string ChatLogs = "chatlogs";
            public const string DayValue = "days";
            public const string SliderValue = "sliders";
        }
        
        [Serializable]
        public class ConversationCollection
        {
            public List<ConversationLog> conversations = new();

            public Dictionary<string, ConversationLog> ToDictionary()
            {
                Dictionary<string, ConversationLog> dict = new();
                foreach (var conv in conversations)
                    dict[conv.roomId] = conv;
                return dict;
            }

            public void FromDictionary(Dictionary<string, ConversationLog> dict)
            {
                conversations = new List<ConversationLog>(dict.Values);
            }
        }
        
        [Serializable]
        public class GameProgress
        {
            public int day = 1;
            public int processedMessages = 0;
            public int successCount;
            public int failCount;
        }

        [Serializable]
        public class SliderProgress
        {
            public float sliderValue;
        }
        
        
    }
}