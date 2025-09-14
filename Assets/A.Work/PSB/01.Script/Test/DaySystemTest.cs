using System;
using Scripts.Chatting.ChatSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Test
{
    public class DaySystemTest : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI resultText;

        [SerializeField] private int maxMessagesPerDay = 3;
        private int _day = 1;
        private int _successCount;
        private int _failCount;
        private int _processedMessages;

        public static event Action OnNextDay; 

        private void OnEnable()
        {
            ChatWindow.OnGlobalSuccess += HandleSuccess;
            ChatWindow.OnGlobalFail += HandleFail;
            OnNextDay += HandleNextDay;
        }

        private void OnDisable()
        {
            ChatWindow.OnGlobalSuccess -= HandleSuccess;
            ChatWindow.OnGlobalFail -= HandleFail;
            OnNextDay -= HandleNextDay;
        }

        private void Awake()
        {
            dayText.text = $"Day {_day}";
            resultText.text = "";
        }

        private void HandleSuccess()
        {
            _successCount++;
            _processedMessages++;
            CheckDayEnd();
        }

        private void HandleFail()
        {
            _failCount++;
            _processedMessages++;
            CheckDayEnd();
        }

        private void HandleNextDay()
        {
            _successCount = 0;
            _failCount = 0;
            slider.value = 0;
        }

        private void CheckDayEnd()
        {
            if (_processedMessages >= maxMessagesPerDay)
            {
                _processedMessages = 0;
                
                CheckDayClear();

                _day++;

                if (_day > 10)
                {
                    CheckFinalClear();
                    return;
                }

                dayText.text = $"Day {_day}";
                OnNextDay?.Invoke();
            }
        }

        private void CheckDayClear()
        {
            if (slider.value >= 50)
            {
                resultText.text =
                    "오늘 하루도 무사히 지나갔다."
                    + $"오늘의 메시지 수 : {_processedMessages} "
                    + $"성공:{_successCount}, 실패:{_failCount}";
            }
            else
            {
                resultText.text =
                    "우주가 잠시 꺼졌다... "
                    + $"오늘의 메시지 수 : {_processedMessages}"
                    + $"성공:{_successCount}, 실패:{_failCount}";
            }
        }
        
        private void CheckFinalClear()
        {
            if (slider.value >= 50)
                resultText.text = $"게임 클리어!\n성공:{_successCount}, 실패:{_failCount}";
            else
                resultText.text = $"게임 실패...\n성공:{_successCount}, 실패:{_failCount}";
        }
        
    }
}