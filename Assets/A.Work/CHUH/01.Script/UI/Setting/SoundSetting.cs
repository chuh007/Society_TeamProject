using Ami.BroAudio;
using UnityEngine;
using UnityEngine.UI;

namespace A.Work.CHUH._01.Script.UI.Setting
{
    public class SoundSetting : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider BGMVolumeSlider;
        [SerializeField] private Slider SFXVolumeSlider;
        
        private void Awake()
        {
            masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChanged);
            BGMVolumeSlider.onValueChanged.AddListener(HandleBGMVolumeChanged);
            SFXVolumeSlider.onValueChanged.AddListener(HandleSFXVolumeChanged);
        }

        private void Start()
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat($"AllVolume");
            BGMVolumeSlider.value = PlayerPrefs.GetFloat($"MusicVolume");
            SFXVolumeSlider.value = PlayerPrefs.GetFloat($"SFXVolume");
        }
        
        public void SetVolume(BroAudioType type, float volume)
        {
            BroAudio.SetVolume(type, volume);
            PlayerPrefs.SetFloat($"{type.ToString()}Volume", volume);
        }
        
        private void HandleMasterVolumeChanged(float value)
        {
            SetVolume(BroAudioType.All, value);
        }
        
        private void HandleBGMVolumeChanged(float value)
        {
            SetVolume(BroAudioType.Music, value);
        }

        private void HandleSFXVolumeChanged(float value)
        {
            SetVolume(BroAudioType.SFX, value);
        }
    }
}