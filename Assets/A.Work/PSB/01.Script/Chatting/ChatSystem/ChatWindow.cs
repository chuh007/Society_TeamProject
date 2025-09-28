using System;
using System.Collections;
using A.Work.CHUH._01.Script.UI.Fraud;
using DG.Tweening;
using Scripts.Chatting.ChatSO;
using Scripts.Chatting.ChatUI;
using Scripts.Cores;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatSystem
{
    public class ChatWindow : MonoBehaviour, IDivisible
    {
        [Header("Layout")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform contentParent;

        [Header("Prefabs")]
        [SerializeField] private MessageBubble messagePrefab;    // 좌측 말풍선
        [SerializeField] private MessageBubble myMessagePrefab;  // 우측 말풍선
        [SerializeField] private ChoiceButton choiceButtonPrefab;

        [Header("Controls")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Button closeButton;

        public event Action<int, int> OnSuccess;
        public event Action<int, int> OnFail;
        public static event Action OnGlobalSuccess;
        public static event Action OnGlobalFail;
        public Action OnSave;

        private ConversationLog _log;
        private MessageSO _messageData;
        private int _currentNodeIndex;
        private Coroutine _playCoroutine;

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseWindow);
            
            transform.localScale = Vector3.zero;

            transform.DOScale(Vector3.one, 0.5f);
        }

        private void FixedUpdate()
        {
            if (GameTypeSingleton.Instance.GameType == GameType.Result 
                || GameTypeSingleton.Instance.GameType == GameType.Story
                || GameTypeSingleton.Instance.GameType == GameType.Clear
                || GameTypeSingleton.Instance.GameType == GameType.Fail)
            {
                CloseWindow();
            }
        }

        private void OnEnable()
        {
            JudgeSlider.Register(this);
        }

        private void OnDisable()
        {
            JudgeSlider.Unregister(this);
        }

        public void BringToFront() => transform.SetAsLastSibling();

        #region 창 열기
        
        public void ReOpen(MessageSO messageData, ConversationLog log, Action onSave)
        {
            transform.DOScale(Vector3.one, 0.5f)
                .OnComplete(() =>
                {
                    gameObject.SetActive(true);
                });
            SetupRoom(messageData, log, onSave, false);
        }

        public void OpenRoom(MessageSO messageData, ConversationLog log, Action onSave)
        {
            transform.DOScale(Vector3.one, 0.5f)
                .OnComplete(() =>
                {
                    gameObject.SetActive(true);
                });
            SetupRoom(messageData, log, onSave, true);
        }
        #endregion

        #region 방 세팅
        
        private void SetupRoom(MessageSO messageData, ConversationLog log, Action onSave, bool clampIndex)
        {
            _messageData = messageData;
            _log = log;
            OnSave = onSave;
            nameText.text = _messageData.roomName;

            _currentNodeIndex = clampIndex
                ? Mathf.Clamp(_log.currentNodeIndex, 0, _messageData.nodes.Length)
                : _log.currentNodeIndex;

            ResetContent();
            RestoreMessages();

            if (_log.judgeState != SpamJudgeState.UnKnown)
            {
                ShowJudgeResult(false);
                return;
            }

            if (_currentNodeIndex >= _messageData.nodes.Length)
            {
                ShowEndJudgeButtons();
                return;
            }

            _playCoroutine = StartCoroutine(PlayNode(_currentNodeIndex));
        }

        private void ResetContent()
        {
            ClearButtons();
            if (_playCoroutine != null)
                StopCoroutine(_playCoroutine);

            foreach (Transform child in contentParent)
                Destroy(child.gameObject);
        }

        private void RestoreMessages()
        {
            foreach (MessageData msgData in _log.messages)
                SpawnMessageBubble(msgData.text, msgData.isMine);
        }

        private void ShowJudgeResult(bool triggerEvent = true)
        {
            bool isSuccess = _log.judgeState == _messageData.isSpam;

            if (triggerEvent)
            {
                if (isSuccess)
                {
                    OnSuccess?.Invoke(_messageData.value, _messageData.gradeSO.successMultiplier);
                    OnGlobalSuccess?.Invoke();
                }
                else
                {
                    OnFail?.Invoke(_messageData.value, _messageData.gradeSO.failMultiplier);
                    OnGlobalFail?.Invoke();
                }
            }

            if (isSuccess)
                SpawnMessageBubble("성공! 올바르게 판별했습니다.", false);
            else
                SpawnMessageBubble("실패! 잘못 판별했습니다.", false);
        }
        #endregion

        #region 대화 
        
        private IEnumerator PlayNode(int index)
        {
            if (_messageData == null || index < 0 || index >= _messageData.nodes.Length)
            {
                ShowEndJudgeButtons();
                yield break;
            }

            DialogueNode node = _messageData.nodes[index];

            foreach (string msg in node.messages)
            {
                if (!_log.messages.Exists(m => m.text == msg))
                {
                    SpawnMessageBubble(msg, false);
                    _log.messages.Add(new MessageData { text = msg, isMine = false });
                    ScrollToBottom();
                    yield return new WaitForSeconds(0.4f);
                }
            }

            if (node.choices != null && node.choices.Length > 0)
            {
                bool autoJudge = node.choices.Length == 1 &&
                                 string.IsNullOrEmpty(node.choices[0].answer) &&
                                 node.choices[0].nextNodeIndex == -1;

                if (autoJudge)
                {
                    _log.currentNodeIndex = _messageData.nodes.Length;
                    OnSave?.Invoke();
                    ShowEndJudgeButtons();
                }
                else
                {
                    foreach (DialogueChoice choice in node.choices)
                    {
                        ChoiceButton btn = Instantiate(choiceButtonPrefab, contentParent);
                        btn.Setup(choice.answer, Color.white, () => OnChoice(choice.answer, choice.nextNodeIndex));
                        ScrollToBottom();
                    }
                }
            }
            else
            {
                if (index + 1 < _messageData.nodes.Length)
                {
                    _log.currentNodeIndex = index + 1;
                    OnSave?.Invoke();
                    _playCoroutine = StartCoroutine(PlayNode(index + 1));
                }
                else
                {
                    _log.currentNodeIndex = _messageData.nodes.Length;
                    OnSave?.Invoke();
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
                OnSave?.Invoke();
                ShowEndJudgeButtons();
                return;
            }

            _currentNodeIndex = Mathf.Clamp(nextIndex, 0, _messageData.nodes.Length);
            _log.currentNodeIndex = _currentNodeIndex;

            if (_playCoroutine != null)
                StopCoroutine(_playCoroutine);

            if (_currentNodeIndex < _messageData.nodes.Length)
                _playCoroutine = StartCoroutine(PlayNode(_currentNodeIndex));
            else
                ShowEndJudgeButtons();

            OnSave?.Invoke();
        }
        #endregion

        #region 판단
        
        private void OnSpamJudge(SpamJudgeState judgedSpam)
        {
            ClearButtons();
            _log.judgeState = judgedSpam;
            ShowJudgeResult(true);
            OnSave?.Invoke();
        }

        private void ShowEndJudgeButtons()
        {
            ClearButtons();

            ChoiceButton spamBtn = Instantiate(choiceButtonPrefab, contentParent);
            spamBtn.Setup("스팸", Color.red, () => OnSpamJudge(SpamJudgeState.Spam));

            ChoiceButton normalBtn = Instantiate(choiceButtonPrefab, contentParent);
            normalBtn.Setup("정상", Color.green, () => OnSpamJudge(SpamJudgeState.NotSpam));

            ScrollToBottom();
        }
        #endregion

        #region 메시지, 버튼 등
        
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
                    Destroy(t.gameObject);
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
            transform.DOScale(Vector3.zero, 0.5f)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }

        #endregion
        
    }
}
