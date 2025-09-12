using System;
using System.Collections.Generic;
using Scripts.Chatting.ChatSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatUI
{
    public class JudgeSliderTest : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public static JudgeSliderTest Instance;
        private readonly List<ChatWindow> _chatWindows = new();

        public event Action<float> OnValueChanged;

        private void Awake()
        {
            Instance = this;
        }

        public static void Register(ChatWindow window)
        {
            if (Instance == null) return;
            if (!Instance._chatWindows.Contains(window))
            {
                Instance._chatWindows.Add(window);
                window.OnSuccess += Instance.HandleSuccess;
                window.OnFail += Instance.HandleFail;
            }
        }

        public static void Unregister(ChatWindow window)
        {
            if (Instance == null) return;
            if (Instance._chatWindows.Contains(window))
            {
                Instance._chatWindows.Remove(window);
                window.OnSuccess -= Instance.HandleSuccess;
                window.OnFail -= Instance.HandleFail;
            }
        }

        private void HandleSuccess(int value, int multiplier)
        {
            slider.value += value * multiplier;
            OnValueChanged?.Invoke(slider.value);
        }

        private void HandleFail(int value, int multiplier)
        {
            slider.value -= value * multiplier;
            OnValueChanged?.Invoke(slider.value);
        }
        
    }
}