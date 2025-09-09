using Scripts.Chatting.Enums;
using UnityEngine;

namespace Scripts.Chatting.ChatSO
{
    [CreateAssetMenu(fileName = "GradeSO", menuName = "SO/Grade", order = 0)]
    public class MessageGradeSO : ScriptableObject
    {
        public MessageGrade gradeEnum;
        public int successMultiplier;
        public int failMultiplier;
    }
}