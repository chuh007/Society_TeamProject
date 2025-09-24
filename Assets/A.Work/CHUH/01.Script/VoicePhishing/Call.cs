using A.Work.CHUH._01.Script.Call;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class Call : MonoBehaviour
    {
        [Header("sender")]
        [SerializeField] protected TextMeshProUGUI senderPhoneText;
        [SerializeField] protected Image senderIcon;
        [Header("recipient")]
        [SerializeField] protected TextMeshProUGUI recipientPhoneText;
        [SerializeField] protected Image recipientIcon;
        [Header("Buttons")]
        [SerializeField] protected Button listenButton;
        [SerializeField] protected Button exitButton;

        protected AudioSource _audioSource;
        
        public void Setup(CallSO senderData, CallSO recipientData, AudioSource source, AudioClip sound)
        {
            senderPhoneText.text = senderData.phoneNumber;
            senderIcon.sprite = senderData.icon;
            recipientPhoneText.text = recipientData.phoneNumber;
            recipientIcon.sprite = recipientData.icon;
            _audioSource = source;
            _audioSource.clip = sound;
        }
        
        public void StopSound()
        {
            _audioSource?.Stop();
        }
    }
}