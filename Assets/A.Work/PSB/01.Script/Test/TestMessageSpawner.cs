using DG.Tweening;
using UnityEngine;

namespace Scripts.Test
{
    public class TestMessageSpawner : MonoBehaviour
    {
        [SerializeField] private MessageDatabaseSO messageDB;
        
        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private Transform messageParent;
        [SerializeField] private TestChatWindow chatWindowPrefab;
        [SerializeField] private Transform chatParent;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                // ✅ 랜덤 메시지 SO 뽑기
                if (messageDB == null || messageDB.allMessages.Length == 0)
                {
                    Debug.LogWarning("MessageDatabaseSO가 비어있습니다!");
                    return;
                }

                MessageSO randomSo = messageDB.allMessages[Random.Range(0, messageDB.allMessages.Length)];

                // 메시지 UI 생성
                GameObject inst = Instantiate(messagePrefab, messageParent);
                inst.transform.DOMoveY(75, 0.5f);

                TestMessage msg = inst.GetComponent<TestMessage>();
                msg.Initialize(randomSo, chatWindowPrefab, chatParent);
            }
        }
        
        
    }
}
