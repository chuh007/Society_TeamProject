using System;
using System.Collections.Generic;
using A.Work.CHUH._01.Script.UI.Fraud;
using Scripts.Chatting.ChatSystem;
using Scripts.Chatting.System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatUI
{
    public class JudgeSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public static JudgeSlider Instance;
        private readonly List<IDivisible> _chatWindows = new();

        public event Action<float> OnValueChanged;

        private void Awake()
        {
            Instance = this;
            
            SaveDTO.SliderProgress progress = SaveSystem.Load<SaveDTO.SliderProgress>(SaveDTO.SaveKeys.SliderValue);
            slider.value = progress.sliderValue;
        }

        public static void Register(IDivisible window)
        {
            if (Instance == null) return;
            if (!Instance._chatWindows.Contains(window))
            {
                Instance._chatWindows.Add(window);
                window.OnSuccess += Instance.HandleSuccess;
                window.OnFail += Instance.HandleFail;
            }
        }

        public static void Unregister(IDivisible window)
        {
            if (Instance == null) return;
            if (Instance._chatWindows.Contains(window))
            {
                Instance._chatWindows.Remove(window);
                window.OnSuccess -= Instance.HandleSuccess;
                window.OnFail -= Instance.HandleFail;
            }
        }

        private void OnDestroy()
        {
            SaveCurrentValue();
        }
        
        private void SaveCurrentValue()
        {
            SaveSystem.Save(new SaveDTO.SliderProgress { sliderValue = slider.value }, SaveDTO.SaveKeys.SliderValue);
        }

        private void HandleSuccess(int value, int multiplier)
        {
            slider.value += value * multiplier;
            OnValueChanged?.Invoke(slider.value);
            SaveCurrentValue();
        }

        private void HandleFail(int value, int multiplier)
        {
            slider.value -= value * multiplier;
            OnValueChanged?.Invoke(slider.value);
            SaveCurrentValue();
        }

        public void ResetSliderValue()
        {
            slider.value = 0;
            SaveCurrentValue();
        }
        
    }
}