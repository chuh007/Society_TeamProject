using DG.Tweening;
using UnityEngine;

namespace Scripts.Test
{
    public class TestMessageSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private Transform messageParent;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                OpenUI();
            }
        }

        private void OpenUI()
        {
            GameObject inst = Instantiate(messagePrefab, messageParent);
            inst.gameObject.SetActive(true);
            inst.transform.DOMoveY(75, 0.5f);
        }
        
        
    }
}
