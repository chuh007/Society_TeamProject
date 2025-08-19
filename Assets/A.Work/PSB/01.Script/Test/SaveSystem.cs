using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Scripts.Test
{
    public static class SaveSystem
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "chatlog.json");

        public static void Save(Dictionary<string, ConversationLog> logs)
        {
            string json = JsonUtility.ToJson(new Wrapper(logs), true);
            File.WriteAllText(SavePath, json);
        }

        public static Dictionary<string, ConversationLog> Load()
        {
            if (!File.Exists(SavePath)) return new();
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<Wrapper>(json).ToDictionary();
        }

        public static void Clear()
        {
            if (File.Exists(SavePath))
                File.Delete(SavePath);
        }

        [System.Serializable]
        private class Wrapper
        {
            // ... (기존 내용은 동일)
            public System.Collections.Generic.List<ConversationLog> conversations = new();

            public Wrapper(Dictionary<string, ConversationLog> dict)
            {
                conversations = new System.Collections.Generic.List<ConversationLog>(dict.Values);
            }

            public Dictionary<string, ConversationLog> ToDictionary()
            {
                var dict = new Dictionary<string, ConversationLog>();
                foreach (var conv in conversations)
                    dict[conv.roomId] = conv;
                return dict;
            }
        }
        
        
    }
}