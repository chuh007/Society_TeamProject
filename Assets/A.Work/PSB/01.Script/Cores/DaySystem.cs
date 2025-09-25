using System;
using Scripts.Chatting.ChatSystem;
using Scripts.Chatting.ChatUI;
using Scripts.Chatting.Enums;
using Scripts.Chatting.System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scripts.Cores
{
    public class DaySystem : MonoBehaviour
    {
        public Action<bool> OnSpamClear;
        
        [Header("Day")]
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private int maxDay = 5;
        [SerializeField] private int maxMessagesPerDay = 3;
        
        [Header("Result")]
        [SerializeField] private RectTransform resultPanel;
        [SerializeField] private TextMeshProUGUI resultText;
        
        private int _day = 1;
        private int _successCount;
        private int _failCount;
        private int _processedMessages;

        private Button _closeResultPanelBtn;
        [SerializeField] private Button endingBtn;

        public static event Action OnNextDay;
        public static event Action<DayResultType> OnResultClose;
        
        private void Awake()
        {
            OnSpamClear += HandleSpamClear;
            
            SaveDTO.GameProgress progress = SaveSystem.Load<SaveDTO.GameProgress>(SaveDTO.SaveKeys.DayValue);
            _day = progress.day;
            _processedMessages = progress.processedMessages;
            _successCount = progress.successCount;
            _failCount = progress.failCount;
            _closeResultPanelBtn = resultPanel.GetComponentInChildren<Button>();
            
            dayText.text = $"Day {_day}";
            resultText.text = "";
            resultPanel.gameObject.SetActive(false);
            endingBtn.gameObject.SetActive(false);
        }

        private void HandleSpamClear(bool value)
        {
            if (value) _successCount++;
            else _failCount++;
            _processedMessages++;
            CheckDayEnd();
        }

        private void Start()
        {
            GameTypeSingleton.Instance.OnGameTypeChanged += HandleGameTypeChanged;

        }

        private void OnEnable()
        {
            ChatWindow.OnGlobalSuccess += HandleSuccess;
            ChatWindow.OnGlobalFail += HandleFail;
            OnNextDay += HandleNextDay;
            _closeResultPanelBtn.onClick.AddListener(CloseResultPanel);
        }

        private void OnDestroy()
        {
            GameTypeSingleton.Instance.OnGameTypeChanged -= HandleGameTypeChanged;
            ChatWindow.OnGlobalSuccess -= HandleSuccess;
            ChatWindow.OnGlobalFail -= HandleFail;
            OnNextDay -= HandleNextDay;
            _closeResultPanelBtn.onClick.RemoveListener(CloseResultPanel);
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
            JudgeSlider.Instance.ResetSliderValue();
            dayText.text = $"Day {_day}";
            
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
            float gauge = slider.value;

            if (gauge >= 100f) return DayResultType.Perfect;
            else if (gauge >= 70f) return DayResultType.Success;
            else if (gauge >= 50f) return DayResultType.Normal;
            else if (gauge >= 20f) return DayResultType.Fail;
            else return DayResultType.Worst;
        }

        private void CloseResultPanel()
        {
            resultPanel.gameObject.SetActive(false);
            GameTypeSingleton.Instance.GameType = GameType.Story;
            
            DayResultType type = GetDayResultType();
            OnResultClose?.Invoke(type);
            OnNextDay?.Invoke();
        }
        
        private void HandleGameTypeChanged(GameType type)
        {
            resultPanel.gameObject.SetActive(type == GameType.Result || type == GameType.Clear || type == GameType.Fail);

            bool isFinal = type == GameType.Clear || type == GameType.Fail;
            _closeResultPanelBtn.gameObject.SetActive(!isFinal);
            endingBtn.gameObject.SetActive(isFinal);
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
            }
        }

        private void CheckDayClear()
        {
            GameTypeSingleton.Instance.GameType = GameType.Result;
            resultPanel.SetAsLastSibling();
            
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
            
        }

        private void CheckFinalClear()
        {
            int totalMessages = maxDay * maxMessagesPerDay;
            
            if (_successCount >= _failCount)
            {
                resultText.text =
                    "게임 클리어!\n"
                    + $"전체 처리한 메시지 수 : {totalMessages}\n"
                    + $"성공:{_successCount}, 실패:{_failCount}";
                GameTypeSingleton.Instance.GameType = GameType.Clear;
            }
            else
            {
                resultText.text =
                    "인터넷 세상은 멸망했다.\n"
                    + $"전체 처리한 메시지 수 : {totalMessages}\n"
                    + $"성공:{_successCount}, 실패:{_failCount}";
                GameTypeSingleton.Instance.GameType = GameType.Fail;
            }
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
            JudgeSlider.Instance?.ResetSliderValue();

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