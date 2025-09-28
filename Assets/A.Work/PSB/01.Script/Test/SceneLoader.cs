using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.Test
{
    public static class SceneLoader
    {
        public static void LoadSceneSkipIfVisited(string targetScene)
        {
            var manager = SceneVisitManager.Instance;
            if (manager == null)
            {
                SceneManager.LoadScene(targetScene);
                return;
            }
            
            if (manager.HasVisited(targetScene))
            {
                int targetIndex = SceneUtility.GetBuildIndexByScenePath(SceneUtility.GetScenePathByBuildIndex(0)); 
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    string path = SceneUtility.GetScenePathByBuildIndex(i);
                    string name = Path.GetFileNameWithoutExtension(path);
                    if (name == targetScene)
                    {
                        targetIndex = i;
                        break;
                    }
                }

                int nextIndex = targetIndex + 1;
                if (nextIndex < SceneManager.sceneCountInBuildSettings)
                {
                    string nextPath = SceneUtility.GetScenePathByBuildIndex(nextIndex);
                    string nextName = Path.GetFileNameWithoutExtension(nextPath);
                    SceneManager.LoadScene(nextName);
                    return;
                }
            }

            SceneManager.LoadScene(targetScene);
            manager.MarkVisited(targetScene);
        }
        
    }
}