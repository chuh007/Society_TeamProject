using Ami.BroAudio;
using UnityEngine;

namespace Code.Scripts.UI
{
    public class PlayBackSound : MonoBehaviour
    {
        [SerializeField] private SoundID soundID;

        private void Start()
        {
            BroAudio.Stop(BroAudioType.Music);
            
            BroAudio.Play(soundID);
        }
        
    }
}