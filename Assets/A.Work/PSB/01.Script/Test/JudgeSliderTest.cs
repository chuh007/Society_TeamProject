using System.Collections.Generic;
using Scripts.Chatting.ChatSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Test
{
    public class JudgeSliderTest : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        private static JudgeSliderTest _instance;
        private readonly List<ChatWindow> _chatWindows = new();

        private void Awake()
        {
            _instance = this;
        }

        public static void Register(ChatWindow window)
        {
            if (_instance == null) return;
            if (!_instance._chatWindows.Contains(window))
            {
                _instance._chatWindows.Add(window);
                window.OnSuccess += _instance.HandleSuccess;
                window.OnFail += _instance.HandleFail;
            }
        }

        public static void Unregister(ChatWindow window)
        {
            if (_instance == null) return;
            if (_instance._chatWindows.Contains(window))
            {
                _instance._chatWindows.Remove(window);
                window.OnSuccess -= _instance.HandleSuccess;
                window.OnFail -= _instance.HandleFail;
            }
        }

        private void HandleSuccess(float value)
        {
            slider.value += value;
        }

        private void HandleFail(float value)
        {
            slider.value -= value;
        }
        
    }
}