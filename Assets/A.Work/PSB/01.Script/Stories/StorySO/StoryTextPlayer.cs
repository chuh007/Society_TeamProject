using System;
using System.Collections;
using System.Linq;
using Scripts.Chatting.Enums;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Stories
{
    public class StoryTextPlayer : MonoBehaviour
    {
        [SerializeField] private StoryDatabase storyDatabase;
        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private float charDelay = 0.1f;

        private StoryTextSO _currentStory;

        private void Awake()
        {
            textUI.text = "";
        }

        public void PlayStory(DayResultType type)
        {
            PickRandomStory(type);
            if (_currentStory == null) return;

            StopAllCoroutines();
            StartCoroutine(ShowMessagesCoroutine());
        }

        private void PickRandomStory(DayResultType type)
        {
            if (storyDatabase == null || storyDatabase.stories.Length == 0) return;

            var candidates = storyDatabase.stories
                .Where(s => s.whatIsType == type)
                .ToArray();

            if (candidates.Length == 0) return;

            int index = Random.Range(0, candidates.Length);
            _currentStory = candidates[index];
        }

        private IEnumerator ShowMessagesCoroutine()
        {
            foreach (var node in _currentStory.nodes)
            {
                foreach (var message in node.messages)
                {
                    yield return StartCoroutine(TypeMessage(message));
                    yield return new WaitForSeconds(_currentStory.nextTextDelay);
                }
            }
        }

        private IEnumerator TypeMessage(string message)
        {
            textUI.text = "";
            foreach (char c in message)
            {
                textUI.text += c;
                yield return new WaitForSeconds(charDelay);
            }
        }
        
        
    }
}