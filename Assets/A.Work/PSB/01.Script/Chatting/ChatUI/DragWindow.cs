using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Chatting.ChatUI
{
    public class DragWindow : MonoBehaviour, IDragHandler, IPointerDownHandler, IEndDragHandler
    {
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private RectTransform _canvasRect;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasRect = _canvas.GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _rectTransform.SetAsLastSibling();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ClampBackToWindow();
        }

        private void ClampBackToWindow()
        {
            Vector3 pos = _rectTransform.anchoredPosition;

            Vector3 canvasSize = _canvasRect.rect.size;
            Vector3 panelSize = _rectTransform.rect.size;

            float halfWidth = panelSize.x * 0.2f;
            float halfHeight = panelSize.y * 0.08f;

            float minX = -canvasSize.x * 0.4f + halfWidth;
            float maxX = canvasSize.x * 0.4f - halfWidth;
            float minY = -canvasSize.y * 0.1f + halfHeight;
            float maxY = canvasSize.y * 0.1f - halfHeight;
            
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            _rectTransform.anchoredPosition = pos;
        }
        
        
    }
}