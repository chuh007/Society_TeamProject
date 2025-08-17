using UnityEngine;

namespace Scripts.Test
{
    [CreateAssetMenu(fileName = "MessageText", menuName = "SO/Message", order = 0)]
    public class MessageSO : ScriptableObject
    {
        public Texture2D icon;
        public string number;
        [TextArea] public string message;
    }
}