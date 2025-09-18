using UnityEngine;

namespace Scripts.Cores
{
    public enum GameType
    {
        Game = 0, 
        Result = 1, 
        Story = 2, 
        Clear = 3, 
        Fail = 4
    }
    
    public class GameTypeSingleton : MonoBehaviour
    {
        public static GameTypeSingleton Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public GameType GameType;

        private void Start()
        {
            GameType = GameType.Game;
        }
        
    }
}