using System.Collections;
using DG.Tweening;
using Scripts.Chatting.ChatSO;
using Scripts.Chatting.ChatSystem;
using UnityEngine;

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
        
        private bool _isMessageActive = false;
        private Coroutine _spawnRoutine;

        private void Start()
        {
            _spawnRoutine = StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            yield return new WaitForSeconds(spawnInterval);
            
            TrySpawnMessage();
            
            while (true)
            {
                yield return new WaitForSeconds(cooldownAfterClick);
                TrySpawnMessage();
            }
        }
        
        private void TrySpawnMessage()
        {
            if (_isMessageActive || IsChatWindowOpen())
                return;

            SpawnMessage();
        }


        private void SpawnMessage()
        {
            if (messageDB == null || messageDB.allMessages.Length == 0)
            {
                Debug.LogWarning("MessageDatabaseSO가 비어있습니다!");
                return;
            }

            MessageSO randomSo = messageDB.allMessages[Random.Range(0, messageDB.allMessages.Length)];

            GameObject inst = Instantiate(messagePrefab, messageParent);
            PreviewMessage msg = inst.GetComponent<PreviewMessage>();
            msg.Initialize(randomSo, chatWindowPrefab, chatParent);
            
            _isMessageActive = true;
            
            msg.OnMessageClicked += OnMessageClicked;
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
