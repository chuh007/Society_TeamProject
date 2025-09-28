using System;
using UnityEngine;

namespace Code.Scripts.Test
{
    public class ResetVisitedTool : MonoBehaviour
    {
        private void Start()
        {
            ResetAllVisited();
        }

        public void ResetAllVisited()
        {
            if (SceneVisitManager.Instance != null)
            {
                SceneVisitManager.Instance.ResetAllVisits();
            }
            else
            {
                Debug.LogWarning("SceneVisitManager 인스턴스 없음. 먼저 SceneVisitManager를 씬에 추가하세요.");
            }
        }
        
    }
}