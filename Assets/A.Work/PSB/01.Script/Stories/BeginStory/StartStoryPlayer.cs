using System.Collections;
using Scripts.Chatting.System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Chatting.Stories.BeginStory
{
    public class StartStoryPlayer : MonoBehaviour
    {
        [SerializeField] private StoryDatabase storyDatabase;
        [SerializeField] private GameObject textPanel;
        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private float charDelay = 0.1f;

        [Header("스토리 종료 이벤트")]
        public UnityEvent OnStoryEnd;

        private StoryTextSO _currentStory;

        private void Awake()
        {
            textUI.text = "";
        }

        private void Start()
        {
            if (storyDatabase != null && storyDatabase.stories.Length > 0)
            {
                _currentStory = storyDatabase.stories[0];
                StartCoroutine(ShowMessagesCoroutine());
            }
        }

        private IEnumerator ShowMessagesCoroutine()
        {
            textPanel.SetActive(true);

            foreach (StoryNodes node in _currentStory.nodes)
            {
                foreach (string message in node.messages)
                {
                    yield return StartCoroutine(TypeMessage(message));
                    yield return new WaitForSeconds(_currentStory.nextTextDelay);
                }
            }
            
            OnStoryEnd?.Invoke();
        }

        public void ActiveFalsePanel()
        {
            textPanel.SetActive(false);
        }

        public void ActiveFalseText()
        {
            textUI.gameObject.SetActive(false);
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
        
        public void SkipStory()
        {
            if (_currentStory == null) return;

            StopAllCoroutines();

            textUI.text = "";
            foreach (StoryNodes node in _currentStory.nodes)
            {
                foreach (string message in node.messages)
                {
                    textUI.text += message + "\n";
                }
            }
            
            OnStoryEnd?.Invoke();
        }
        
        
    }
}