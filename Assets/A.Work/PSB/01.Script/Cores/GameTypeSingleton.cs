using System;
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

        [SerializeField] private GameType _gameType;

        public GameType GameType
        {
            get => _gameType;
            set
            {
                if (_gameType != value)
                {
                    _gameType = value;
                    OnGameTypeChanged?.Invoke(_gameType);
                }
            }
        }

        public event Action<GameType> OnGameTypeChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            GameType = GameType.Game;
        }
    }
    
}