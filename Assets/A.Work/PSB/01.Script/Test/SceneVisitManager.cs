using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Test
{
    public class SceneVisitManager : MonoBehaviour
    {
        public static SceneVisitManager Instance { get; private set; }
        
        public bool usePersistent = true;

        private const string PlayerPrefsKey = "VisitedScenes_v1";
        private HashSet<string> _visited = new HashSet<string>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (usePersistent)
                LoadFromPrefs();
        }

        #region Public API
        public bool HasVisited(string sceneName)
        {
            return _visited.Contains(sceneName);
        }
    
        public void MarkVisited(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            if (_visited.Add(sceneName) && usePersistent)
                SaveToPrefs();
        }
    
        public void ResetAllVisits()
        {
            _visited.Clear();
            if (usePersistent)
                PlayerPrefs.DeleteKey(PlayerPrefsKey);
        }
    
        public void UnmarkVisited(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            if (_visited.Remove(sceneName) && usePersistent)
                SaveToPrefs();
        }
        #endregion

        #region Persistence
        private void SaveToPrefs()
        {
            try
            {
                var joined = string.Join(",", _visited);
                PlayerPrefs.SetString(PlayerPrefsKey, joined);
                PlayerPrefs.Save();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"SceneVisitManager Save 실패: {e}");
            }
        }

        private void LoadFromPrefs()
        {
            if (!PlayerPrefs.HasKey(PlayerPrefsKey)) return;
            var joined = PlayerPrefs.GetString(PlayerPrefsKey);
            if (string.IsNullOrEmpty(joined)) return;
            var parts = joined.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts) _visited.Add(p);
        }
        #endregion
        
    }
}