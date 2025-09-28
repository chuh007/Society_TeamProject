using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.Scripts.UI
{
    public class ExitPanel : MonoBehaviour
    {
        [SerializeField] private GameObject exitPanel;
        [SerializeField] private Button closePanelBtn;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button lobbyButton;

        private void Awake()
        {
            exitPanel.SetActive(false);
            closePanelBtn.onClick.AddListener(CloseExitPanel);
            exitButton.onClick.AddListener(ExitScene);
            lobbyButton.onClick.AddListener(LobbyScene);
        }

        private void OnDestroy()
        {
            closePanelBtn.onClick.RemoveListener(CloseExitPanel);
            exitButton.onClick.RemoveListener(ExitScene);
            lobbyButton.onClick.RemoveListener(LobbyScene);
        }

        public void OpenExitPanel()
        {
            exitPanel.transform.localScale = Vector3.zero;
            exitPanel.gameObject.SetActive(true);
            
            exitPanel.transform.DOScale(Vector3.one, 0.5f);
        }
        
        private void CloseExitPanel()
        {
            exitPanel.transform.DOScale(Vector3.zero, 0.5f)
                .OnComplete(() =>
                {
                    exitPanel.gameObject.SetActive(false);
                });
        }
        
        public void ExitScene()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        public void LobbyScene()
        {
            SceneManager.LoadScene("TitleScene");
        }
        
        
    }
}