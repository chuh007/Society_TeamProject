using System;
using DG.Tweening;
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
        private int _totalSuccess;     // 전체 성공
        private int _totalFail;        // 전체 실패
        private int _processedMessages;

        private Button _closeResultPanelBtn;
        [SerializeField] private Button endingBtn;

        public static event Action OnNextDay;
        public static event Action<DayResultType> OnResultClose;
        public static event Action<bool> OnFinalResult;
        public static bool? LastFinalResult = null;
        
        private void Awake()
        {
            OnSpamClear += HandleSpamClear;
            
            SaveDTO.GameProgress progress = SaveSystem.Load<SaveDTO.GameProgress>(SaveDTO.SaveKeys.DayValue);
            _day = progress.day;
            _processedMessages = progress.processedMessages;
            _successCount = progress.successCount;
            _failCount = progress.failCount;
            _totalSuccess = progress.successCount;
            _totalFail = progress.failCount;
            _closeResultPanelBtn = resultPanel.GetComponentInChildren<Button>();
            
            dayText.text = $"Day {_day}";
            resultText.text = "";
            resultPanel.gameObject.SetActive(false);
            endingBtn.gameObject.SetActive(false);
        }

        private void HandleSpamClear(bool value)
        {
            if (value)
            {
                _successCount++;
                _totalSuccess++;
            }
            else
            {
                _failCount++;
                _totalFail++;
            }

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
            _totalSuccess++;
            _processedMessages++;
            CheckDayEnd();
        }

        private void HandleFail()
        {
            _failCount++;
            _totalFail++;
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
                totalSuccessCount = _successCount,
                totalFailCount = _failCount,
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
            resultPanel.transform.DOScale(Vector3.zero, 0.5f)
                .OnComplete(() =>
                {
                    resultPanel.gameObject.SetActive(false);
                });
            GameTypeSingleton.Instance.GameType = GameType.Story;
            
            DayResultType type = GetDayResultType();
            OnResultClose?.Invoke(type);
            OnNextDay?.Invoke();
        }
        
        private void HandleGameTypeChanged(GameType type)
        {
            resultPanel.transform.localScale = Vector3.one;
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
                DayResultType type = GetDayResultType();

                if (type == DayResultType.Worst)
                {
                    _day = maxDay + 1;
                    ForceFail();
                    return;
                }
                
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
                    "오늘 하루는 무사하지 않았다.\n"
                    + $"오늘의 메시지 수 : {_processedMessages}\n"
                    + $"성공:{_successCount}, 실패:{_failCount}";
            }
            
        }

        private void CheckFinalClear()
        {
            int totalMessages = maxDay * maxMessagesPerDay;
            bool isClear = _totalSuccess >= _totalFail;

            LastFinalResult = isClear;

            if (isClear)
            {
                resultText.text =
                    "평판이 높아 승진하였습니다!\n"
                    + "부서에 새로운 신입이 들어와 일이 줄었습니다 \n"
                    + $"전체 처리한 메시지 수 : {totalMessages}\n"
                    + $"성공:{_totalSuccess}, 실패:{_totalFail}";
                GameTypeSingleton.Instance.GameType = GameType.Clear;
            }
            else
            {
                resultText.text =
                    "평판이 낮아 해고되었습니다!\n"
                    + $"전체 처리한 메시지 수 : {totalMessages}\n"
                    + $"성공:{_totalSuccess}, 실패:{_totalFail}";
                GameTypeSingleton.Instance.GameType = GameType.Fail;
            }

            OnFinalResult?.Invoke(isClear);
        }
        
        private void ForceFail()
        {
            LastFinalResult = false;

            resultText.text =
                "평판이 낮아 해고되었습니다!\n";

            GameTypeSingleton.Instance.GameType = GameType.Fail;
            OnFinalResult?.Invoke(false);
        }

        #endregion
        
        #region Temp

        #if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                ResetAll();
            }
        }
        #endif

        private void ResetAll()
        {
            // Day 초기화
            _day = 1;
            _processedMessages = 0;
            _successCount = 0;
            _failCount = 0;
            _totalSuccess = 0;
            _totalFail = 0;
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
                totalSuccessCount = _successCount,
                totalFailCount = _failCount,
            }, SaveDTO.SaveKeys.DayValue);
    
            SaveSystem.Save(new SaveDTO.SliderProgress
            {
                sliderValue = 0
            }, SaveDTO.SaveKeys.SliderValue);
        }

        #endregion
        
        
    }
}