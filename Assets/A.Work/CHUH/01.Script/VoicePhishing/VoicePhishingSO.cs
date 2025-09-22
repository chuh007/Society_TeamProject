using A.Work.CHUH._01.Script.Call;
using UnityEngine;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    [CreateAssetMenu(fileName = "VoicePhishing", menuName = "SO/Call/VoicePhishing", order = 0)]
    public class VoicePhishingSO : ScriptableObject
    {
        public AudioClip voiceClip;
        public bool isFraud;
        public CallSO sender;
        public CallSO recipient;
        [TextArea] public string description;
    }
}