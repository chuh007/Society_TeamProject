using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI
{
    public class RedirectBtn : MonoBehaviour
    {
        [SerializeField] private string link;

        private void Start()
        {
            RedirectLink();
        }

        private void RedirectLink()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                Application.OpenURL(link);
            });
        }
        
    }
}