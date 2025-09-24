using System;
using A.Work.CHUH._01.Script.UI.Fraud;
using A.Work.CHUH._01.Script.UI.PopUp;
using Scripts.Chatting.ChatUI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class VoicePhishing : MonoBehaviour, IPopupable, IDivisible
    {
        [SerializeField] private WaitListen waitListen;
        [SerializeField] private Wiretapping wiretapping;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private AudioSource soundSource;
        [SerializeField] private AudioClip soundClip;
        
        public event Action<int, int> OnSuccess;
        public event Action<int, int> OnFail;
        
        private VoicePhishingSO _phishingData;
        private float _timer = 0f;
        private float _voiceTime;
        
        private void Update()
        {
            _timer += Time.deltaTime;
            timeText.text = _timer.ToString("0.00");
            if (_timer >= _voiceTime)
                Hide();
        }

        public void SetData(VoicePhishingSO phishingData)
        {
            _phishingData = phishingData;
            _voiceTime = _phishingData.voiceClip.length;
        }
        
        public void PopUp()
        {
            JudgeSlider.Register(this);
            waitListen.gameObject.SetActive(true);
            waitListen.Setup(_phishingData.sender, _phishingData.recipient, soundSource, soundClip);
            waitListen.PlaySound();
            wiretapping.gameObject.SetActive(false);
        }


        public void Hide()
        {
            JudgeSlider.Unregister(this);
            waitListen.StopSound();
            wiretapping.StopSound();
            Destroy(gameObject);
        }

        public void Wiretapping()
        {
            waitListen.StopSound();
            waitListen.gameObject.SetActive(false);
            wiretapping.Setup(_phishingData.sender, _phishingData.recipient, soundSource, _phishingData.voiceClip);
            wiretapping.gameObject.SetActive(true);
            wiretapping.Play(_timer);
        }

        public void ExplanationPhishing()
        {
            if (_phishingData.isFraud)
            {
                OnSuccess?.Invoke(_phishingData.value, _phishingData.gradeSO.successMultiplier);
            }
            else
            {
                OnFail?.Invoke(_phishingData.value, _phishingData.gradeSO.failMultiplier);
            }
            Hide();
        }
    }
}