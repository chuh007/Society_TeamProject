using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Chatting.ChatUI
{
    public class SliderMarkUI : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        [SerializeField] private RawImage markImage;
        [SerializeField] private TextMeshProUGUI sliderText;
        
        [Header("Images")]
        [SerializeField] private Texture2D[] markSpriteRenderer;
        
        private void Update()
        {
            if (markImage != null)
            {
                float gauge = slider.value;
                sliderText.text = gauge.ToString();

                if (gauge >= 100f) markImage.texture = markSpriteRenderer[0];
                else if (gauge >= 70f) markImage.texture = markSpriteRenderer[1];
                else if (gauge >= 50f) markImage.texture = markSpriteRenderer[2];
                else if (gauge >= 20f) markImage.texture = markSpriteRenderer[3];
                else markImage.texture = markSpriteRenderer[4];
            }
        }
        
    }
}