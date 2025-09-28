using Ami.BroAudio;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace A.Work.CHUH._01.Script.UI.Setting
{
    public class ButtonSoundManager : MonoBehaviour
    {
        [SerializeField] private SoundID buttonSound;
        
        public void PlayButtonSound()
        {
            BroAudio.Play(buttonSound);
        }
        
#if UNITY_EDITOR
        [ContextMenu("Auto Button Sound Setting")]
        private void AutoButtonSound()
        {
            var scene = SceneManager.GetActiveScene();
            var roots = scene.GetRootGameObjects();
            
            foreach (var root in roots)
            {
                var buttons = root.GetComponentsInChildren<Button>(true);
                foreach (var btn in buttons)
                {
                    UnityEventTools.AddPersistentListener(btn.onClick, PlayButtonSound);
                    EditorUtility.SetDirty(btn);
                }
            }
            
            EditorSceneManager.MarkSceneDirty(scene);
        }
#endif
        
    }
}