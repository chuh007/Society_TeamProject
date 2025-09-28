using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Scripts.Chatting.Enums;
using Scripts.Cores;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scripts.Chatting.Stories
{
    public class StoryTextPlayer : MonoBehaviour
    {
        [SerializeField] private StoryDatabase storyDatabase;
        [SerializeField] private GameObject textPanel;
        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private float charDelay = 0.1f;

        private StoryTextSO _currentStory;

        private void Awake()
        {
            textUI.text = "";
            textPanel.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            //나중에 DoTween, 대기 추가하기, 좀 더 효율적인 방법 찾기
            if (GameTypeSingleton.Instance.GameType == GameType.Story)
                Open();
            else
                Close();
        }

        private void Open()
        {
            textPanel.transform.localScale = Vector3.one;
            textPanel.gameObject.SetActive(true);
        }
        
        private void Close()
        {
            textPanel.transform.DOScale(Vector3.zero, 0.5f)
                .OnComplete(() =>
                {
                    textPanel.gameObject.SetActive(false);
                });
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

            StoryTextSO[] candidates = storyDatabase.stories
                .Where(s => s.whatIsType == type)
                .ToArray();

            if (candidates.Length == 0) return;

            int index = Random.Range(0, candidates.Length);
            _currentStory = candidates[index];
        }

        private IEnumerator TextPanelShow()
        {
            textPanel.SetActive(true);
            yield return null;
        }

        private IEnumerator ShowMessagesCoroutine()
        {
            foreach (StoryNodes node in _currentStory.nodes)
            {
                foreach (string message in node.messages)
                {
                    yield return StartCoroutine(TypeMessage(message));
                    yield return new WaitForSeconds(_currentStory.nextTextDelay);
                }
            }

            GameTypeSingleton.Instance.GameType = GameType.Game;
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