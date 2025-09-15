using System;
using System.Collections.Generic;
using System.IO;
using Scripts.Chatting.ChatSystem;
using UnityEngine;

namespace Scripts.Chatting.System
{
    public static class SaveSystem
    {
        private static string GetPath(string fileName)
            => Path.Combine(Application.persistentDataPath, fileName + ".json");

        public static void Save<T>(T data, string fileName)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetPath(fileName), json);
        }

        public static T Load<T>(string fileName) where T : new()
        {
            string path = GetPath(fileName);
            if (!File.Exists(path))
                return new T(); // 파일 없으면 기본값 반환
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        public static void Clear(string fileName)
        {
            string path = GetPath(fileName);
            if (File.Exists(path)) File.Delete(path);
        }
        
        
    }
}