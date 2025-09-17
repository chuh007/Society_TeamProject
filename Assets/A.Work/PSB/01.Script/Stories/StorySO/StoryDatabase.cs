using UnityEngine;

namespace Scripts.Stories
{
    [CreateAssetMenu(fileName = "StoryDatabase", menuName = "SO/Base/Story", order = 10)]
    public class StoryDatabase : ScriptableObject
    {
        public StoryTextSO[] stories;
    }
}