using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scripts.UI
{
    public class TooltipButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Tooltip tooltip;
        
        private RectTransform _rectTrm;
        public bool IsActive { get; private set; }

        private void Awake()
        {
            _rectTrm = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            IsActive = true;
        }

        private void OnDisable()
        {
            IsActive = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltip != null && IsActive)
            {
                Invoke(nameof(ShowTooltip), tooltip.HoverDelay);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CancelInvoke();
            tooltip?.Hide();
        }

        private void ShowTooltip()
        {
            tooltip.Show();
            tooltip.RectTransform.position = new Vector2
            (
                _rectTrm.position.x + _rectTrm.rect.width * 0.5f,
                _rectTrm.position.y + _rectTrm.rect.height * 0.5f
            );
        }
        
        
    }
}