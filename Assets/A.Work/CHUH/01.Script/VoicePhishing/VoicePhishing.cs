using System;
using A.Work.CHUH._01.Script.UI.Fraud;
using A.Work.CHUH._01.Script.UI.PopUp;
using Ami.BroAudio;
using Scripts.Chatting.ChatUI;
using Scripts.Cores;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class VoicePhishing : MonoBehaviour, IPopupable, IDivisible
    {
        public Action EndEvent;
        
        [SerializeField] private WaitListen waitListen;
        [SerializeField] private Wiretapping wiretapping;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private SoundID soundId;
        
        public event Action<int, int> OnSuccess;
        public event Action<int, int> OnFail;
        
        private DaySystem _daySystem;
        
        private VoicePhishingSO _phishingData;
        private float _timer = 0f;
        private float _voiceTime;
        
        private void Update()
        {
            _timer += Time.deltaTime;
            timeText.text = _timer.ToString("0.00");
            if (_timer >= _voiceTime)
            {
                if (GameTypeSingleton.Instance.GameType != GameType.Game)
                {
                    Hide();
                }
            }
            
        }

        public void SetData(VoicePhishingSO phishingData, DaySystem daySystem)
        {
            _phishingData = phishingData;
            _daySystem = daySystem;
        }

        public void PopUp()
        {
            JudgeSlider.Register(this);
            waitListen.gameObject.SetActive(true);
            waitListen.Setup(_phishingData.sender, _phishingData.recipient, soundId);
            waitListen.PlaySound();
            wiretapping.gameObject.SetActive(false);
        }


        public void Hide()
        {
            EndEvent?.Invoke();
            JudgeSlider.Unregister(this);
            waitListen.StopSound();
            wiretapping.StopSound();
            Destroy(gameObject);
        }

        public void Wiretapping()
        {
            waitListen.StopSound();
            waitListen.gameObject.SetActive(false);
            wiretapping.Setup(_phishingData.sender, _phishingData.recipient, _phishingData.voiceId);
            wiretapping.gameObject.SetActive(true);
            wiretapping.Play(_timer);
        }

        public void ExplanationPhishing()
        {
            if (_phishingData.isFraud)
            {
                _daySystem.OnSpamClear?.Invoke(true);
                OnSuccess?.Invoke(_phishingData.value, _phishingData.gradeSO.successMultiplier);
            }
            else
            {
                _daySystem.OnSpamClear?.Invoke(false);
                OnFail?.Invoke(_phishingData.value, _phishingData.gradeSO.failMultiplier);
            }
            Hide();
        }

        public void Exit()
        {
            if (!_phishingData.isFraud)
            {
                _daySystem.OnSpamClear?.Invoke(true);
                OnSuccess?.Invoke(_phishingData.value, _phishingData.gradeSO.successMultiplier);
            }
            else
            {
                _daySystem.OnSpamClear?.Invoke(false);
                OnFail?.Invoke(_phishingData.value, _phishingData.gradeSO.failMultiplier);
            }
            Hide();
        }
    }
}