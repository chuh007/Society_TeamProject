using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Test
{
    public class SceneGuard : MonoBehaviour
    {
        [Tooltip("이 씬이 이전에 방문된 적 있으면 이동할 대체 씬 이름")]
        public string fallbackScene = "GameScene";

        [Tooltip("이 씬이 처음 방문이면 자동으로 방문 처리(MarkVisited)")]
        public bool markOnEnter = true;

        [Tooltip("Guard가 동작하지 않도록 예외 처리: true면 검사 안함 (디버그용)")]
        public bool disableGuard = false;
        
        private static bool _isRedirecting = false;

        private void Start()
        {
            if (disableGuard) return;

            var manager = SceneVisitManager.Instance;
            if (manager == null)
            {
                Debug.LogWarning("SceneVisitManager가 씬에 없습니다. SceneGuard가 동작하려면 SceneVisitManager가 필요합니다.");
                return;
            }

            string current = SceneManager.GetActiveScene().name;
            
            if (_isRedirecting) return;

            if (manager.HasVisited(current))
            {
                if (!string.IsNullOrEmpty(fallbackScene) && fallbackScene != current)
                {
                    _isRedirecting = true;
                    SceneManager.LoadScene(fallbackScene);
                }
                return;
            }
            
            if (markOnEnter)
                manager.MarkVisited(current);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _isRedirecting = false;
        }
        
    }
}