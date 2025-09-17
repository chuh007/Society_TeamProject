using Scripts.Chatting.Enums;
using Scripts.Test;
using UnityEngine;

namespace Scripts.Stories
{
    public class StoryManager : MonoBehaviour
    {
        [SerializeField] private StoryTextPlayer storyPlayer;

        private void OnEnable()
        {
            DaySystemTest.OnDayEnd += HandleDayEnd;
        }

        private void OnDisable()
        {
            DaySystemTest.OnDayEnd -= HandleDayEnd;
        }

        private void HandleDayEnd(DayResultType resultType)
        {
            if (storyPlayer != null)
            {
                storyPlayer.PlayStory(resultType);
            }
        }
        
        
    }
}