using UnityEngine;
using UnityEngine.Serialization;

namespace A.Work.CHUH._01.Script.VoicePhishing
{
    public class VoicePhishingSpawner : MonoBehaviour
    {
        [SerializeField] private Canvas spawnCanvas;
        [SerializeField] private VoicePhishing voicePhishingPrefab;
        [SerializeField] private PhishingDataList phishingDataList; 
        
        #region Test

        [ContextMenu("Spawn")]
        public void TestSpawn()
        {
            Spawn();
        }

        #endregion
        
        public void Spawn()
        {
            VoicePhishingSO data = phishingDataList.voicePhishingData
                [Random.Range(0, phishingDataList.voicePhishingData.Length)];
            VoicePhishing voicePhishing = Instantiate(voicePhishingPrefab, spawnCanvas.transform);
            voicePhishing.SetData(data);
            voicePhishing.PopUp();
        }
    }
}