using System.Collections;
using Scripts.Chatting.System;
using Scripts.Cores;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Scripts.Chatting.Stories
{
    public class EndStoryPlayer : MonoBehaviour
    {
        [SerializeField] private StoryDatabase clearStoryDatabase;
        [SerializeField] private StoryDatabase failStoryDatabase;
        [SerializeField] private GameObject textPanel;
        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private float charDelay = 0.05f;

        [Header("스토리 종료 이벤트")]
        public UnityEvent OnStoryEnd;

        private StoryTextSO _currentStory;

        private void Awake()
        {
            textUI.text = "";
            DaySystem.OnFinalResult += HandleFinalResult;
            
            if (DaySystem.LastFinalResult.HasValue)
            {
                HandleFinalResult(DaySystem.LastFinalResult.Value);
            }
        }

        private void OnDestroy()
        {
            DaySystem.OnFinalResult -= HandleFinalResult;
        }

        private void HandleFinalResult(bool isClear)
        {
            StoryDatabase db = isClear ? clearStoryDatabase : failStoryDatabase;

            if (db == null || db.stories == null || db.stories.Length == 0)
            {
                Debug.LogWarning("EndStoryPlayer: 해당 데이터베이스가 비어있습니다.");
                return;
            }

            int idx = Random.Range(0, db.stories.Length);
            _currentStory = db.stories[idx];

            Debug.Log($"EndStoryPlayer: FinalResult={isClear} -> 선택된 스토리 index {idx}");

            StopAllCoroutines();
            StartCoroutine(ShowMessagesCoroutine());
        }
        
        public void ActiveFalseText()
        {
            textUI.gameObject.SetActive(false);
        }

        private IEnumerator ShowMessagesCoroutine()
        {
            textPanel.SetActive(true);
            textUI.gameObject.SetActive(true);
            textUI.text = "";

            foreach (var node in _currentStory.nodes)
            {
                foreach (string message in node.messages)
                {
                    yield return StartCoroutine(TypeMessage(message));
                    yield return new WaitForSeconds(_currentStory.nextTextDelay);
                }
            }

            Debug.Log("EndStoryPlayer: 스토리 종료.");
            OnStoryEnd?.Invoke();
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

        
        public void EndStory()
        {
            SaveSystem.Clear(SaveDTO.SaveKeys.ChatLogs); 
            SaveSystem.Clear(SaveDTO.SaveKeys.DayValue); 
            SaveSystem.Clear(SaveDTO.SaveKeys.SliderValue); 
        }
        
        
    }
}
