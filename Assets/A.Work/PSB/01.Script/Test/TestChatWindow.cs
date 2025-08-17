using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Test
{
    public class TestChatWindow : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject messageBubblePrefab;
        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private Transform choiceParent;

        private MessageSO _data;
        private int _currentNodeIndex;

        public void Open(MessageSO data)
        {
            _data = data;
            _currentNodeIndex = 0;
            ShowNode(_currentNodeIndex);
        }

        private void ShowNode(int nodeIndex)
        {
            DialogueNode node = _data.nodes[nodeIndex];

            // 메시지 출력
            foreach (string msg in node.messages)
            {
                GameObject bubble = Instantiate(messageBubblePrefab, content);
                bubble.GetComponentInChildren<TextMeshProUGUI>().text = msg;
            }

            // 기존 선택지 제거
            foreach (Transform child in choiceParent)
                Destroy(child.gameObject);

            // 선택지 버튼 생성
            foreach (DialogueChoice choice in node.choices)
            {
                GameObject btnObj = Instantiate(choiceButtonPrefab, choiceParent);
                btnObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.answer;

                btnObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ShowNode(choice.nextNodeIndex);
                });
            }
        }
        
    }
}