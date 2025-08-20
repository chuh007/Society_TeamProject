using System;
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

        [Serializable]
        private class Wrapper
        {
            public List<ConversationLog> conversations = new();

            public Wrapper(Dictionary<string, ConversationLog> dict)
            {
                conversations = new List<ConversationLog>(dict.Values);
            }

            public Dictionary<string, ConversationLog> ToDictionary()
            {
                Dictionary<string, ConversationLog> dict = new Dictionary<string, ConversationLog>();
                
                foreach (ConversationLog conv in conversations)
                {
                    dict[conv.roomId] = conv;
                }
                
                return dict;
            }
        }
        
        
    }
}