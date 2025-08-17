using UnityEngine;

namespace Scripts.Test
{
    [System.Serializable]
    public class DialogueNode
    {
        [TextArea] public string[] messages;
        public DialogueChoice[] choices;
    }

    [System.Serializable]
    public class DialogueChoice
    {
        [TextArea] public string answer;
        public int nextNodeIndex;
    }
    
    [CreateAssetMenu(fileName = "MessageText", menuName = "SO/Message", order = 0)]   
    public class MessageSO : ScriptableObject
    {
        public Texture2D icon;
        public string number;
        [TextArea] public string[] messagePreview;
        
        public DialogueNode[] nodes;
        public bool isSpam;
    }
}