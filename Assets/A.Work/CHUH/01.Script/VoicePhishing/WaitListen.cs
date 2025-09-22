using UnityEngine;
using UnityEngine.UI;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class WaitListen : Call
    {
        public void PlaySound()
        {
            _audioSource.loop = true;
            _audioSource.Play();
        }


    }
}