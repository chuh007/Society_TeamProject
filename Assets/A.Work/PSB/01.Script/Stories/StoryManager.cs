using Scripts.Chatting.Enums;
using Scripts.Cores;
using UnityEngine;

namespace Scripts.Chatting.Stories
{
    public class StoryManager : MonoBehaviour
    {
        [SerializeField] private StoryTextPlayer storyPlayer;

        private void OnEnable()
        {
            DaySystem.OnResultClose += HandleResultClose;
        }

        private void OnDisable()
        {
            DaySystem.OnResultClose -= HandleResultClose;
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