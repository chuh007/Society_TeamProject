using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Test
{
    public class SceneGuard : MonoBehaviour
    {
        [Tooltip("이 씬이 이전에 방문된 적 있으면 이동할 대체 씬 이름")]
        public string fallbackScene = "GameScene";
        public bool markOnEnter = true;
        public bool disableGuard = false;

        private static HashSet<string> _visitedThisSession = new HashSet<string>();
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

            if (_visitedThisSession.Contains(current))
            {
                if (!string.IsNullOrEmpty(fallbackScene) && fallbackScene != current)
                {
                    _isRedirecting = true;
                    SceneManager.LoadScene(fallbackScene);
                    return;
                }
            }

            if (markOnEnter)
            {
                manager.MarkVisited(current);
                _visitedThisSession.Add(current);
            }
            else
            {
                _visitedThisSession.Add(current);
            }
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

        public void TrueTrigger()
        {
            markOnEnter = true;
        }
        
        
    }
}
