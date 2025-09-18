using Scripts.Chatting.Enums;
using Scripts.Test;
using UnityEngine;

namespace Scripts.Chatting.Stories
{
    public class StoryManager : MonoBehaviour
    {
        [SerializeField] private StoryTextPlayer storyPlayer;

        private void OnEnable()
        {
            DaySystemTest.OnResultClose += HandleResultClose;
        }

        private void OnDisable()
        {
            DaySystemTest.OnResultClose -= HandleResultClose;
        }

        private void HandleResultClose(DayResultType resultType)
        {
            if (storyPlayer != null)
            {
                storyPlayer.PlayStory(resultType);
            }
        }
        
        
    }
}