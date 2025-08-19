using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatUI
{
    public class ChoiceButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Image background;

        public void Setup(string text, Color color, UnityAction onClick)
        {
            if (label != null) label.text = text;
            if (background != null) background.color = color;

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(onClick);
            }
        }
        
    }
}