using A.Work.CHUH._01.Script.Call;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class Wiretapping : Call
    {
        public void Play(float time)
        {
            _audioSource.time = time;
            _audioSource.Play();
        }
        
    }
}