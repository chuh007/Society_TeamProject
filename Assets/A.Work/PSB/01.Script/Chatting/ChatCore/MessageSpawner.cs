using System;
using System.Collections;
using DG.Tweening;
using Scripts.Chatting.ChatSO;
using Scripts.Chatting.ChatSystem;
using Scripts.Cores;
using Scripts.Test;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Scripts.Chatting.ChatCore
{
    public class MessageSpawner : MonoBehaviour
    {
        [SerializeField] private MessageDatabaseSO messageDB;
        
        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private Transform messageParent;
        [SerializeField] private ChatWindow chatWindowPrefab;
        [SerializeField] private Transform chatParent;
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnInterval = 50f; 
        [SerializeField] private float cooldownAfterClick = 15f; 
        
        private int _spawnedToday;
        private bool _isMessageActive = false;
        private Coroutine _spawnRoutine;

        private void Start()
        {
            _spawnRoutine = StartCoroutine(TrySpawnRoutine());
        }
        
        private void OnEnable()
        {
            DaySystemTest.OnNextDay += ResetSpawnCount;
        }

        private void OnDisable()
        {
            DaySystemTest.OnNextDay -= ResetSpawnCount;
        }

        private void ResetSpawnCount()
        {
            _spawnedToday = 0;
        }

        private void Update()
        {
            if (Keyboard.current.sKey.wasPressedThisFrame)
                TrySpawnMessage();
        }

        private IEnumerator TrySpawnRoutine()
        {
            while (true)
            {
                while (GameTypeSingleton.Instance.GameType == GameType.Story
                       || GameTypeSingleton.Instance.GameType == GameType.Result)
                {
                    yield return null;
                }
                
                yield return new WaitForSeconds(spawnInterval);

                if (GameTypeSingleton.Instance.GameType != GameType.Story 
                    || GameTypeSingleton.Instance.GameType != GameType.Result)
                {
                    TrySpawnMessage();
                }
                
                float timer = 0f;
                while (timer < cooldownAfterClick)
                {
                    if (GameTypeSingleton.Instance.GameType == GameType.Story
                        || GameTypeSingleton.Instance.GameType == GameType.Result)
                        break;

                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        }

        
        private void TrySpawnMessage()
        {
            if (_spawnedToday >= 30)
            {
                Debug.LogError("Spawn false, because spawnedToday is upper 30");
                return;
            }
            else if (_isMessageActive)
            {
                Debug.LogError("Spawn false, because isMessageActive is true");
                return;
            }
            else if (IsChatWindowOpen())
            {
                Debug.LogError("Spawn false, because IsChatWindowOpen is true");
                return;
            }

            SpawnMessage();
        }


        private void SpawnMessage()
        {
            if (messageDB == null || messageDB.allMessages.Length == 0)
            {
                Debug.LogWarning("MessageDatabaseSO가 비어있습니다!");
                return;
            }
            
            for (int i = 0; i < 10; i++)
            {
                MessageSO randomSo = messageDB.allMessages[Random.Range(0, messageDB.allMessages.Length)];
        
                if (ChatListWindow.Instance != null && ChatListWindow.Instance.HasRoom(randomSo.roomId))
                {
                    Debug.Log($"이미 방 존재: {randomSo.roomName}, Preview 생성 안 함");
                    continue;
                }

                if (!ChatManager.Instance.HasAppeared(randomSo))
                {
                    GameObject inst = Instantiate(messagePrefab, messageParent);
                    PreviewMessage msg = inst.GetComponent<PreviewMessage>();
                    msg.Initialize(randomSo, chatWindowPrefab, chatParent);  
                    msg.OnMessageClicked += OnMessageClicked;

                    _isMessageActive = true;
                    return;
                }
            }
            _spawnedToday++;
        }

        private void OnMessageClicked()
        {
            _isMessageActive = false;
        }

        private bool IsChatWindowOpen()
        {
            foreach (var window in chatParent.GetComponentsInChildren<ChatWindow>(true))
            {
                if (window.gameObject.activeInHierarchy)
                    return true;
            }
            return false;
        }
        
        
    }
}
