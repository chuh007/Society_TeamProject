using System;
using UnityEngine;

namespace Scripts.Cores
{
    public class GameBooleanSingleton : MonoBehaviour
    {
        public static GameBooleanSingleton Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public bool IsGame = false;
        public bool IsResult = false;
        public bool IsStory = false;
        
        public bool IsGameClear = false;
        public bool IsGameFail = false;

        private void Start()
        {
            IsGame = true;
        }
        
    }
}