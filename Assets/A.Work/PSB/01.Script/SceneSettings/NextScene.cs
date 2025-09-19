using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.SceneSettings
{
    public class NextScene : MonoBehaviour
    {
        [SerializeField] private string nextScene;

        public void NextSceneMethod()
        {
            SceneManager.LoadScene(nextScene);
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
    }
}