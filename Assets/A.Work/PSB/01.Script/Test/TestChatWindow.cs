using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Test
{
    public class TestChatWindow : MonoBehaviour, IMessageOpener
    {
        [SerializeField] private ScrollRect scrollRect;
        
        [SerializeField] private Transform contentParent;

        [Header("Prefabs")]
        [SerializeField] private GameObject messagePrefab;     // 좌측 말풍선
        [SerializeField] private GameObject myMessagePrefab;   // 우측 말풍선
        [SerializeField] private GameObject choiceButtonPrefab;

        private MessageSO _messageData;
        private int _currentNodeIndex;
        private Coroutine _playCoroutine;

        public void Open(MessageSO messageData)
        {
            gameObject.SetActive(true);
            _messageData = messageData; 
            _currentNodeIndex = 0;

            if (_playCoroutine != null)
                StopCoroutine(_playCoroutine);

            // Content 초기화
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            // 버튼 초기화
            foreach (Transform child in contentParent)
                Destroy(child.gameObject);

            _playCoroutine = StartCoroutine(PlayNode(_currentNodeIndex));
        }

        private IEnumerator PlayNode(int index)
        {
            DialogueNode node = _messageData.nodes[index];

            // 메시지 출력 (좌측)
            foreach (string msg in node.messages)
            {
                GameObject msgObj = Instantiate(messagePrefab, contentParent);
                msgObj.GetComponentInChildren<TextMeshProUGUI>().text = msg;
                ScrollToBottom();
                yield return new WaitForSeconds(0.5f);
            }

            // 선택지 출력 (우측, Content에 바로 삽입)
            if (node.choices != null && node.choices.Length > 0)
            {
                foreach (var choice in node.choices)
                {
                    GameObject btnObj = Instantiate(choiceButtonPrefab, contentParent);
                    btnObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.answer;
                    ScrollToBottom();

                    int nextIndex = choice.nextNodeIndex;
                    btnObj.GetComponentInChildren<Button>().onClick.AddListener(() => OnChoice(btnObj, nextIndex));
                }
            }
            else
            {
                EndDialogue();
            }

            _playCoroutine = null;
        }

        private void OnChoice(GameObject btnObj, int nextIndex)
        {
            string choiceText = btnObj.GetComponentInChildren<TextMeshProUGUI>().text;

            // 1) 다른 버튼 전부 제거 (내가 누른 버튼도 포함)
            foreach (Transform child in contentParent)
            {
                if (child.GetComponentInChildren<Button>() != null)
                    Destroy(child.gameObject);
            }

            // 2) 내가 누른 버튼 위치에 내 메시지(오른쪽 말풍선) 버블 생성
            GameObject choiceBubble = Instantiate(myMessagePrefab, contentParent);
            choiceBubble.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;
            ScrollToBottom();

            if (nextIndex < 0)
            {
                EndDialogue();
                return;
            }
    
            _currentNodeIndex = nextIndex;

            if (_playCoroutine != null)
                StopCoroutine(_playCoroutine);

            _playCoroutine = StartCoroutine(PlayNode(_currentNodeIndex));
        }

        private void EndDialogue()
        {
            Debug.Log(_messageData.isSpam ? "⚠️ 스팸 메시지!" : "✅ 정상 메시지!");
        }
        
        private void ScrollToBottom()
        {
            StartCoroutine(ScrollToBottomCoroutine());
        }

        private IEnumerator ScrollToBottomCoroutine()
        {
            yield return null; // 한 프레임 기다리기 (레이아웃 갱신 후 실행)
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
        }
        
        
    }
}
