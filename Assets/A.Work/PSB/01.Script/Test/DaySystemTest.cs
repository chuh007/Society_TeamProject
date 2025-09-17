using System;
using Scripts.Chatting.ChatSystem;
using Scripts.Chatting.ChatUI;
using Scripts.Chatting.Enums;
using Scripts.Chatting.System;
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

        [SerializeField] private int maxDay = 5;
        [SerializeField] private int maxMessagesPerDay = 3;
        private int _day = 1;
        private int _successCount;
        private int _failCount;
        private int _processedMessages;

        public static event Action OnNextDay;
        public static event Action<DayResultType> OnDayEnd;
        
        private void Awake()
        {
            SaveDTO.GameProgress progress = SaveSystem.Load<SaveDTO.GameProgress>(SaveDTO.SaveKeys.DayValue);
            _day = progress.day;
            _processedMessages = progress.processedMessages;
            _successCount = progress.successCount;
            _failCount = progress.failCount;
            
            dayText.text = $"Day {_day}";
            resultText.text = "";
        }

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
            JudgeSliderTest.Instance.ResetSliderValue();
            
            SaveSystem.Save(new SaveDTO.GameProgress
            {
                day = _day,
                processedMessages = _processedMessages,
                successCount = _successCount,
                failCount = _failCount,
            }, SaveDTO.SaveKeys.DayValue);
        }

        private DayResultType GetDayResultType()
        {
            if (_processedMessages == 0) return DayResultType.Normal;

            float successRate = (float)_successCount / _processedMessages;

            if (successRate >= 1f) return DayResultType.Perfect;
            if (successRate >= 0.7f) return DayResultType.Success;
            if (successRate >= 0.5f) return DayResultType.Normal;
            if (successRate >= 0.2f) return DayResultType.Fail;
            return DayResultType.Worst;
        }

        #region CheckEnd
        
        private void CheckDayEnd()
        {
            if (_processedMessages >= maxMessagesPerDay)
            {
                CheckDayClear();
                _processedMessages = 0;
                
                _day++;

                if (_day > maxDay)
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
                    "오늘 하루도 무사히 지나갔다.\n"
                    + $"오늘의 메시지 수 : {_processedMessages}\n"
                    + $"성공:{_successCount}, 실패:{_failCount}";
            }
            else
            {
                resultText.text =
                    "오늘은 무사하지 않네...\n"
                    + $"오늘의 메시지 수 : {_processedMessages}\n"
                    + $"성공:{_successCount}, 실패:{_failCount}";
            }

            DayResultType type = GetDayResultType();
            OnDayEnd?.Invoke(type);
        }
        
        private void CheckFinalClear()
        {
            if (slider.value >= 50)
                resultText.text = 
                    "게임 클리어!\n" 
                    + $"전체 처리한 메시지 수 : {maxDay * maxMessagesPerDay}\n" 
                    + $"성공:{_successCount}, 실패:{_failCount}";
            else
                resultText.text = 
                    "인터넷 세상은 멸망했다.\n" 
                    + $"전체 처리한 메시지 수 : {maxDay * maxMessagesPerDay}\n" 
                    + $"성공:{_successCount}, 실패:{_failCount}";
        }

        #endregion
        
        #region Temp

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                ResetAll();
            }
        }

        private void ResetAll()
        {
            // Day 초기화
            _day = 1;
            _processedMessages = 0;
            _successCount = 0;
            _failCount = 0;
            _processedMessages = 0;
            dayText.text = $"Day {_day}";
            resultText.text = "";

            // Slider 초기화
            JudgeSliderTest.Instance?.ResetSliderValue();

            // 저장 초기화
            SaveSystem.Save(new SaveDTO.GameProgress
            {
                day = _day,
                processedMessages = _processedMessages,
                successCount = _successCount,
                failCount = _failCount,
            }, SaveDTO.SaveKeys.DayValue);
    
            SaveSystem.Save(new SaveDTO.SliderProgress
            {
                sliderValue = 0
            }, SaveDTO.SaveKeys.SliderValue);
        }

        #endregion
        
        
    }
}