using System;
using Scripts.Chatting.Enums;
using UnityEngine;

namespace Scripts.Chatting.Stories
{
    [Serializable]
    public class StoryNodes
    {
        [TextArea] public string[] messages;
    }
    
    [CreateAssetMenu(fileName = "StoryText", menuName = "SO/Story/Message", order = 0)]
    public class StoryTextSO : ScriptableObject
    {
        [Header("Text")]
        public StoryNodes[] nodes;
        
        [Header("GradeValue")]
        public DayResultType whatIsType;

        [Header("Value")] 
        public int nextTextDelay;

    }
}