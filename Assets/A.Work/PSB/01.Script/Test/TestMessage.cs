using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Test
{
    public class TestMessage : MonoBehaviour
    {
        [SerializeField] private MessageSO[] messages;
        [SerializeField] private RawImage iconImage;
        [SerializeField] private TextMeshProUGUI numTxt;
        [SerializeField] private TextMeshProUGUI messageTxt;

        private Button _messageBtn;
        
        private void Awake()
        {
            _messageBtn = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _messageBtn.onClick.AddListener(ClickMessage);
        }

        private void OnDestroy()
        {
            _messageBtn.onClick.AddListener(ClickMessage);
        }

        private void Start()
        {
            int rand = Random.Range(0, messages.Length);
            iconImage.texture = messages[rand].icon;
            numTxt.text = messages[rand].number;
        }

        private void ClickMessage()
        {
            Debug.Log("Clicked!");
        }
        
        
    }
}