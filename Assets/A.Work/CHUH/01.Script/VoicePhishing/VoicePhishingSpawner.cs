using System.Collections;
using Scripts.Cores;
using UnityEngine;
using Random = UnityEngine.Random;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class VoicePhishingSpawner : MonoBehaviour
    {
        [SerializeField] private Canvas spawnCanvas;
        [SerializeField] private VoicePhishing voicePhishingPrefab;
        [SerializeField] private PhishingDataList phishingDataList;
        [SerializeField] private DaySystem daySystem;
        [SerializeField] private float spawnCooldown;
        
        
        #region Test

        [ContextMenu("Spawn")]
        public void TestSpawn()
        {
            Spawn();
        }

        #endregion
        
        private void Start()
        {
            StartCoroutine(TrySpawn());
        }

        private IEnumerator TrySpawn()
        {
            yield return new WaitForSeconds(spawnCooldown);
            if (GameTypeSingleton.Instance.GameType == GameType.Game)
                Spawn();
        }
        
        public void Spawn()
        {
            VoicePhishingSO data = phishingDataList.voicePhishingData
                [Random.Range(0, phishingDataList.voicePhishingData.Length)];
            VoicePhishing voicePhishing = Instantiate(voicePhishingPrefab, spawnCanvas.transform);
            voicePhishing.EndEvent += HandleEnd;
            voicePhishing.SetData(data, daySystem);
            voicePhishing.PopUp();
        }

        private void HandleEnd()
        {
            StartCoroutine(TrySpawn());
        }
        
        public void SetSpawnCooldown(float cooldown)
        {
            spawnCooldown = cooldown;
        }
    }
}