using System.Collections;
using Scripts.Chatting.ChatCore;
using Scripts.Chatting.ChatSO;
using Scripts.Chatting.ChatUI;
using Scripts.Test;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatSystem
{
    public class ChatWindow : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform contentParent;

        [Header("Prefabs")]
        [SerializeField] private MessageBubble messagePrefab;    // 좌측 말풍선
        [SerializeField] private MessageBubble myMessagePrefab;  // 우측 말풍선
        [SerializeField] private ChoiceButton choiceButtonPrefab;

        [Header("Controls (Optional)")]
        [SerializeField] private Button closeButton;

        private ConversationLog _log;
        private MessageSO _messageData;
        private int _currentNodeIndex;
        private Coroutine _playCoroutine;
        private System.Action _onSave;

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseWindow);
        }

        public void BringToFront() => transform.SetAsLastSibling();

        public void ReOpen(MessageSO messageData, ConversationLog log, System.Action onSave)
        {
            _messageData = messageData;
            _log = log;
            _onSave = onSave;
            _currentNodeIndex = _log.currentNodeIndex;
        
            ClearButtons();
        
            if (_playCoroutine != null) StopCoroutine(_playCoroutine);
        
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        
            foreach (var msgData in _log.messages)
            {
                SpawnMessageBubble(msgData.text, msgData.isMine);
            }
        
            // 대화가 끝났거나 판별이 완료된 상태를 확인하고 처리
            if (_currentNodeIndex >= _messageData.nodes.Length)
            {
                if (!_log.judgedSpam.HasValue)
                {
                    ShowEndJudgeButtons(); // 판별이 안 된 경우 버튼 표시
                }
                else
                {
                    SpawnMessageBubble("판별이 완료되었습니다.", false);
                }
                return;
            }
            
            _playCoroutine = StartCoroutine(PlayNode(_currentNodeIndex));
        }

        public void OpenRoom(MessageSO messageData, ConversationLog log, System.Action onSave)
        {
            gameObject.SetActive(true);
            _messageData = messageData;
            _log = log;
            _onSave = onSave;

            _currentNodeIndex = Mathf.Clamp(_log.currentNodeIndex, 0, _messageData.nodes.Length);
            
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
            }
            
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var msgData in _log.messages)
            {
                SpawnMessageBubble(msgData.text, msgData.isMine);
            }
            
            if (_currentNodeIndex >= _messageData.nodes.Length)
            {
                if (!_log.judgedSpam.HasValue)
                {
                    ShowEndJudgeButtons();
                }
                else
                {
                    SpawnMessageBubble("판별이 완료되었습니다.", false);
                }
                return;
            }

            _playCoroutine = StartCoroutine(PlayNode(_currentNodeIndex));
        }
        
        private IEnumerator PlayNode(int index)
        {
            if (_messageData == null || index < 0 || index >= _messageData.nodes.Length)
            {
                ShowEndJudgeButtons();
                yield break;
            }

            DialogueNode node = _messageData.nodes[index];
            
            // ... (메시지 중복 출력 방지 로직은 기존과 동일)
            foreach (string msg in node.messages)
            {
                bool isAlreadyLogged = false;
                foreach (var loggedMsg in _log.messages)
                {
                    if (loggedMsg.text == msg)
                    {
                        isAlreadyLogged = true;
                        break;
                    }
                }

                if (!isAlreadyLogged)
                {
                    SpawnMessageBubble(msg, false);
                    _log.messages.Add(new MessageData { text = msg, isMine = false });
                    ScrollToBottom();
                    yield return new WaitForSeconds(0.4f);
                }
            }
            
            if (node.choices != null && node.choices.Length > 0)
            {
                foreach (var choice in node.choices)
                {
                    ChoiceButton btn = Instantiate(choiceButtonPrefab, contentParent);
                    btn.Setup(choice.answer, Color.white, () => OnChoice(choice.answer, choice.nextNodeIndex));
                    ScrollToBottom();
                }
            }
            else
            {
                // 선택지가 없는 노드에 도달하면 다음 노드로 넘어가거나 종료
                if (index + 1 < _messageData.nodes.Length)
                {
                    _log.currentNodeIndex = index + 1;
                    _onSave?.Invoke();
                    _playCoroutine = StartCoroutine(PlayNode(index + 1));
                }
                else
                {
                    // 다음 노드가 없으면 대화 종료
                    _log.currentNodeIndex = _messageData.nodes.Length;
                    _onSave?.Invoke();
                    ShowEndJudgeButtons();
                }
            }
            
            _playCoroutine = null;
        }

        private void OnChoice(string choiceText, int nextIndex)
        {
            ClearButtons();

            SpawnMessageBubble(choiceText, true);
            _log.messages.Add(new MessageData { text = choiceText, isMine = true });
            ScrollToBottom();

            if (nextIndex < 0)
            {
                _log.currentNodeIndex = _messageData.nodes.Length;
                _onSave?.Invoke();
                ShowEndJudgeButtons();
                return;
            }

            _currentNodeIndex = Mathf.Clamp(nextIndex, 0, _messageData.nodes.Length);
            _log.currentNodeIndex = _currentNodeIndex;

            if (_playCoroutine != null)
                StopCoroutine(_playCoroutine);

            if (_currentNodeIndex < _messageData.nodes.Length)
            {
                _playCoroutine = StartCoroutine(PlayNode(_currentNodeIndex));
            }
            else
            {
                ShowEndJudgeButtons();
            }

            _onSave?.Invoke();
        }

        private void OnSpamJudge(bool judgedSpam)
        {
            ClearButtons();
            _log.judgedSpam = judgedSpam;

            Debug.Log(judgedSpam == _messageData.isSpam
                ? "성공 올바르게 판별했습니다."
                : "실패 잘못 판별했습니다.");
            
            SpawnMessageBubble("판별이 완료되었습니다.", false);
            _onSave?.Invoke();
        }

        private void ShowEndJudgeButtons()
        {
            ClearButtons();

            ChoiceButton spamBtn = Instantiate(choiceButtonPrefab, contentParent);
            spamBtn.Setup("스팸", Color.red, () => OnSpamJudge(true));

            ChoiceButton normalBtn = Instantiate(choiceButtonPrefab, contentParent);
            normalBtn.Setup("정상", Color.green, () => OnSpamJudge(false));

            ScrollToBottom();
        }

        private void SpawnMessageBubble(string msg, bool isMine)
        {
            MessageBubble prefab = isMine ? myMessagePrefab : messagePrefab;
            MessageBubble obj = Instantiate(prefab, contentParent);
            obj.SetText(msg);
        }

        private void ClearButtons()
        {
            for (int i = contentParent.childCount - 1; i >= 0; i--)
            {
                Transform t = contentParent.GetChild(i);
                if (t.GetComponent<ChoiceButton>() != null)
                {
                    Destroy(t.gameObject);
                }
            }
        }

        private void ScrollToBottom()
        {
            StartCoroutine(ScrollToBottomCoroutine());
        }

        private IEnumerator ScrollToBottomCoroutine()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f;
                Canvas.ForceUpdateCanvases();
            }
        }

        private void CloseWindow()
        {
            gameObject.SetActive(false);
        }
        
        
    }
}