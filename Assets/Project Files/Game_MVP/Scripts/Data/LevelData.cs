using UnityEngine;
using System.Collections.Generic;

namespace SquadShooterMVP
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "Game/LevelData")]
    public class LevelData : ScriptableObject
    {
        public string LevelName = "Level 1";
        public string LevelDescription = "";
        public List<WaveConfig> Waves; 
    }

    [System.Serializable]
    public class WaveConfig
    {
        public float WaveTimer = 60f;
        public int EnemyCount = 5;
        public float SpawnInterval = 1.5f;
        public GameObject EnemyPrefab;
    }
}